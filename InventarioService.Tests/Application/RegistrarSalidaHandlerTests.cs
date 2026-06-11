using FluentAssertions;
using InventarioService.Application.Commands.RegistrarSalida;
using InventarioService.Domain.Entities;
using InventarioService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace InventarioService.Tests.Application.Inventario;

public class RegistrarSalidaHandlerTests
{
    [Fact]
    public async Task Handle_Should_Decrease_Stock_When_Stock_Is_Sufficient()
    {
        // Arrange

        var options = new DbContextOptionsBuilder<InventarioDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new InventarioDbContext(options);

        context.Existencias.Add(new ExistenciaProducto
        {
            ProductoId = 1,
            CantidadDisponible = 10,
            FechaActualizacion = DateTime.Now,
            CreatedBy = "system"
        });

        await context.SaveChangesAsync();

        var logger = Mock.Of<ILogger<RegistrarSalidaHandler>>();

        var handler = new RegistrarSalidaHandler(
            context,
            logger);

        var command = new RegistrarSalidaCommand(
            ProductoId: 1,
            Cantidad: 4);

        // Act

        var result = await handler.Handle(
            command,
            CancellationToken.None);

        // Assert

        result.Should().BeTrue();

        var stock = await context.Existencias.FirstAsync();

        stock.CantidadDisponible.Should().Be(6);
    }


    [Fact]
    public async Task Handle_Should_Throw_When_Stock_Does_Not_Exist()
    {
        // Arrange

        var options = new DbContextOptionsBuilder<InventarioDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new InventarioDbContext(options);

        var logger = Mock.Of<ILogger<RegistrarSalidaHandler>>();

        var handler = new RegistrarSalidaHandler(
            context,
            logger);

        var command = new RegistrarSalidaCommand(
            ProductoId: 1,
            Cantidad: 5);

        // Act

        Func<Task> act = async () =>
            await handler.Handle(command, CancellationToken.None);

        // Assert

        await act.Should()
            .ThrowAsync<Exception>()
            .WithMessage("Stock insuficiente");
    }

    [Fact]
    public async Task Handle_Should_Throw_When_Stock_Is_Insufficient()
    {
        // Arrange

        var options = new DbContextOptionsBuilder<InventarioDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        await using var context = new InventarioDbContext(options);

        context.Existencias.Add(new ExistenciaProducto
        {
            ProductoId = 1,
            CantidadDisponible = 5,
            FechaActualizacion = DateTime.Now,
            CreatedBy = "system"
        });

        await context.SaveChangesAsync();

        var logger = Mock.Of<ILogger<RegistrarSalidaHandler>>();

        var handler = new RegistrarSalidaHandler(
            context,
            logger);

        var command = new RegistrarSalidaCommand(
            ProductoId: 1,
            Cantidad: 10);

        // Act

        Func<Task> act = async () =>
            await handler.Handle(command, CancellationToken.None);

        // Assert

        await act.Should()
            .ThrowAsync<Exception>()
            .WithMessage("Stock insuficiente");
    }
}