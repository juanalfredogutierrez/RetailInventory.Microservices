using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using System.Text;
using System.Text.Json;
using TransaccionService.Application.Commands.CreateVenta;
using TransaccionService.Application.DTOs;
using TransaccionService.Domain.Errors;
using TransaccionService.Infrastructure.Persistence;
using TransaccionService.Tests.Helpers;

namespace TransaccionService.Tests.Application;

public class CreateVentaHandlerTests
{
    private readonly TransaccionDbContext _context;
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
    private readonly Mock<ILogger<CreateVentaHandler>> _loggerMock;

    private readonly CreateVentaHandler _handler;

    public CreateVentaHandlerTests()
    {
        var options = new DbContextOptionsBuilder<TransaccionDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new TransaccionDbContext(options);
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();

        _loggerMock = new Mock<ILogger<CreateVentaHandler>>();

        var httpClient = new HttpClient(
            new FakeHttpMessageHandler(() =>
            {
                var stockResponse = new ApiResponse<int>
                {
                    Data = 100
                };

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(
                        JsonSerializer.Serialize(stockResponse),
                        Encoding.UTF8,
                        "application/json")
                };
            }))
        {
            BaseAddress = new Uri("http://localhost")
        };

        _httpClientFactoryMock
            .Setup(x => x.CreateClient("InventarioApi"))
            .Returns(httpClient);

        _handler = new CreateVentaHandler(
            _context,
            _httpClientFactoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Should_Return_Failure_When_Stock_Is_Insufficient()
    {
        // Arrange
        var httpClient = new HttpClient(
            new FakeHttpMessageHandler(() =>
            {
                var stockResponse = new ApiResponse<int>
                {
                    Data = 2
                };

                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(
                        JsonSerializer.Serialize(stockResponse),
                        Encoding.UTF8,
                        "application/json")
                };
            }))
        {
            BaseAddress = new Uri("http://localhost")
        };

        _httpClientFactoryMock
            .Setup(x => x.CreateClient("InventarioApi"))
            .Returns(httpClient);

        var handler = new CreateVentaHandler(
            _context,
            _httpClientFactoryMock.Object,
            _loggerMock.Object);

        var command = new CreateVentaCommand(
            new()
            {
            new DetalleVentaDto(1, 5, 100)
            },
            "Venta prueba");

        // Act
        var result = await handler.Handle(
            command,
            CancellationToken.None);

        // Assert
        result.IsFailure.Should().BeTrue();

        result.FirstError.Should().NotBeNull();

        result.FirstError!.Code.Should()
            .Be(VentaErrors.StockInsuficiente(1, 2, 5).Code);

        result.FirstError.Message.Should()
            .Be(VentaErrors.StockInsuficiente(1, 2, 5).Message);

        // No debe persistir la venta
        _context.Ventas.Should().BeEmpty();


    }
    public async Task Handle_Should_Create_Venta_When_Request_Is_Valid()
    {
        var command = new CreateVentaCommand(
        new()
        {
        new DetalleVentaDto(1, 2, 100),
        new DetalleVentaDto(2, 1, 50)
        },
        "Venta prueba");

        var result = await _handler.Handle(
        command,
        CancellationToken.None);

        Assert.True(result.IsSuccess);

        Assert.NotEqual(Guid.Empty, result.Value);

        var venta = await _context.Ventas
        .Include(x => x.Detalles)
        .FirstOrDefaultAsync();

        Assert.NotNull(venta);

        Assert.Equal(250, venta.TotalVenta);

        Assert.Equal(2, venta.Detalles.Count);

      
    }

}