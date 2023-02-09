using Common.Configuration.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace FileMicroservice.IntegrationTests.Helpers
{
    public class StubConfigHelper : IConfigHelper
    {
        public string GetConfigValue(string section, string key)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.test.json").Build();
            var configValue = configuration[$"{section}:{key}"] ?? string.Empty;
            return configValue;
        }
    }
}
