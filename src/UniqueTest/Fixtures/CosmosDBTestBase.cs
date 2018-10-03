using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CosmosDBTest.Common;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Xunit.Abstractions;

namespace UniqueTest.Fixtures
{
    public abstract  class CosmosDBTestBase
    {
        protected readonly CosmosDBFixture DB;
        protected readonly ITestOutputHelper Output;
        protected readonly IDocumentClient Client;

        protected CosmosDBTestBase(CosmosDBFixture db, ITestOutputHelper output)
        {
            DB = db;
            Output = output;
            Client = DB.DocumentClient;
        }

        protected  Uri CreateDocumentCollectionUri()
        {
            return UriFactory.CreateDocumentCollectionUri(DB.DatabaseId, DB.CollectionId);
        }

        protected async Task<dynamic[]> CreateDocuments(dynamic[] data, string message)
        {
            var results = new List<dynamic>();

            foreach (var item in data)
            {
                try
                {
                    var response = await Client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri( DB.DatabaseId, DB.CollectionId), item);
                    results.Add(response);
                }
                catch (DocumentClientException e)
                {
                    Output.WriteLine("{0}: {1}, {2}", message, Dump(item), e.Message);
                    throw;
                }
            }

            return results.ToArray();
        }

        protected async Task DumpQuery(string query)
        {
            var result = Client
                .CreateDocumentQuery(CreateDocumentCollectionUri(), query)
                .AsDocumentQuery();

            while (result.HasMoreResults)
            {
                var feed = await result.ExecuteNextAsync();
                Output.WriteLine("query:{0}, activityId:{1},requestCharge:{2},count:{3}", query, feed.ActivityId, feed.RequestCharge, feed.Count);
                foreach (var o in feed)
                {
                    Output.WriteLine(Dump(o));
                }
            }
        }

        protected string Dump(object o)
        {
            return string.Join(", ", o.GetType().GetProperties().Select(pi => $"{pi.Name}: {pi.GetValue(o)}"));
        }
    }
}

