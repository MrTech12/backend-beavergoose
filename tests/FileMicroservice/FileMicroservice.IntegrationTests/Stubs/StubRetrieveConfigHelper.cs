using Castle.Core.Configuration;
using FileMicroservice.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileMicroservice.IntegrationTests.Stubs
{
    public class StubRetrieveConfigHelper : IRetrieveConfigHelper
    {
        public string GetConfigValue(string configurationSection, string configurationKey)
        {
            IConfigurationRoot conf = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.test.json").Build();
            var configurationValue = conf[$"{configurationSection}:{configurationKey}"];
            return configurationValue;
        }
    }
}
