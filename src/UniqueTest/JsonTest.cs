using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.Documents;
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

            Assert.Matches("", json);
        }

    }
}
