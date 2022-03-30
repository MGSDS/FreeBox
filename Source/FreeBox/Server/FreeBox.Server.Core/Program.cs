using FreeBox.Server.Core.CompressionAlgorithms;
using FreeBox.Server.Core.Interfaces;
using FreeBox.Server.Core.Repositories;
using FreeBox.Server.Core.Services;
using FreeBox.Server.DataAccess;
using Microsoft.EntityFrameworkCore;
using NLog.Web;
using NLog;

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
    builder.Services.AddDbContext<BusinessContext>(options =>
    {
        options.UseSqlServer(connectionString ?? 
                             throw new ArgumentException("No ConnectionString specified"));
    });
    
    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
    builder.Host.UseNLog();
    builder.Services.AddScoped<IUserService, UserService>();
    builder.Services.AddScoped<ICompressionAlgorithm, ZipCompressor>();
    var repositoryPath = builder.Configuration.GetSection("RepositoryPath").Value;
    builder.Services.AddScoped<IRepository, LocalFsRepository>(x =>
        ActivatorUtilities.CreateInstance<LocalFsRepository>(x, repositoryPath));
    builder.Services.AddScoped<IFileService, FileService>();


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