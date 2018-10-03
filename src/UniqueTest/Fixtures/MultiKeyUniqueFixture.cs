using CosmosDBTest.Common;
using Xunit.Abstractions;

namespace UniqueTest.Fixtures
{
    public class MultiKeyUniqueFixture : CosmosDBFixture
    {
        public MultiKeyUniqueFixture(IMessageSink messageSink) : base(messageSink, "Fixtures/MultiKeyUniqueFixture.json")
        {
            Initialize();
        }
    }
}
