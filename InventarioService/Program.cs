using Microsoft.EntityFrameworkCore;
using InventarioService.Infrastructure.Persistence;
using InventarioService.Infrastructure.Messaging;
using BuildingBlocks.Middleware;
using BuildingBlocks.Middleware.Correlation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<InventarioDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.Services.AddHostedService<RabbitMqConsumerWorker>();

var app = builder.Build();

app.UseMiddleware<CorrelationMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();

app.MapControllers();

app.Run();