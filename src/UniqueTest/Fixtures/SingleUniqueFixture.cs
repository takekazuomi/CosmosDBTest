using CosmosDBTest.Common;
using Xunit.Abstractions;

namespace UniqueTest.Fixtures
{
    public class SingleUniqueFixture : CosmosDBFixture
    {
        public SingleUniqueFixture(IMessageSink messageSink) : base(messageSink)
        {
            Initialize("Fixtures/SingleUniqueFixture.json");
        }
    }
}
