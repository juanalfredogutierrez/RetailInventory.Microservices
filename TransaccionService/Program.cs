using BuildingBlocks;
using BuildingBlocks.Messaging.RabbiMQ;
using BuildingBlocks.Middleware;
using BuildingBlocks.Middleware.Correlation;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using TransaccionService.Application;
using TransaccionService.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

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

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<CorrelationMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();
app.MapControllers();

app.Run();