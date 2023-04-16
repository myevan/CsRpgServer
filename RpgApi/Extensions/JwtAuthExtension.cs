using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Rpg.Configs;
using Rpg.Helpers;
using Rpg.Services;
using System.Text;

namespace Rpg.Extensions
{
    public static class JwtAuthExtension
    {
        public static void AddJwtService(this IServiceCollection serviceColleciton, JwtAuthConfig cfg)
        {
            serviceColleciton.AddEndpointsApiExplorer();
            serviceColleciton.AddSwaggerGen(opts =>
            {
                opts.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Name = "Authorization",
                    Description = "Bearer Authentication with JWT Token",
                    Type = SecuritySchemeType.Http
                });

                opts.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Id = "Bearer", Type = ReferenceType.SecurityScheme }
                        },
                        new List<string>()
                    }
                });
            });

            serviceColleciton.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opts =>
            {
                opts.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateActor = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = cfg.Issuer,
                    ValidAudience = cfg.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(cfg.Key))
                };
            });

            serviceColleciton.AddAuthorization();
            serviceColleciton.AddSingleton<JwtAuthConfig>(provider => cfg);
            serviceColleciton.AddScoped<JwtService>();

        }
    }
}
