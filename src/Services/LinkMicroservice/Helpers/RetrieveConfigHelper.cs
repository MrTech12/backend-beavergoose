namespace LinkMicroservice.Helpers
{
    public class RetrieveConfigHelper
    {
        private readonly IConfiguration _configuration;

        public RetrieveConfigHelper(IConfiguration configuration)
        {
            this._configuration = configuration;
        }

        public string GetConfigValue(string configurationSection, string configurationKey)
        {
            var environmentType = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (environmentType == "Development")
            {
                var configurationValue = this._configuration[$"{configurationSection}:{configurationKey}"];

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
