using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace FileMicroservice.UnitTests
{
    public class TestConfiguration
    {
        public IConfigurationRoot GetTestDataConfiguration()
        {
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.test.json", optional: false, reloadOnChange: false)
                .AddEnvironmentVariables()
                .Build();
        }
    }
}
