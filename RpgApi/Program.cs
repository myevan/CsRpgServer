using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.Yaml;
using Rpg;
using Rpg.Services;
using Rpg.Helpers;
using Rpg.Examples;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddYamlFile("AppSettings.yaml");

builder.Services.AddDbContext<UserDbContext>(opts => opts.UseInMemoryDatabase("UserDb"));

builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
builder.Services.AddScoped<GameService>();

JwtAuthHelper.InitializeBuilder(builder);

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
app.MapGrpcService<GameGrpcService>();

app.MapGet("/", 
    () => 
        "Welcome to Rpg!");

app.MapPost("/auth", 
    (string guid, JwtAuthService authSvc) => 
        Results.Text(authSvc.CreateRoleToken("OPERATOR")));

app.MapGet("/world/player",
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "OPERATOR")]
    (UserDbContext dbCtx) =>
        Results.Ok(dbCtx.PlayerSet.ToList()));

app.MapGet("/world/player/{id}",
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "OPERATOR")]
    (int id, UserDbContext dbCtx) =>
        Results.Ok(dbCtx.PlayerSet.Find(id)));

GameServiceExample.Run();

app.Run();