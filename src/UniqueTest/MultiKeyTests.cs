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
    public class MultiKeyTests : CosmosDBTestBase, IClassFixture<MultiKeyUniqueFixture>
    {
        public MultiKeyTests(MultiKeyUniqueFixture db, ITestOutputHelper output) : base(db, output)
        {
        }

        /// <summary>
        /// 単純作成
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task SimpleUniqueTest()
        {
            dynamic[] uniqueData =
            {
                new {uid1=1, uid2=1},
                new {uid1=2, uid2=2}
            };

            await CreateDocuments(uniqueData, "SimpleUniqueTest");

            await DumpQuery("select * from c");
        }

        [Fact]
        public async Task UniqueTest()
        {
            dynamic[] data =
            {
                new {uid1 = 2, uid2 = 1},
                new {uid1 = 2, uid2 = 2}
            };

            var exception = await Assert.ThrowsAnyAsync<DocumentClientException>(() => CreateDocuments( data, "conflictData"));

            Assert.True(exception.StatusCode == HttpStatusCode.Conflict);
            Assert.Matches(".+Unique index constraint violation.+", exception.Message);

            await DumpQuery("select * from c where c.uid1=2");
        }
    }
}

