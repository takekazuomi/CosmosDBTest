using System;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace CosmosDBTest.Common
{
    internal static class ConfigurationExtensions
    {
        public static T GetOrDefault<T>(this IConfiguration configuration, string key, Func<T> defaultValue = null)
        {
            var section = configuration.GetSection(key);
            return section.GetChildren().Any()
                ? section.Get<T>()
                : defaultValue != null ? defaultValue() : default(T);
        }
    }
}