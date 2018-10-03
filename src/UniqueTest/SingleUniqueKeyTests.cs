using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CosmosDBTest.Common;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using UniqueTest.Fixtures;
using Xunit;
using Xunit.Abstractions;

namespace UniqueTest
{
    public class SingleUniqueKeyTests : CosmosDBTestBase, IClassFixture<SingleUniqueFixture>
    {
        public SingleUniqueKeyTests(SingleUniqueFixture db, ITestOutputHelper output):base(db, output)
        {
        }

        /// <summary>
        /// Unique index constraint 違反でのDocumentExceptionを確認する。
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task SimpleUniqueViolationTest()
        {
            var uniqueData = new[]
            {
                new {uid1 = 1}
            };


            await CreateDocuments(uniqueData, "SimpleUniqueViolationTest");

            var conflictData = new[]
            {
                new {uid1 = 1}
            };

            var exception = await Assert.ThrowsAnyAsync<DocumentClientException>(() => CreateDocuments( conflictData, "conflictData"));

            Assert.True(exception.StatusCode == HttpStatusCode.Conflict);
            Assert.Matches(".+Unique index constraint violation.+", exception.Message);

        }

        /// <summary>
        /// Idの重複があった場合のDocumentExceptionを確認
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task SimpleIdViolationTest()
        {
            var uniqueData = new[]
            {
                new {id="1"}
            };

            await CreateDocuments(uniqueData, "SimpleIdViolationTest");

            var conflictData = new[]
            {
                new {id="1"}
            };

            var exception = await Assert.ThrowsAnyAsync<DocumentClientException>(() => CreateDocuments( conflictData, "conflictData"));

            Assert.True(exception.StatusCode == HttpStatusCode.Conflict);
            Assert.Matches(".+Unique index constraint violation.+", exception.Message);

        }


        /// <summary>
        /// IdとUnique成約違反の両方があった場合のDocumentExceptionを確認
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async Task SimpleIdUniqueViolationTest()
        {
            var uniqueData = new[]
            {
                new {id="2", uid1=2}
            };

            await CreateDocuments(uniqueData, "SimpleIdUniqueViolationTest");

            var conflictData = new[]
            {
                new {id="2", uid1=2}
            };

            var exception = await Assert.ThrowsAnyAsync<DocumentClientException>(() => CreateDocuments( conflictData, "conflictData"));

            Assert.True(exception.StatusCode == HttpStatusCode.Conflict);
            Assert.Matches(".+Unique index constraint violation.+", exception.Message);
        }
    }
}

