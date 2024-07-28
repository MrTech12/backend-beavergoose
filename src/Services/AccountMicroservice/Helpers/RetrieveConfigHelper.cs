namespace AccountMicroservice.Helpers
{
    public static class RetrieveConfigHelper
    {
        public static string GetConfigValue(string configurationSection, string configurationKey)
        {
            var environmentType = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (environmentType == "Development")
            {
                IConfiguration conf = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true, reloadOnChange: false).Build();
                var configurationValue = conf[$"{configurationSection}:{configurationKey}"];

                if (!string.IsNullOrEmpty(configurationValue))
                {
                    return configurationValue;
                }
                throw new MissingFieldException("Configuration value not set");
            }
            else if (environmentType == "Production")
            {
                var environmentValue = Environment.GetEnvironmentVariable($"{configurationSection}_{configurationKey}");
                if (!string.IsNullOrEmpty(environmentValue))
                {
                    return environmentValue;
                }
                throw new MissingFieldException("Environment value not set");
            }
            throw new MissingFieldException("Environment type not set.");
        }
    }
}
