using Isotralis.App.Middleware;
using Isotralis.App.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.DataProtection;
using NLog;
using NLog.Web;
using Oracle.ManagedDataAccess.Client;

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
    builder.Services.AddControllersWithViews();

    logger.Debug("Controller views are wired.");

    // Add services
    builder.Services.AddSingleton<WindowsAuthenticationService>();

    logger.Debug("Services are wired.");

    // Add repositories

    logger.Debug("Repositories are wired.");

    // Add exception handlers
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

    logger.Debug("Exception handlers are wired.");

    builder.Services.AddMemoryCache();

    logger.Debug("In memory cache is wired.");

    // Add cookie authentication
    builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.SlidingExpiration = true;
            options.ExpireTimeSpan = TimeSpan.FromDays(7);
            options.LoginPath = "/Login/Index";
            options.AccessDeniedPath = "/Error/Index/403";
            options.Cookie = new CookieBuilder
            {
                HttpOnly = true,
                SecurePolicy = CookieSecurePolicy.SameAsRequest,
                SameSite = SameSiteMode.Strict,
                IsEssential = true,
                Name = "Isotralis.Cookie"
            };
        });

    logger.Debug("Cookie authentication is wired.");

    WebApplication app = builder.Build();

    if (!app.Environment.IsDevelopment())
    {
        // Production error handling
        app.UseExceptionHandler("/Error/Index/500");
    }
    else
    {
        // Development error handling
        app.UseDeveloperExceptionPage();
    }



    logger.Debug("Exception handlers are in use.");

    // Serve static front-end files under wwwroot
    app.UseStaticFiles();

    logger.Debug("Static files are in use.");

    // Add middleware for routing requests
    app.UseRouting();

    logger.Debug("Routing is in use.");

    // Add middleware for authentication and authorization
    app.UseAuthentication();
    app.UseAuthorization();

    logger.Debug("Authentication and authorization are in use.");

    // Map area controller routes
    app.MapControllerRoute("areas", "{area:exists}/{controller=Home}/{action=Index}/{id?}");

    logger.Debug("Area controller routes are in use.");

    // Map controller routes
    app.MapControllerRoute("default", "{controller=Login}/{action=Index}/{id?}");
    app.MapFallbackToController("Index", "Login");

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