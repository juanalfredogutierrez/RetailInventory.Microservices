using AuthService.Application;
using AuthService.Domain.Entities;
using AuthService.Infrastructure.Persistence;
using AuthService.Infrastructure.Security;
using BuildingBlocks;
using BuildingBlocks.Middleware;
using BuildingBlocks.Middleware.Correlation;
using BuildingBlocks.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddApplication();
builder.Services.AddBuildingBlocks();

builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<PasswordHasher<Usuario>>();
builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection("Jwt"));

builder.Services.AddScoped<JwtTokenGenerator>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<CorrelationMiddleware>();
app.UseMiddleware<ExceptionMiddleware>();

app.MapControllers();

app.Run();