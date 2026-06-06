using BuildingBlocks.Middleware;
using BuildingBlocks.Middleware.Correlation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using TransaccionService.Infrastructure.Messaging;
using TransaccionService.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

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


builder.Services.AddSingleton<RabbitMqPublisher>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<TransaccionDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.Services.AddSingleton<RabbitMqPublisher>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<CorrelationMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();
app.MapControllers();

app.Run();