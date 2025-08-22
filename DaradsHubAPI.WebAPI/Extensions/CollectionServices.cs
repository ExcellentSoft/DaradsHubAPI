using DaradsHubAPI.Domain.Entities;
using DaradsHubAPI.Infrastructure;
using DaradsHubAPI.Shared.Concrete;
using DaradsHubAPI.Shared.Interface;
using DaradsHubAPI.Shared.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SendGrid.Extensions.DependencyInjection;
using System.Reflection;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;

namespace DaradsWebMobAPIs.WebAPI.Extensions;

public static class CollectionServices
{
    public static IServiceCollection AddWebServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddIdentityService(config);
        services.AddAuthorization(options =>
        {
            var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder()
            .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
            .RequireAuthenticatedUser()
            .Build();
        });
        return services;
    }

    static void AddIdentityService(this IServiceCollection services, IConfiguration configuration)
    {
        var appConnectionSection = configuration.GetConnectionString("mycon");
        var appSettingsSection = configuration.GetSection("AppSettings");
        var appSettings = appSettingsSection.Get<AppSettings>();
        var jt = appSettings!.SendGridKey;
        services.AddSendGrid(options => options.ApiKey = jt);

        services.AddDbContext<AuthDataContext>(opt =>
        {
            opt.UseSqlServer(appConnectionSection);

        });

        services.AddIdentityCore<User>(options => { });
        services.AddScoped<IFileService, FileService>();
        services.AddIdentity<User, IdentityRole>(option =>
        {
            option.Password.RequireDigit = false;
            option.Password.RequiredLength = 6;
            option.Password.RequireLowercase = false;
            option.Password.RequireNonAlphanumeric = false;
            option.Password.RequireUppercase = false;
        })
        .AddEntityFrameworkStores<AuthDataContext>()
        .AddDefaultTokenProviders();


        var key = Encoding.ASCII.GetBytes(appSettings!.JwtKey);
        services.AddAuthentication(x =>
        {
            x.DefaultScheme = IdentityConstants.ApplicationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(x =>
        {
            x.RequireHttpsMetadata = false;
            x.SaveToken = true;
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false
            };
            x.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                    {
                        context.Response.Headers.Append("Token-Expired", "true");
                    }
                    return Task.CompletedTask;
                }
            };
        });
        services.AddDbContext<AppDbContext>(option => option.UseSqlServer(appConnectionSection));
        services.Configure<AppSettings>(configuration.GetSection(nameof(AppSettings)));
        services.Configure<CloudSettings>(configuration.GetSection(nameof(CloudSettings)));
        services.RegisterSwagger();
    }
    public static IServiceCollection RegisterSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "DaradsHubAPI",
                Version = "1.0"
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid bearer token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = JwtBearerDefaults.AuthenticationScheme
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        In = ParameterLocation.Header
                    },
                    Array.Empty<string>()
                }
                });
            // Set the comments path for the Swagger JSON and UI.

            var xmlFile = $"{Assembly.GetEntryAssembly()?.GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath);
        });
        return services;
    }
}

public static class IdentityUtil
{
    public static string GetUserId(this IIdentity identity) => GetClaimValue(identity, "id");
    public static string GetUserIdentityId(this IIdentity identity) => GetClaimValue(identity, "userId");
    public static string GetUserEmail(this IIdentity identity) => GetClaimValue(identity, "userEmail");
    public static int GetWalletId(this IIdentity identity)
    {
        var result = int.TryParse(GetClaimValue(identity, "walletId"), out int walletId);
        return result ? walletId : 0;
    }

    private static string GetClaimValue(IIdentity identity, string claimType)
    {
        var claimIdentity = (ClaimsIdentity)identity;
        return claimIdentity.Claims.GetClaimValue(claimType);
    }
    private static string GetClaimValue(this IEnumerable<Claim> claims, string claimType)
    {
        var claimsList = new List<Claim>(claims);
        var claim = claimsList.Find(c => c.Type == claimType);
        return claim?.Value ?? "";
    }
}