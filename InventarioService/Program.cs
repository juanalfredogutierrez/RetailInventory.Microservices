using BuildingBlocks;
using BuildingBlocks.Messaging.RabbiMQ;
using BuildingBlocks.Middleware;
using BuildingBlocks.Middleware.Correlation;
using BuildingBlocks.Observability;
using InventarioService.Application;
using InventarioService.Infrastructure.Messaging;
using InventarioService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
StartupConsoleExtensions.PrintStartupInfo(
    builder.Environment.ApplicationName,
    builder.Environment.EnvironmentName,
    builder.Configuration);

builder.Host.AddSerilogLogging(builder);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<InventarioDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.Services.AddHostedService<RabbitMqConsumerWorker>();
builder.Services.AddApplication();
builder.Services.AddBuildingBlocks();
builder.Services.AddRabbitMq(builder.Configuration);



var app = builder.Build();

using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<InventarioDbContext>();

db.Database.Migrate();

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<CorrelationMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();

app.MapControllers();

app.Run();