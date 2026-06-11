
using BuildingBlocks.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using TransaccionService.Application.Commands.CreateCompra;
using TransaccionService.Domain.Events;
using TransaccionService.Infrastructure.Persistence;
using TransaccionService.Tests.Helpers;

namespace TransaccionService.Tests.Application;
public class CreateCompraHandlerTests
{
    private readonly TransaccionDbContext _context;
    private readonly Mock<IMessagePublisher> _publisherMock;
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
    private readonly Mock<ILogger<CreateCompraHandler>> _loggerMock;
    private readonly CreateCompraHandler _handler;

    public CreateCompraHandlerTests()
    {
        var options = new DbContextOptionsBuilder<TransaccionDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString())
        .Options;

        _context = new TransaccionDbContext(options);

        _publisherMock = new Mock<IMessagePublisher>();

        _loggerMock =
        new Mock<ILogger<CreateCompraHandler>>();

        var httpClient = new HttpClient(
       new FakeHttpMessageHandler(() =>
           new HttpResponseMessage(HttpStatusCode.OK)))
        {
            BaseAddress = new Uri("http://localhost")
        };

        _httpClientFactoryMock =
        new Mock<IHttpClientFactory>();

        _httpClientFactoryMock
        .Setup(x => x.CreateClient("ProductoApi"))
        .Returns(httpClient);

        _handler = new CreateCompraHandler(
        _context,
        _publisherMock.Object,
        _loggerMock.Object,
        _httpClientFactoryMock.Object);
    }


    [Fact]
    public async Task Handle_Should_Create_Compra_When_Request_Is_Valid()
    {
        var command = new CreateCompraCommand(
            new()
            {
            new DetalleCompraDto(1, 2, 100),
            new DetalleCompraDto(2, 1, 50)
            },
            "Compra de prueba");

        var result = await _handler.Handle(
            command,
            CancellationToken.None);

        Assert.True(result.IsSuccess);

        Assert.NotEqual(Guid.Empty, result.Value);

        var compra = await _context.Compras
            .Include(x => x.Detalles)
            .FirstOrDefaultAsync();

        Assert.NotNull(compra);

        Assert.Equal(250, compra.TotalCompra);

        Assert.Equal(2, compra.Detalles.Count);

        _publisherMock.Verify(
            x => x.PublishAsync(
                "compra.registrada",
                It.IsAny<CompraRegistradaEvent>()),
            Times.Once);
    }
}

