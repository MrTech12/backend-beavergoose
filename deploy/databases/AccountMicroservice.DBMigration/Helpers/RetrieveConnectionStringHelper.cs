using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountMicroservice.DBMigration.Helpers
{
    public static class RetrieveConnectionStringHelper
    {
        public static string GetConnectionString()
        {
            var environmentType = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
            if (environmentType == "Development")
            {
                IConfiguration conf = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
                string connectionString = conf.GetConnectionString("AccountContext");

                if (!string.IsNullOrEmpty(connectionString))
                {
                    return connectionString;
                }
                throw new MissingFieldException("Connectionstring in appsettings.json not set");
            }
            else if (environmentType != "DOTNET_ENVIRONMENT")
            {
                var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings_AccountContext");
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
