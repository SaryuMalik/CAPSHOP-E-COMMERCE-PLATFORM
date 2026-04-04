using CapShop.NotificationService;

var builder = Host.CreateApplicationBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", optional: false);
builder.Services.AddHostedService<NotificationWorker>();

var host = builder.Build();
host.Run();