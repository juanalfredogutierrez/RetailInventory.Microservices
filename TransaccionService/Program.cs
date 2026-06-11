using BuildingBlocks;
using BuildingBlocks.Messaging;
using BuildingBlocks.Middleware;
using BuildingBlocks.Middleware.Correlation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using TransaccionService.Application;
using TransaccionService.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient("InventarioApi", client =>
{
    client.BaseAddress = new Uri("http://localhost:5002/");
});

builder.Services.AddHttpClient("ProductoApi",
    client =>
    {
        client.BaseAddress = new Uri(
            builder.Configuration["Services:ProductoApi"]!);
    });

builder.Services.AddRabbitMq( builder.Configuration["RabbitMq:Host"]!);

builder.Services.AddSingleton<IConnection>(sp =>
{
    var factory = new ConnectionFactory
    {
        HostName = "localhost",
        UserName = "guest",
        Password = "guest"
    };

    return factory.CreateConnectionAsync().GetAwaiter().GetResult();
});

builder.Services.AddSingleton<IChannel>(sp =>
{
    var connection = sp.GetRequiredService<IConnection>();

    return connection.CreateChannelAsync().GetAwaiter().GetResult();
});


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();
builder.Services.AddBuildingBlocks();

builder.Services.AddDbContext<TransaccionDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));


var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<CorrelationMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();
app.MapControllers();

app.Run();