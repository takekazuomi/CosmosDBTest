using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CosmosDBTest.Common;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
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

        protected async Task<dynamic> CreateDocuments(dynamic[] data, string message)
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

            return results;
        }

        protected string Dump(object o)
        {
            return string.Join(", ", o.GetType().GetProperties().Select(pi => $"{pi.Name}: {pi.GetValue(o)}"));
        }
    }
}

