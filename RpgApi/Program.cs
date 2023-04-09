using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.Yaml;
using Rpg;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddYamlFile("AppSettings.yaml");
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<UserDbContext>(opts => opts.UseInMemoryDatabase("UserDb"));

var app = builder.Build();
app.Logger.LogDebug("app_started");

if (app.Environment.IsDevelopment())
{
    app.Logger.LogInformation("swagger_started");
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => "Hello World!");
app.MapGet("/player/{id}", async (int id, UserDbContext ctx) => await ctx.FindPlayer(id));
app.MapPost("/player", async (UserDbContext ctx) => await ctx.MakePlayer());

app.Run();
