using FreeBox.Server.Core;
using FreeBox.Server.Core.CompressionAlgorithms;
using FreeBox.Server.Core.Interfaces;
using FreeBox.Server.Core.Services;
using FreeBox.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NLog.Web;
using NLog;
using Microsoft.AspNetCore.Authentication.JwtBearer;

Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");
var config = new ConfigurationManager();

try
{
    var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

    builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddScoped<ICompressionAlgorithm, ZipCompressor>();
    var connectionString = builder.Configuration.GetSection("ConnectionString").Value;
    builder.Services.AddDbContext<FreeBoxContext>(options =>
    {
        options.UseSqlServer(connectionString ?? 
                             throw new ArgumentException("No ConnectionString specified"));
    });
    
    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
    builder.Host.UseNLog();
    builder.Services.AddScoped<ICompressionAlgorithm, ZipCompressor>();
    builder.Services.AddScoped<IFileService, FileService>();
    builder.Services.AddScoped<IUserService, UserService>();
    
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = AuthOptions.ISSUER,
                ValidateAudience = true,
                ValidAudience = AuthOptions.AUDIENCE,
                ValidateLifetime = true,
                IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                ValidateIssuerSigningKey = true,
            };
        });


    var app = builder.Build();


// Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }


    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception exception)
{
    logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally
{
    NLog.LogManager.Shutdown();
}