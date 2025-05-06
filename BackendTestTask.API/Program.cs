using BackendTestTask.API.Models;
using BackendTestTask.AspNetExtensions.Filters;
using BackendTestTask.Database;
using BackendTestTask.Services;
using BackendTestTask.Services.Models.Settings;
using BackendTestTask.Services.Services.Implementations;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.OpenApi.Models;
using Npgsql;
using Polly;
using Polly.Extensions.Http;
using System.Reflection;

const string CorsPolicyKey = "CorsPolicy";

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var services = builder.Services;
var connectionString = configuration.GetConnectionString("DefaultConnection");

#region Service Registrations

services.AddControllers(options =>
{
    options.Filters.Add<ExceptionHandlerAttribute>();
    options.Filters.Add<ModelValidationAttribute>();
    options.Filters.Add<ResponseModelFilter>();
});

services.Configure<MeteoriteDataSettings>(
    builder.Configuration.GetSection("MeteoriteData"));

services.AddEndpointsApiExplorer();

services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "BackendTestTask API",
        Description = "An ASP.NET Core Web API for managing Meteorites",
        Contact = new OpenApiContact { Name = "Tasks" }
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
    options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
});

// Custom services
services.AddServices();
services.AddHttpClient<MeteoriteImporter>();


// Logging, EF, Hangfire
services
    .AddLogging(logger => logger.AddLog4Net())
    .AddOptions()
    .AddDbContext<BackendTestTaskContext>(options =>
        options.UseNpgsql(connectionString));

services.AddHangfire(config =>
    config.UsePostgreSqlStorage(opt => opt.UseNpgsqlConnection(connectionString)));

services.AddHangfireServer();
services.AddMemoryCache();

// Redis cache (optional)
/*
services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});
*/

services.AddCors(options => options.AddPolicy(CorsPolicyKey, policy =>
{
    policy.WithOrigins("http://localhost:8080") 
          .AllowAnyMethod()
          .AllowAnyHeader()
          .AllowCredentials();
}));


#endregion

var app = builder.Build();

#region Database Init and Hangfire Job Registration

using (var scope = app.Services.CreateScope()) // polly checking can be added
{    
    var db = scope.ServiceProvider.GetRequiredService<BackendTestTaskContext>();
    db.Database.Migrate();

    var jobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
    var importer = scope.ServiceProvider.GetRequiredService<MeteoriteImporter>();

    jobManager.AddOrUpdate(
        "MeteoriteDataSyncJob",
        () => importer.SyncMeteoritesAsync(),
        Cron.Hourly
    );
}

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    IgnoreAntiforgeryToken = true,
    Authorization = new[] { new AllowAllDashboardAuthorizationFilter() }
});


#endregion

#region Middleware

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(CorsPolicyKey);
app.MapControllers();
#endregion

app.Run();

#region Connection Validation (optional debug)

static bool IsConnectionStringValid(string connectionString)
{
    const int maxRetries = 5;
    const int delayMs = 2000;

    for (int attempt = 1; attempt <= maxRetries; attempt++)
    {
        try
        {
            using var connection = new NpgsqlConnection(connectionString);
            connection.Open();
            if (connection.State == System.Data.ConnectionState.Open)
            {
                Console.WriteLine($"✅ Connected on attempt {attempt}");
                return true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Attempt {attempt}: {ex.Message}");
            Thread.Sleep(delayMs);
        }
    }

    return false;
}
#endregion
