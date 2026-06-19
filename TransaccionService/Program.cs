using BuildingBlocks;
using BuildingBlocks.Messaging.RabbiMQ;
using BuildingBlocks.Middleware;
using BuildingBlocks.Middleware.Correlation;
using BuildingBlocks.Observability;
using Microsoft.EntityFrameworkCore;
using TransaccionService.Application;
using TransaccionService.Infrastructure.Messaging;
using TransaccionService.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);
StartupConsoleExtensions.PrintStartupInfo(
    builder.Environment.ApplicationName,
    builder.Environment.EnvironmentName,
    builder.Configuration);

builder.Host.AddSerilogLogging(builder);

builder.Configuration
.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json",
                        optional: true,reloadOnChange: true);

builder.Services.AddHttpClient("ProductoApi",
                        client =>
                        {
                            client.BaseAddress = new Uri(
                            builder.Configuration["Services:ProductoApi"]!);
                        });

builder.Services.AddHttpClient( "InventarioApi",
                    client =>
                    {
                        client.BaseAddress = new Uri(
                        builder.Configuration["Services:InventarioApi"]!);
                    });

builder.Services.AddRabbitMq(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();
builder.Services.AddBuildingBlocks();

builder.Services.AddDbContext<TransaccionDbContext>(opt =>
opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHostedService<OutboxPublisherWorker>();

var app = builder.Build();


using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<TransaccionDbContext>();

db.Database.Migrate();

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<CorrelationMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();
app.MapControllers();

app.Run();