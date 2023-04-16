using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.Yaml;
using Rpg;
using Rpg.Configs;
using Rpg.Helpers;
using Rpg.Services;
using Rpg.Examples;
using Rpg.DbContexts;
using Microsoft.Extensions.Caching.Distributed;
using Rpg.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddYamlFile("AppSettings.yaml");

var jwtAuthCfg = ConfigHelper.Create<JwtAuthConfig>(builder.Configuration, "Jwt:");
builder.Services.AddJwtService(jwtAuthCfg);

builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
builder.Services.AddDbContext<AuthDbContext>(opts => opts.UseInMemoryDatabase("AuthDb"));
builder.Services.AddDbContext<WorldDbContext>(opts => opts.UseInMemoryDatabase("WorldDb"));
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<WorldService>();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();

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
app.MapGrpcService<AuthGrpcService>();
app.MapGrpcService<WorldGrpcService>();

app.MapGet("/", 
    () => 
        "Welcome to Rpg!");

app.MapPost("/auth", 
    async (string name, AuthService authSvc, JwtService jwtSvc) => 
        Results.Text(jwtSvc.DumpSession(await authSvc.SignUpAsync(name))));

app.MapGet("/auth",
    (HttpContext httpCtx, JwtService jwtSvc) =>
    {
        var ses = jwtSvc.LoadSession(httpCtx);
        if (ses == null) return Results.NotFound();
        return Results.Ok(ses.ToDict());
    });

app.MapPost("/world/player",
    async (HttpContext httpCtx, WorldService worldSvc) =>
        Results.Ok(await worldSvc.ConnectPlayerAsync(httpCtx)));

app.MapGet("/world/player",
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "OPERATOR")]
    (WorldDbContext dbCtx) =>
        Results.Ok(dbCtx.PlayerSet.ToList()));

app.MapGet("/world/player/{id}",
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "OPERATOR")]
    (int id, WorldDbContext dbCtx) =>
        Results.Ok(dbCtx.PlayerSet.Find(id)));

//GameServiceExample.Run();

AesExample.Run();

app.Run();