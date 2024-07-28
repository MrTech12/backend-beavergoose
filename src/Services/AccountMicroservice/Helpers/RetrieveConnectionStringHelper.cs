namespace AccountMicroservice.Helpers
{
    public static class RetrieveConnectionStringHelper
    {
        public static string GetConnectionString()
        {
            var environmentType = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (environmentType == "Development")
            {
                IConfiguration conf = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true, reloadOnChange: false).Build();
                string connectionString = conf.GetConnectionString("AccountContext");

                if (!string.IsNullOrEmpty(connectionString))
                {
                    return connectionString;
                }
                throw new MissingFieldException("Connectionstring in appsettings.json not set");
            }
            else if (environmentType == "Production")
            {
                var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings_AccountContext");
                if (!string.IsNullOrEmpty(connectionString))
                {
                    return connectionString;
                }
                throw new MissingFieldException("Connectionstring in Environment value not set");
            }
            throw new MissingFieldException("Environment type not set.");
        }
    }
}
