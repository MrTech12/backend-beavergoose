namespace LinkMicroservice.Helpers
{
    public static class RetrieveConnectionStringHelper
    {
        public static string GetConnectionString()
        {
            var environmentType = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (environmentType == "Development")
            {
                IConfiguration conf = (new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build());
                string connectionString = conf.GetConnectionString("LinkContext");

                if (!string.IsNullOrEmpty(connectionString))
                {
                    return connectionString;
                }
                throw new MissingFieldException("Connectionstring in appsettings.json not set");
            }
            else if (environmentType == "Production")
            {
                var connectionString = Environment.GetEnvironmentVariable($"ConnectionStrings_LinkContext");
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
