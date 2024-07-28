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
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                    .AddJsonFile($"appsettings.{hostingContext.HostingEnvironment.EnvironmentName}.json", optional: true, reloadOnChange: false)
                    .AddJsonFile($"ocelot.{hostingContext.HostingEnvironment.EnvironmentName}.json", optional: false, reloadOnChange: false)
                    .AddEnvironmentVariables();
            })
            .ConfigureServices(s => {

                s.AddCors(options =>
                {
                    options.AddPolicy(name: "CorsPolicy",
                        builder => builder.WithOrigins(new string[] { })
                                .AllowAnyMethod()
                                .AllowAnyHeader());
                });

                // Add JWT verification
                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(RetrieveConfigHelper.GetConfigValue("JWT", "Secret")));
                var authIssuer = RetrieveConfigHelper.GetConfigValue("JWT", "Issuer");

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