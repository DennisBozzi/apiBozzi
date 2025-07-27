using apiBozzi.Context;
using apiBozzi.Services;
using apiBozzi.Services.FelicianoBozzi;
using apiBozzi.Services.Firebase;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace apiBozzi.Configurations;

public static class ServiceConfiguration
{
    public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient<string>();

        var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
        var firebaseValidIssuer = Environment.GetEnvironmentVariable("FIREBASE_VALID_ISSUER");
        var firebaseAudience = Environment.GetEnvironmentVariable("FIREBASE_AUDIENCE");
        var firebaseCredentials = Environment.GetEnvironmentVariable("FIREBASE_CREDENTIALS");

        FirebaseApp.Create(new AppOptions
        {
            Credential = GoogleCredential.FromJson(firebaseCredentials)
        });

        var baseType = typeof(ServiceBase);
        var serviceTypes = baseType.Assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(baseType))
            .ToArray();

        foreach (var type in serviceTypes)
            services.AddScoped(type);

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddCors(options =>
        {
            options.AddPolicy("AllowAllOrigins",
                builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
        });

        services.AddAuthentication().AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            options.Authority = firebaseValidIssuer;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidIssuer = firebaseValidIssuer,
                ValidAudience = firebaseAudience,
            };
        });

        services.AddHttpClient();

        services.AddControllers();

        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Por favor, insira o token JWT com o prefixo 'Bearer '",
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    []
                }
            });
        });
    }
}