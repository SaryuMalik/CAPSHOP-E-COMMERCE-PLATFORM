using MMLib.SwaggerForOcelot.DependencyInjection;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

// ✅ Swagger for Ocelot
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerForOcelot(builder.Configuration);
builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();

app.UseCors("AllowAngular");

// ✅ Swagger UI Gateway pe
app.UseSwaggerForOcelotUI(options =>
{
    options.PathToSwaggerGenerator = "/swagger/docs";
});

await app.UseOcelot();
app.Run();
