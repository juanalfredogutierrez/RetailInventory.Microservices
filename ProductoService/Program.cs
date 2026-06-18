using BuildingBlocks.Middleware;
using BuildingBlocks.Middleware.Correlation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductoService.Application;
using ProductoService.Infrastructure.Persistence;
using ProductoService.Infrastructure.Seeders;

var builder = WebApplication.CreateBuilder(args);

StartupConsoleExtensions.PrintStartupInfo(
    builder.Environment.ApplicationName,
    builder.Environment.EnvironmentName,
    builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplication();

builder.Services.AddDbContext<ProductoDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

var app = builder.Build();
using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<ProductoDbContext>();

db.Database.Migrate();

await ProductoDbSeeder.SeedAsync(db);
app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<CorrelationMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();

app.MapControllers();

app.Run();