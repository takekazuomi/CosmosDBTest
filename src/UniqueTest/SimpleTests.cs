using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CosmosDBTest.Common;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using UniqueTest.Fixtures;
using Xunit;
using Xunit.Abstractions;

namespace UniqueTest
{
    public class SimpleTests : CosmosDBTestBase, IClassFixture<SimpleFixture>
    {
        public SimpleTests(SimpleFixture db, ITestOutputHelper output) : base(db, output)
        {
        }

        /// <summary>
        /// 単純作成
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task CreateDocumentTest()
        {
            dynamic[] uniqueData =
            {
                new {test = 1},
                new {test = 2}
            };

            await CreateDocuments(uniqueData, "CreateDocumentTest");

            var query = Client
//                .CreateDocumentQuery(CreateDocumentCollectionUri(), "select * from c where c.test = 1")
                .CreateDocumentQuery(CreateDocumentCollectionUri(), "select * from c")
                .AsDocumentQuery();

            while (query.HasMoreResults)
            {
                var feed = await query.ExecuteNextAsync();
                Output.WriteLine("{0}:{1},{2}", feed.ActivityId, feed.RequestCharge, feed.Count);
                foreach (var o in feed)
                {
                    Output.WriteLine(Dump(o));
                }
            }
        }
    }
}

