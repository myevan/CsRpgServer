using Microsoft.Extensions.Configuration.Yaml;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddYamlFile("AppSettings.yaml");

var app = builder.Build();
app.Logger.LogDebug("start");

app.MapGet("/", () => "Hello World!");

app.Run();
