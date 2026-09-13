using SmartHomeBackend;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

builder.Services.AddHostedService<Worker>();


var app = builder.Build();
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapHub<SmartHomeHub>("/smart-home");

app.Run();