using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.Yaml;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Rpg;
using Rpg.Services;
using System.Text;
using System.Diagnostics;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddYamlFile("AppSettings.yaml");

var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];
var jwtKeyStr = builder.Configuration["Jwt:Key"];
Debug.Assert(!string.IsNullOrEmpty(jwtIssuer));
Debug.Assert(!string.IsNullOrEmpty(jwtAudience));
Debug.Assert(!string.IsNullOrEmpty(jwtKeyStr));

builder.Services.AddDbContext<UserDbContext>(opts => opts.UseInMemoryDatabase("UserDb"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opts =>
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

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opts =>
{
    opts.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateActor = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKeyStr))
    };
});

builder.Services.AddAuthorization();
builder.Services.AddSingleton<AuthService>(provider =>
{
    return new AuthService(jwtIssuer, jwtAudience, jwtKeyStr);
});

builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();
builder.Services.AddAutoMapper(typeof(AutoMapperConfig));

var app = builder.Build();
app.Logger.LogDebug("app_started");

if (app.Environment.IsDevelopment())
{
    app.Logger.LogInformation("swagger_started");
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapGrpcReflectionService();
}

app.UseAuthorization();
app.UseAuthentication();
app.MapGrpcService<GameRpcService>();

app.MapGet("/", 
    () => 
        "Welcome to Rpg!");

app.MapPost("/auth", 
    (string guid, AuthService authSvc) => 
        Results.Text(authSvc.CreateRoleToken("OPERATOR")));

app.MapGet("/world/player/{id}",
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "OPERATOR")]
    (int id, UserDbContext dbCtx) =>
        Results.Ok(dbCtx.PlayerSet.Find(id)));

app.Run();