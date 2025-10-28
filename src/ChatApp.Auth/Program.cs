using ChatApp.Auth;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureAppServices();

var app = builder.Build();

await app.Configure();

app.Run();
