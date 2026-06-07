using BuildingBlocks;
using BuildingBlocks.Middleware;
using BuildingBlocks.Middleware.Correlation;
using InventarioService.Application;
using InventarioService.Infrastructure.Messaging;
using InventarioService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

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

var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<CorrelationMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();

app.MapControllers();

app.Run();