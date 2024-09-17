using FileMicroservice.Data;
using FileMicroservice.Helpers;
using FileMicroservice.Interfaces;
using FileMicroservice.Messaging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
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
    options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
    options.ReportApiVersions = true;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v2", new OpenApiInfo
    {
        Version = "v2",
        Title = "FileService API",
        Description = "API Documentation of the FileService API which retrieves files during upload and can download them back."
    });

    // using System.Reflection;
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

// Configure Serilog Logging
var retrieveConfigHelper = new RetrieveConfigHelper();
builder.Host.UseSerilog((ctx, lc) => lc.ReadFrom.Configuration(builder.Configuration)
    .MinimumLevel.Information()
    .Enrich.WithProperty("Application", builder.Environment.ApplicationName)
    .Enrich.WithProperty("Environment", Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"))
    .Enrich.WithExceptionDetails()
    .Enrich.FromLogContext()
    .WriteTo.Console(outputTemplate: "[{Timestamp:dd-MM-yyyy HH:mm:ss}] [{Level}] ({SourceContext}) {Message}{NewLine}{Exception}")
    .WriteTo.Seq(retrieveConfigHelper.GetConfigValue("Seq", "ServerUrl"), apiKey: retrieveConfigHelper.GetConfigValue("Seq", "ApiKey")));

Serilog.Debugging.SelfLog.Enable(Console.Error);

builder.Services.AddScoped<IFileProvider, DigitalOceanFileProvider>();
builder.Services.AddScoped<IMessagingProducer, RabbitMQProducer>();
builder.Services.AddScoped<IRetrieveConfigHelper, RetrieveConfigHelper>();
builder.Services.AddScoped<IDeleteFileHelper, DeleteFileHelper>();
builder.Services.AddScoped<IRetrieveExternalSecretHelper, RetrieveExternalSecretHelper>();

builder.Services.Configure<FormOptions>(x => { x.MultipartBodyLengthLimit = 524288000; });

// Add JWT verification
var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(retrieveConfigHelper.GetConfigValue("JWT", "Secret")));
var authIssuer = retrieveConfigHelper.GetConfigValue("JWT", "Issuer");

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