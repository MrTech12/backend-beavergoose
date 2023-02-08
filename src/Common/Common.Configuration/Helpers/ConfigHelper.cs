using Microsoft.Extensions.Configuration;

namespace Common.Configuration.Helpers
{
    public static class ConfigHelper
    {
        public static string GetConfigValue(string section, string key)
        {
            var environmentTypeAsp = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var environmentTypeDotnet = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            string configValue = string.Empty;

            if (environmentTypeAsp == "Development" || environmentTypeDotnet == "Development")
            {
                IConfiguration configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile($"appsettings.json").Build();
                configValue = configuration[$"{section}:{key}"] ?? string.Empty;
            }
            else
            {
                IConfiguration configuration = new ConfigurationBuilder().AddEnvironmentVariables().Build();
                configValue = configuration[$"{section}_{key}"] ?? string.Empty;
            }

            if (!string.IsNullOrEmpty(configValue))
            {
                return configValue;
            }
            throw new MissingFieldException($"Configuration value for section: {section} & key: {key}' not found.");
        }
    }
}
