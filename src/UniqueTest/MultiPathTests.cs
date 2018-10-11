using System.Threading.Tasks;
using UniqueTest.Fixtures;
using Xunit;
using Xunit.Abstractions;

namespace UniqueTest
{
    public class MultiPathTests : CosmosDBTestBase, IClassFixture<MultiPathUniqueFixture>
    {
        public MultiPathTests(MultiPathUniqueFixture db, ITestOutputHelper output) : base(db, output)
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
                new {uid1=1, uid2=2}
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

            var result = await CreateDocuments(data, "UniqueTest");

            Assert.True(result.Length == 2);

            await DumpQuery("select * from c where c.uid1=2");
        }
    }
}

