using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace UniqueTest
{
    public class JsonTest
    {
        private readonly ITestOutputHelper _output;

        public JsonTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void UniqueKeyPolicyTest()
        {
            // initialize single unique key collection
            var uniqueKeyPolicy = new UniqueKeyPolicy
            {
                UniqueKeys =
                {
                    new UniqueKey
                    {
                        Paths = {"/uid1"}
                    },
                }
            };

            var json = JsonConvert.SerializeObject(uniqueKeyPolicy);
            _output.WriteLine(json);

            Assert.Matches(".+/uid1.+", json);
        }

        [Fact]
        public void ConnectionPolicyTest()
        {

            var connectionPolicy = new ConnectionPolicy
            {
                UserAgentSuffix = " unique-net/3",
                ConnectionMode = ConnectionMode.Direct,
                ConnectionProtocol = Protocol.Https
            };

            var json = JsonConvert.SerializeObject(connectionPolicy);
            _output.WriteLine(json);

            Assert.Matches(".+unique-net/3.+", json);
        }
    }
}
