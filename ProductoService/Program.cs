using BuildingBlocks;
using BuildingBlocks.Middleware;
using BuildingBlocks.Middleware.Correlation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductoService.Application;
using ProductoService.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplication();

builder.Services.AddDbContext<ProductoDbContext>(opt =>
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