﻿using CosmosDBTest.Common;
using Xunit.Abstractions;

namespace UniqueTest.Fixtures
{
    public class SimpleFixture : CosmosDBFixture
    {
        public SimpleFixture(IMessageSink messageSink) : base(messageSink)
        {
            Initialize("Fixtures/SimpleFixture.json");
        }
    }
}
