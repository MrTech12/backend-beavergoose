using FileMicroservice.Data;
using FileMicroservice.DTOs;
using FileMicroservice.Helpers;
using FileMicroservice.Interfaces;
using FileMicroservice.Messaging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
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
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IFileProvider, DigitalOceanFileProvider>();
builder.Services.AddScoped<IMessagingProducer, RabbitMQProducer>();

builder.Services.Configure<FormOptions>(x => { x.MultipartBodyLengthLimit = 524288000; });

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

// Adding Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
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

app.Run();