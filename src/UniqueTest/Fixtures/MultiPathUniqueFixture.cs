using CosmosDBTest.Common;
using Xunit.Abstractions;

namespace UniqueTest.Fixtures
{
    public class MultiPathUniqueFixture : CosmosDBFixture
    {
        public MultiPathUniqueFixture(IMessageSink messageSink) : base(messageSink, "Fixtures/MultiPathUniqueFixture.json")
        {
            Initialize();
        }
    }
}
