using CapShop.NotificationService;

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false)
    .AddEnvironmentVariables();
builder.Services.AddHostedService<NotificationWorker>();

var host = builder.Build();
host.Run();