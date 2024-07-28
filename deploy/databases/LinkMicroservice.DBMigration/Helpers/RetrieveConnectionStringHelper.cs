using Microsoft.Extensions.Configuration;

namespace LinkMicroservice.DBMigration.Helpers
{
    public static class RetrieveConnectionStringHelper
    {
        public static string GetConnectionString()
        {
            var environmentType = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
            if (environmentType == "Development")
            {
                IConfiguration conf = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true, reloadOnChange: false).Build();
                string connectionString = conf.GetConnectionString("LinkContext");

                if (!string.IsNullOrEmpty(connectionString))
                {
                    return connectionString;
                }
                throw new MissingFieldException("Connectionstring in appsettings.json not set");
            }
            else if (environmentType != "DOTNET_ENVIRONMENT")
            {
                var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings_LinkContext");
                if (!string.IsNullOrEmpty(connectionString))
                {
                    return connectionString;
                }
                throw new MissingFieldException("Connectionstring in Environment value not set");
            }
            return string.Empty;
        }
    }
}
