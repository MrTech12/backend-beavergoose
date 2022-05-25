using APIGateway.Helpers;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using System.Text;

namespace OcelotBasic
{
    public class Program
    {
        public static void Main(string[] args)
        {
            new WebHostBuilder()
            .UseKestrel()
            .UseContentRoot(Directory.GetCurrentDirectory())
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                config
                    .SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
                    .AddJsonFile("appsettings.json", true, true)
                    .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", true, true)
                    .AddJsonFile($"ocelot.{hostingContext.HostingEnvironment.EnvironmentName}.json", false, true)
                    .AddEnvironmentVariables();
            })
            .ConfigureServices(s => {

                s.AddCors(options =>
                {
                    options.AddPolicy(name: "CorsPolicy",
                        //builder => builder.AllowAnyOrigin()
                        builder => builder.WithOrigins("http://localhost:4200")
                            .AllowAnyMethod()
                            .AllowAnyHeader());
                });

                // Add JWT verification
                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(RetrieveConfigHelper.GetConfigValue("JWT", "Secret")));
                var authIssuer = RetrieveConfigHelper.GetConfigValue("JWT", "Issuer");
                var expireDate = RetrieveConfigHelper.GetConfigValue("JWT", "ExpirationInDays");
                var signCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256);

                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = authSigningKey,
                    ValidateIssuer = true,
                    ValidIssuer = authIssuer,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    RequireExpirationTime = true,
                };
                s.AddAuthentication().AddJwtBearer("TestKey", x =>
                {
                    x.SaveToken = true;
                    x.TokenValidationParameters = tokenValidationParameters;
                });               

                s.AddOcelot();
            })
            .ConfigureLogging((hostingContext, logging) =>
            {
                //add your logging                
                logging.AddConsole();
            })
            .UseIISIntegration()
            .Configure(app =>
            {
                app.UseCors("CorsPolicy");

                app.UseOcelot().Wait();
            })
            .Build()
            .Run();
        }
    }
}