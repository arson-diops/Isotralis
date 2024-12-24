using NLog;
using NLog.Web;
using Oracle.ManagedDataAccess.Client;
using Microsoft.Extensions.Logging;
using Isotralis.Infrastructure.Repositories.Nims;

Logger logger = LogManager
    .Setup()
    .LoadConfigurationFromAppSettings(optional: false, reloadOnChange: true)
    .GetCurrentClassLogger();

try
{
    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    logger.Info($"Building application with environment '{builder.Environment.EnvironmentName}'.");

    // Setup application configuration sources
    builder.Configuration
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
        .AddEnvironmentVariables()
        .Build();

    logger.Debug("Configuration components are wired.");

#if !DEBUG
    // Add data protection to use ephemeral keys. Used for invalidation of cookies in cases of app crashes / restarts.
    builder.Services.AddDataProtection(options => options.ApplicationDiscriminator = "Isotralis")
                    .UseEphemeralDataProtectionProvider();

    logger.Debug("Ephemeral data protection provider is wired.");
#endif

    // Add NLog as logging provider
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    logger.Debug("NLog logging is wired.");

    // Add controllers and views
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    logger.Debug("Controllers are wired.");

    // Add repositories
    builder.Services.AddSingleton<NimsPersonsRepository>();


    logger.Debug("Repositories are wired.");

    WebApplication app = builder.Build();

    // HTTP request pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Isotralis API V1");
        });
    }

    // Add middleware for routing requests
    app.UseRouting();

    logger.Debug("Routing is in use.");

    // Add middleware for authentication and authorization
    app.UseAuthentication();
    app.UseAuthorization();

    logger.Debug("Authentication and authorization are in use.");

    // Map controller routes
    app.MapControllers();

    logger.Debug("Controller routes are in use.");

    // Special configuration to connect to older Oracle databases
    OracleConfiguration.SqlNetAllowedLogonVersionClient = OracleAllowedLogonVersionClient.Version11;

    logger.Info("Starting application.");

    // Run the application
    await app.RunAsync();

    logger.Info("Application shutting down...");
}
catch (AggregateException e)
{
    logger.Error(e);
}
catch (Exception e)
{
    logger.Error(e, "Error occurred during application startup");
}
finally
{
    LogManager.Flush();
    LogManager.Shutdown();
}