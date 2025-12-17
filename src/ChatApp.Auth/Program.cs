using ChatApp.Auth;

var builder = WebApplication.CreateBuilder(args);

await builder.ConfigureAppServices();

var app = builder.Build();

await app.Configure();

app.Run();
