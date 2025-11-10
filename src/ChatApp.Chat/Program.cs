using ChatApp.Chat;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

builder.ConfigureAppServices();

app.MapGet("/", () => "Hello World!");

app.Run();
