using Microsoft.Extensions.Configuration;
using System.IO;

namespace LinkMicroservice.UnitTests
{
    public class TestConfiguration
    {
        public IConfigurationRoot GetTestDataConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.test.json", optional: false, reloadOnChange: false)
                .AddEnvironmentVariables()
                .Build();
        }
    }
}
