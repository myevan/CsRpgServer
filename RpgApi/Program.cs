using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.Yaml;
using Rpg;
using Rpg.Configs;
using Rpg.Helpers;
using Rpg.Services;
using Rpg.Examples;
using Rpg.DbContexts;
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
builder.Services.AddScoped<AuthRestService>();
builder.Services.AddScoped<WorldRestService>();
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
    async (string name, AuthRestService svc) => await svc.PostAuthAsync(name));

app.MapGet("/auth",
    (HttpContext ctx, AuthRestService svc) => svc.GetAuth(ctx));

app.MapPost("/world/player",
    async (HttpContext ctx, WorldRestService svc) => await svc.PostWorldPlayerAsync(ctx));

app.MapGet("/world/player",
    async (HttpContext ctx, WorldRestService svc) => await svc.GetWorldPlayerAsync(ctx));

app.MapGet("/world/player/{id}",
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "OPERATOR")]
    async (int id, WorldRestService svc) => await svc.GetWorldPlayerAsync(id));

app.MapGet("/world/players",
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "OPERATOR")]
    async (WorldRestService svc) => await svc.GetWorldPlayersAsync());


//GameServiceExample.Run();
//AesExample.Run();

app.Run();