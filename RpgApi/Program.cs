using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.Yaml;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Rpg;
using System.Text;
using System.Diagnostics;

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

var app = builder.Build();
app.Logger.LogDebug("app_started");

if (app.Environment.IsDevelopment())
{
    app.Logger.LogInformation("swagger_started");
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.UseAuthentication();
app.MapGrpcService<Rpg.Services.MathGrpcService>();

app.MapGet("/", () => "Hello World!");

app.MapPost("/game", PostGame);
app.MapGet("/game", GetGame);

app.MapGet("/game/player", GetPlayer);

app.Run();

async Task<IResult> PostGame(UserDbContext dbCtx, AuthService authSvc)
{
    var player = await dbCtx.MakePlayer();
    var tokenStr = authSvc.CreateToken(player.Id);
    return Results.Text(tokenStr);
}

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "PLAYER")]
async Task<IResult> GetGame(HttpContext httpCtx, UserDbContext dbCtx, AuthService authSvc)
{
    var player = await authSvc.GetPlayer(httpCtx, dbCtx);
    if (player == null) return Results.NotFound("PLAYER_NOT_FOUND");
    return Results.Ok($"Hello, Player({player.Id})");
}

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "PLAYER")]
async Task<IResult> GetPlayer(HttpContext httpCtx, UserDbContext dbCtx, AuthService authSvc)
{
    var player = await authSvc.GetPlayer(httpCtx, dbCtx);
    if (player == null) return Results.NotFound("PLAYER_NOT_FOUND");
    return Results.Ok(player);
}
