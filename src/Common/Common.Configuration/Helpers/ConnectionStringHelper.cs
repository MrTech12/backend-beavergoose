using Microsoft.Extensions.Configuration;

namespace Common.Configuration.Helpers
{
    public static class ConnectionStringHelper
    {
        public static string GetConnectionString(string databaseContext)
        {
            var environmentTypeAsp = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            var environmentTypeDotnet = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            string connectionString;

            if (environmentTypeAsp == "Development" || environmentTypeDotnet == "Development")
            {
                IConfiguration configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile($"appsettings.json").Build();
                connectionString = configuration.GetConnectionString(databaseContext) ?? string.Empty;
            }
            else
            {
                IConfiguration configuration = new ConfigurationBuilder().AddEnvironmentVariables().Build();
                connectionString = configuration[$"{databaseContext}"] ?? string.Empty;
            }

            if (!string.IsNullOrEmpty(connectionString))
            {
                return connectionString;
            }
            throw new MissingFieldException($"Connectionstring for {databaseContext} not found.");
        }
    }
}