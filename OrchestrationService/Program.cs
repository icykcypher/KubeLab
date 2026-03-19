using Serilog;
using Prometheus;
using MongoDB.Driver;
using Newtonsoft.Json;
using Serilog.Exceptions;
using OpenTelemetry.Trace;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using Serilog.Sinks.Grafana.Loki;
using OrchestrationService.Shared;
using OrchestrationService.Model.Dtos;
using OrchestrationService.Repositories;
using Microsoft.AspNetCore.CookiePolicy;
using OrchestrationService.ApiExtensions;
using OrchestrationService.Model.Options;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using OrchestrationService.Model.Tokens;
using OrchestrationService.Services.Authorization;
using OrchestrationService.Services.DataInitializer;
using OrchestrationService.Services.TokenProvider;
using OrchestrationService.Services.PasswordHashers;
using OrchestrationService.Services.MongoInitializers;
using OrchestrationService.Services.GlobalExceptionHandler;
using OrchestrationService.Services.RedisStore;

namespace OrchestrationService;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration.AddEnvironmentVariables();

        builder.WebHost.ConfigureKestrel(options =>
        {
            options.ListenAnyIP(8080, listenOptions => { listenOptions.Protocols = HttpProtocols.Http1AndHttp2; });
        });

        var env = builder.Configuration["ENV"]
                  ?? throw new InvalidOperationException("ENV environment variable not set");
        var appId = builder.Configuration["APP_ID"]
                    ?? throw new InvalidOperationException("APP_ID environment variable not set");
        var appName = builder.Configuration["APP_NAME"]
                      ?? throw new InvalidOperationException("APP_NAME environment variable not set");

        builder.Host.UseSerilog((context, config) =>
        {
            config.Enrich.FromLogContext();
            config.Enrich.WithExceptionDetails();
            config.Enrich.With(new SerilogEnricher(
                appId,
                appName,
                env
            ));
            if (builder.Environment.IsProduction())
            {
                config
                    .WriteTo.GrafanaLoki
                    (
                        $"http://loki:{builder.Configuration["LOKI_PORT"]
                                       ?? throw new InvalidOperationException("LOKI_PORT environment variable not set")}",
                        new List<LokiLabel>
                        {
                            new() { Key = "appId", Value = appId }, new() { Key = "appName", Value = appName },
                            new() { Key = "env", Value = env }
                        }
                    );
            }

            config.WriteTo.Console
            (
                outputTemplate:
                "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} level=[{Level:u3}] appId={ApplicationId} appName={ApplicationName} env={Environment} {Message:lj} {NewLine}{Exception}"
            );
            config.ReadFrom.Configuration(context.Configuration);
        });

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAllOrigins",
                builderPolicy => builderPolicy.WithOrigins(builder.Configuration["CORS_ORIGIN"]
                                                           ?? throw new InvalidOperationException(
                                                               "CORS_ORIGIN environment variable not set"))
                    .AllowCredentials()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
        });

        
        
        var workFactor = builder.Configuration.GetValue<int>("PASSWORDHASHING:WorkFactor") ;
        var enhancedEntropy = builder.Configuration.GetValue<bool>("PASSWORDHASHING:EnhancedEntropy");
        
        builder.Services.Configure<PasswordHashingOptions>(options =>
        {
            options.WorkFactor = workFactor; 
            options.EnhancedEntropy = enhancedEntropy;
        });
        
        var jwtSecret = builder.Configuration.GetValue<string>("JWT:SecretKey")
                        ?? throw new InvalidOperationException("JWT:SecretKey environment variable not set");
        var expireHours = builder.Configuration.GetValue<int>("JWT:ExpireHours");
        var issuer = builder.Configuration.GetValue<string>("JWT:Issuer")
                     ?? throw new InvalidOperationException("JWT:Issuer environment variable not set");
        var audience = builder.Configuration.GetValue<string>("JWT:Audience")
                     ?? throw new InvalidOperationException("JWT:Audience environment variable not set");
        
        var refreshTokenLifetimeDays = builder.Configuration.GetValue<int>("JWT:RefreshTokenLifetimeDays");
        
        builder.Services.Configure<TokenOptions>(options =>
        {
            options.ExpireHours = expireHours; 
            options.SecretKey = jwtSecret;
            options.Issuer = issuer;
            options.Audience = audience;
            options.RefreshTokenLifetimeDays = refreshTokenLifetimeDays;
        });
        
        var adminsJson = Environment.GetEnvironmentVariable("ADMINS_JSON")
                   ?? throw new InvalidOperationException("ADMINS_JSON not set");

        var usersJson = Environment.GetEnvironmentVariable("USERS_JSON")
                   ?? throw new InvalidOperationException("USERS_JSON not set");
        
        var admins = JsonConvert.DeserializeObject<List<AdminSeedingDto>>(adminsJson)!;
        var users = JsonConvert.DeserializeObject<UsersSeedingDto>(usersJson);
        if (builder.Environment.IsProduction())
        {
            var otel = builder.Services.AddOpenTelemetry();

            // Configure OpenTelemetry Resources with the application name
            otel.ConfigureResource(resource =>
            {
                resource.AddService(serviceName: $"{appName}");
                var globalOpenTelemetryAttributes = new List<KeyValuePair<string, object>>();
                globalOpenTelemetryAttributes.Add(new KeyValuePair<string, object>("env", env));
                globalOpenTelemetryAttributes.Add(new KeyValuePair<string, object>("appId", appId));
                globalOpenTelemetryAttributes.Add(new KeyValuePair<string, object>("appName", appName));
                resource.AddAttributes(globalOpenTelemetryAttributes);
            });

            // Add Metrics for ASP.NET Core and our custom metrics and export to Prometheus
            otel.WithMetrics(metrics => metrics
                .AddOtlpExporter(otlpOptions =>
                {
                    otlpOptions.Endpoint =
                        new Uri($"http://prometheus:{builder.Configuration["PROMETHEUS_PORT"]
                                                     ?? throw new InvalidOperationException("PROMETHEUS_PORT environment variable not set")}");
                })
                // Metrics provider from OpenTelemetry
                .AddAspNetCoreInstrumentation()
                .AddMeter(appName)
                // Metrics provides by ASP.NET Core in .NET 8
                .AddMeter("Microsoft.AspNetCore.Hosting")
                .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
                .AddPrometheusExporter());

            // Add Tracing for ASP.NET Core and our custom ActivitySource and export to Jaeger
            otel.WithTracing(tracing =>
            {
                tracing.AddAspNetCoreInstrumentation();
                tracing.AddHttpClientInstrumentation();
                tracing.AddSource(appName);
                tracing.AddOtlpExporter(otlpOptions =>
                {
                    otlpOptions.Endpoint =
                        new Uri($"http://jaeger:{builder.Configuration["JAEGER_PORT"]
                                                 ?? throw new InvalidOperationException("JAEGER_PORT environment variable not set")}");
                });
                tracing.AddConsoleExporter();
            });
        }

        builder.Services.AddControllers()
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddSingleton(Log.Logger);
        builder.Services.AddSingleton<Serilog.Extensions.Hosting.DiagnosticContext>();

        builder.Services.Configure<AuthorizationOptions>(
            builder.Configuration.GetSection(nameof(AuthorizationOptions)));

        builder.Services.AddSingleton<IMongoClient>(sp =>
        {
            var connectionString = builder.Configuration["ConnectionString:mongo"];
            return new MongoClient(connectionString);
        });

        builder.Services.AddScoped<IMongoDatabase>(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            var dbName = builder.Configuration["MongoDbSettings:Database"];
            return client.GetDatabase(dbName);
        });
        
        builder.Services.AddHealthChecks();
        
        builder.Services.AddScoped<ITokenProvider, TokenProvider>();
        builder.Services.AddScoped<IRedisRefreshTokenStore, RedisRefreshTokenStore>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<ProductionDataInitializer>();
        
        builder.Services.AddApiAuthentication(builder.Configuration);
        builder.Services.UseHttpClientMetrics();
        builder.Services.AddScoped<MongoInitializer>();
        
        var app = builder.Build();

        app.UseRouting();

        app.UseMiddleware<GlobalExceptionHandler>();

        using var scope = app.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        try
        {
            var mongoInitializer = scope.ServiceProvider
                .GetRequiredService<MongoInitializer>();

            await mongoInitializer.InitializeAsync();

            var initializer = scope.ServiceProvider
                .GetRequiredService<ProductionDataInitializer>();

            await initializer.InitializeAsync(admins, users);
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Database migration or seeding failed");
            throw;
        }

        app.UseCors("AllowAllOrigins");

        app.UseSerilogRequestLogging();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseCookiePolicy(new CookiePolicyOptions
        {
            MinimumSameSitePolicy = SameSiteMode.None,
            HttpOnly = HttpOnlyPolicy.Always,
            Secure = CookieSecurePolicy.None
        });

        app.UseAuthentication();
        
        app.UseAuthorization();

        app.MapControllers();
        app.MapHealthChecks("/health");

        app.UseMetricServer();
        app.UseHttpMetrics();

        await app.RunAsync();
    }
}