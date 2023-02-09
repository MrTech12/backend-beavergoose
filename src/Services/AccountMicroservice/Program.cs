using AccountMicroservice.Data;
using AccountMicroservice.Models;
using Common.Configuration.Helpers;
using Common.Configuration.Interfaces;
using Common.Http.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Exceptions;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    options.ReportApiVersions = true;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "AccountService API",
        Description = "API Documentation of the AccountService API which creates accounts and retrieves account data."
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

var localConfigHelper = new LocalConfigHelper();

// Configure Serilog Logging
builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(builder.Configuration)
    .MinimumLevel.Information()
    .Enrich.WithProperty("Application", builder.Environment.ApplicationName)
    .Enrich.WithProperty("Environment", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"))
    .Enrich.WithExceptionDetails()
    .Enrich.FromLogContext()
    .WriteTo.Console(outputTemplate: "[{Timestamp:dd-MM-yyyy HH:mm:ss}] [{Level}] ({SourceContext}) {Message}{NewLine}{Exception}")
    .WriteTo.Seq(localConfigHelper.GetConfigValue("Seq", "ServerUrl"), apiKey: localConfigHelper.GetConfigValue("Seq", "ApiKey")));

Serilog.Debugging.SelfLog.Enable(Console.Error);

// For Entity Framework
builder.Services.AddSingleton<AccountContext>();
builder.Services.AddScoped<IConfigHelper, LocalConfigHelper>();

// For Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.User.RequireUniqueEmail = true;
}).AddEntityFrameworkStores<AccountContext>().AddDefaultTokenProviders();

// Adding Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
});

// For JWT
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JWT"));

// For hashing password
builder.Services.Configure<PasswordHasherOptions>(option => option.CompatibilityMode = PasswordHasherCompatibilityMode.IdentityV3);

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true); // Needed for saving DateTime variables

// Add JWT verification
var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(localConfigHelper.GetConfigValue("JWT", "Secret")));
var authIssuer = localConfigHelper.GetConfigValue("JWT", "Issuer");

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

// Adding Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.SaveToken = true;
    x.TokenValidationParameters = tokenValidationParameters;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseSerilogRequestLogging(options => {
    options.EnrichDiagnosticContext = HttpContextEnricherHelper.HttpRequestEnricher;
});

app.Run();
