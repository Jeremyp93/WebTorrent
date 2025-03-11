using MongoDB.Bson.Serialization.Conventions;
using AspNetCore.Identity.MongoDbCore.Extensions;
using AspNetCore.Identity.MongoDbCore.Infrastructure;
using AspNetCore.Identity.MongoDbCore.Models;
using MongoDB.Driver;
using Microsoft.AspNetCore.Identity;
using LouisManager.Api.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using LouisManager.Api.Repositories;
using LouisManager.Api.Options;
using LouisManager.Api.Services;
using LouisManager.Api.Helpers;

namespace LouisManager.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureAutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(Program));

        return services;
    }

    public static IServiceCollection ConfigureRepositories(this IServiceCollection services)
    {
        //transient service
        services.AddTransient<IGoogleOAuthRepository, GoogleOAuthRepository>();
        services.AddTransient<IEntryRepository, EntryRepository>();
        services.AddTransient<ITokenRepository, TokenRepository>();
        return services;
    }

    public static IServiceCollection ConfigureHelpers(this IServiceCollection services)
    {
        services.AddTransient<IDateTimeProvider, DateTimeProvider>();

        return services;
    }

    public static IServiceCollection ConfigureMongoDb(this IServiceCollection services, IConfiguration configuration)
    {
        var pack = new ConventionPack
        {
            new CamelCaseElementNameConvention()
        };

        ConventionRegistry.Register("camelCase", pack, t => true);

        var mongoDbSettings = configuration.GetSection("MongoDb");
        var mongoDbConn = string.Format(configuration.GetConnectionString("MongoDBConnection") ?? "", mongoDbSettings.GetValue<string>("Username"), mongoDbSettings.GetValue<string>("Password"));

        services.AddSingleton<IMongoDatabase>(serviceProvider =>
        {
            MongoClient mongoClient = new MongoClient(mongoDbConn);
            IMongoDatabase mongoDatabase = mongoClient.GetDatabase("louismanager");

            return mongoDatabase;
        });

        var mongoDbIdentityConfiguration = new MongoDbIdentityConfiguration
        {
            MongoDbSettings = new MongoDbSettings
            {
                ConnectionString = mongoDbConn,
                DatabaseName = "louismanager"
            },
            IdentityOptionsAction = options =>
            {
                // ApplicationUser settings
                options.User.RequireUniqueEmail = true;

                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;
            }
        };
        services.ConfigureMongoDbIdentity<ApplicationUser, MongoIdentityRole, Guid>(mongoDbIdentityConfiguration).AddDefaultTokenProviders();

        return services;
    }

    public static IServiceCollection ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAnyOrigin",
                builder => builder
                    .WithOrigins("http://localhost:4200")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials());
        });

        services.AddCors(options =>
        {
            options.AddPolicy("AllowScotexTech",
                builder => builder
                    .WithOrigins("https://louismanager.scotex.tech")
                    .AllowAnyHeader()
                    .WithMethods("GET", "POST", "PUT", "DELETE")
                    .AllowCredentials());
        });

        return services;
    }

    public static IServiceCollection SetupCookieAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<GoogleOptions>().Bind(configuration.GetSection("google"));
        services.AddHttpContextAccessor();
        services.AddScoped<IClaimReader, ClaimReader>();

        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.Cookie.Name = "AuthCookie";
                options.Cookie.SameSite = SameSiteMode.Strict;
                options.Cookie.Domain = configuration.GetValue<string>("Cookie:Domain");
                options.Cookie.HttpOnly = true;
                options.ExpireTimeSpan = TimeSpan.FromMinutes(20);

                if (configuration.GetValue<bool>("IsProduction"))
                {
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                }

                options.Events.OnRedirectToLogin = (context) =>
                {
                    context.Response.StatusCode = 401;
                    return Task.CompletedTask;
                };
            });

        return services;
    }

    public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<IGoogleClient, GoogleClient>(googleClient =>
        {
            googleClient.BaseAddress = new Uri("https://www.googleapis.com/oauth2/v1/");
        });

        return services;
    }
}