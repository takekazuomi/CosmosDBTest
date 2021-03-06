﻿using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.UserSecrets;
using Xunit.Abstractions;
using Xunit.Sdk;

// DBの課金が最低1時間なので、Fact毎にDBを作るのは高くなるし、時間もかかる。
// Shared Context between Tests
// https://xunit.github.io/docs/shared-context.html
[assembly: UserSecretsId("ba18f91b-e8a7-4aba-8c4a-888ca0202f40")]
namespace UniqueTest
{
    public class CosmosDBFixtureUnused : IDisposable
    {
        // class/collection fixtureでは、IMessageSink を使う。2.4 beta以降
        // Add diagnostic message sink option for fixtures https://github.com/xunit/xunit/issues/565
        // Allow to inject IMessageSink into ClassFixture/CollectionFixture https://github.com/xunit/xunit/pull/1705
        // Configuring xUnit.net with JSON
        // https://xunit.github.io/docs/configuring-with-json
        private readonly IMessageSink _messageSink;
        private readonly CosmosDBFixtureSettings _settings;

        private readonly string _accountEndpoint;
        private readonly string _accountKey;

        public string DatabaseId { get; } = "db";
        public string CollectionId { get; } = "col";

        private IDocumentClient _documentClient;
        public IDocumentClient DocumentClient
        {
            get
            {
                if (_documentClient == null)
                    lock (this)
                    {
                        if (_documentClient == null)
                            _documentClient = Init().Result;
                    }

                return _documentClient;
            }
            set => _documentClient = value;
        }

        private PartitionKeyDefinition _partitionKeyDefinition;
        /// <summary>
        /// パーテーション定義
        /// 変更したい場合は、DocumentClient にアクセスする前に行う
        /// </summary>
        public PartitionKeyDefinition PartitionKeyDefinition
        {
            get => _partitionKeyDefinition;
            set
            {
                if(_documentClient != null)
                    throw new InvalidOperationException();
                _partitionKeyDefinition = value;
            }
        }

        private UniqueKeyPolicy _uniqueKeyPolicy;
        /// <summary>
        /// ユニークキー定義
        /// 変更したい場合は、DocumentClient にアクセスする前に行う
        /// </summary>
        public UniqueKeyPolicy UniqueKeyPolicy
        {
            get => _uniqueKeyPolicy;
            set
            {
                if(_documentClient != null)
                    throw new InvalidOperationException();
                _uniqueKeyPolicy = value;
            }
        }


        private readonly ConnectionPolicy _connectionPolicy = new ConnectionPolicy
        {
            UserAgentSuffix = " unique-net/3", 
            ConnectionMode = ConnectionMode.Direct,
            ConnectionProtocol = Protocol.Https
        };

        public CosmosDBFixtureUnused(IMessageSink messageSink)
        {
            // _output = output;
            _messageSink = messageSink;
            _messageSink.OnMessage(new DiagnosticMessage("DatabaseFixture constructor message"));

            var builder = new ConfigurationBuilder();
            builder
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("settings.json")
                .AddUserSecrets<CosmosDBFixture>();
            IConfiguration configuration = builder.Build();
            _settings = configuration.Get<CosmosDBFixtureSettings>();

            _accountEndpoint = configuration.GetSection("CosmosDB:AccountEndpoint").Value;
            _accountKey = configuration.GetSection("CosmosDB:AccountKey").Value;
        }

        private async Task<IDocumentClient> Init()
        {
            var documentClient = new DocumentClient(new Uri(_accountEndpoint), _accountKey, _connectionPolicy, ConsistencyLevel.Session);

            // 作成前に削除する。費用はかかるがDBの内容が確認できる。
            if (!_settings.Cleanup)
                Cleanup(documentClient).Wait();

            await documentClient.CreateDatabaseIfNotExistsAsync(new Database {Id = DatabaseId});

            var documentCollection = new DocumentCollection
            {
                Id = CollectionId,
                UniqueKeyPolicy = UniqueKeyPolicy
            };

            if (PartitionKeyDefinition != null)
                documentCollection.PartitionKey = PartitionKeyDefinition;

            await documentClient.CreateDocumentCollectionIfNotExistsAsync(
                UriFactory.CreateDatabaseUri(DatabaseId),
                documentCollection,
                new RequestOptions {OfferThroughput = 400});

            return documentClient;
        }

        private async Task Cleanup(IDocumentClient client)
        {
            var database = client.CreateDatabaseQuery().Where(db => db.Id == DatabaseId).AsEnumerable().FirstOrDefault();
            if(database != null)
                await client.DeleteDatabaseAsync(UriFactory.CreateDatabaseUri(DatabaseId));
 
           _messageSink.OnMessage(new DiagnosticMessage($"DeleteDatabaseAsync: {DatabaseId}, {database != null}"));
        }

        public void Dispose()
        {
            if (DocumentClient != null)
            {
                if (_settings.Cleanup)
                    Cleanup(DocumentClient).Wait();
                (DocumentClient as IDisposable)?.Dispose();
                DocumentClient = null;
            }
        }
    }

    public class CosmosDBFixtureSettings
    {
        public bool Cleanup { get; set; } = false;
    }
}