using DematecStock.Application.UseCases.InventoryMovement;
using DematecStock.Communication.Requests.InventoryMovement;
using DematecStock.Domain.Repositories;
using DematecStock.Domain.Repositories.InventoryLocation;
using DematecStock.Domain.Repositories.InventoryMovement;
using DematecStock.Domain.Repositories.Product;
using DematecStock.Domain.Repositories.WarehouseLocations;
using DematecStock.Exception.ExceptionsBase;
using FluentAssertions;
using Moq;

namespace DematecStock.Tests.UseCases.InventoryMovement;

/// <summary>
/// Testes unitários para AddInventoryMovementsUseCase.
/// Cobre: sucesso (E/S), atualização de OnHandQuantity, MovementDate automático,
/// validações de campo, existência de endereço/produto e saldo insuficiente.
/// </summary>
public class AddInventoryMovementsUseCaseTests
{
    private readonly Mock<IInventoryMovementsWriteOnlyRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IWarehouseLocationsReadOnlyRepository> _warehouseLocationRepoMock;
    private readonly Mock<IProductReadOnlyRepository> _productRepoMock;
    private readonly Mock<IInventoryLocationUpdateOnlyRepository> _inventoryLocationRepoMock;
    private readonly AddInventoryMovementsUseCase _sut;

    public AddInventoryMovementsUseCaseTests()
    {
        _repositoryMock = new Mock<IInventoryMovementsWriteOnlyRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _warehouseLocationRepoMock = new Mock<IWarehouseLocationsReadOnlyRepository>();
        _productRepoMock = new Mock<IProductReadOnlyRepository>();
        _inventoryLocationRepoMock = new Mock<IInventoryLocationUpdateOnlyRepository>();

        _sut = new AddInventoryMovementsUseCase(
            _repositoryMock.Object,
            MapperFactory.Create(),
            _unitOfWorkMock.Object,
            _warehouseLocationRepoMock.Object,
            _productRepoMock.Object,
            _inventoryLocationRepoMock.Object);
    }

    // ── helpers ───────────────────────────────────────────────────────────────

    private static RequestAddInventoryMovementJsons ValidRequest() => new()
    {
        IdProduct = 10,
        IdLocation = 5,
        IdUser = 1,
        Operation = "TRANSFERÊNCIA",
        MovementDirection = 'E',
        Quantity = 100m,
        Remarks = "Teste"
    };

    private void SetupHappyPath(decimal currentBalance = 500m)
    {
        _warehouseLocationRepoMock
            .Setup(r => r.ExistsById(It.IsAny<int>()))
            .ReturnsAsync(true);
        _productRepoMock
            .Setup(r => r.ExistsAsync(It.IsAny<int>()))
            .ReturnsAsync(true);
        var location = CreateInventoryLocation(5, 10, currentBalance);
        _inventoryLocationRepoMock
            .Setup(r => r.GetByKey(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync(location);
    }

    private static Domain.Entities.InventoryLocation CreateInventoryLocation(int idLocation, int idProduct, decimal balance)
    {
        var entity = new Domain.Entities.InventoryLocation
        {
            IdLocation = idLocation,
            IdProduct = idProduct,
            Reference = "REF"
        };
        entity.UpdateOnHandQuantity(balance);
        return entity;
    }

    // ── testes de sucesso ─────────────────────────────────────────────────────

    /// <summary>Fluxo feliz E → persiste movimento e comita.</summary>
    [Fact]
    public async Task Execute_ValidEntrada_AddsAndCommits()
    {
        SetupHappyPath(currentBalance: 0m);

        await _sut.Execute(ValidRequest());

        _repositoryMock.Verify(r => r.Add(It.IsAny<Domain.Entities.InventoryMovements>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
    }

    /// <summary>Direção S com saldo suficiente → persiste sem erros.</summary>
    [Fact]
    public async Task Execute_ValidSaida_AddsAndCommits()
    {
        SetupHappyPath(currentBalance: 200m);
        var request = ValidRequest();
        request.MovementDirection = 'S';
        request.Quantity = 100m;

        await _sut.Execute(request);

        _repositoryMock.Verify(r => r.Add(It.IsAny<Domain.Entities.InventoryMovements>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
    }

    /// <summary>Entrada E → OnHandQuantity deve aumentar corretamente.</summary>
    [Fact]
    public async Task Execute_EntradaMovement_IncreasesOnHandQuantity()
    {
        var location = CreateInventoryLocation(5, 10, 50m);
        _warehouseLocationRepoMock.Setup(r => r.ExistsById(5)).ReturnsAsync(true);
        _productRepoMock.Setup(r => r.ExistsAsync(10)).ReturnsAsync(true);
        _inventoryLocationRepoMock.Setup(r => r.GetByKey(5, 10)).ReturnsAsync(location);

        var request = ValidRequest();
        request.MovementDirection = 'E';
        request.Quantity = 30m;

        await _sut.Execute(request);

        location.OnHandQuantity.Should().Be(80m);
        _inventoryLocationRepoMock.Verify(r => r.Update(location), Times.Once);
    }

    /// <summary>Saída S → OnHandQuantity deve diminuir corretamente.</summary>
    [Fact]
    public async Task Execute_SaidaMovement_DecreasesOnHandQuantity()
    {
        var location = CreateInventoryLocation(5, 10, 100m);
        _warehouseLocationRepoMock.Setup(r => r.ExistsById(5)).ReturnsAsync(true);
        _productRepoMock.Setup(r => r.ExistsAsync(10)).ReturnsAsync(true);
        _inventoryLocationRepoMock.Setup(r => r.GetByKey(5, 10)).ReturnsAsync(location);

        var request = ValidRequest();
        request.MovementDirection = 'S';
        request.Quantity = 40m;

        await _sut.Execute(request);

        location.OnHandQuantity.Should().Be(60m);
    }

    /// <summary>MovementDate é atribuída automaticamente pelo sistema.</summary>
    [Fact]
    public async Task Execute_MovementDate_IsSetAutomatically()
    {
        SetupHappyPath();
        var before = DateTimeOffset.UtcNow;

        await _sut.Execute(ValidRequest());

        _repositoryMock.Verify(
            r => r.Add(It.Is<Domain.Entities.InventoryMovements>(m => m.MovementDate >= before)),
            Times.Once);
    }

    /// <summary>Remarks nullable não causa erro.</summary>
    [Fact]
    public async Task Execute_NullRemarks_AddsAndCommits()
    {
        SetupHappyPath();
        var request = ValidRequest();
        request.Remarks = null;

        await _sut.Execute(request);

        _repositoryMock.Verify(r => r.Add(It.IsAny<Domain.Entities.InventoryMovements>()), Times.Once);
    }

    // ── validação: Quantity ───────────────────────────────────────────────────

    [Fact]
    public async Task Execute_ZeroQuantity_ThrowsValidationError()
    {
        var request = ValidRequest();
        request.Quantity = 0;

        var act = async () => await _sut.Execute(request);

        var ex = await act.Should().ThrowAsync<ErrorOnValidationException>();
        ex.Which.GetErrors().Should().ContainSingle(e => e.Contains("quantidade"));
    }

    [Fact]
    public async Task Execute_NegativeQuantity_ThrowsValidationError()
    {
        var request = ValidRequest();
        request.Quantity = -1m;

        var act = async () => await _sut.Execute(request);

        await act.Should().ThrowAsync<ErrorOnValidationException>();
    }

    // ── validação: MovementDirection ─────────────────────────────────────────

    [Theory]
    [InlineData('X')]
    [InlineData('e')]
    [InlineData('s')]
    [InlineData(' ')]
    public async Task Execute_InvalidMovementDirection_ThrowsValidationError(char direction)
    {
        var request = ValidRequest();
        request.MovementDirection = direction;

        var act = async () => await _sut.Execute(request);

        var ex = await act.Should().ThrowAsync<ErrorOnValidationException>();
        ex.Which.GetErrors().Should().ContainSingle(e => e.Contains("direção"));
    }

    // ── validação: Operation ─────────────────────────────────────────────────

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task Execute_EmptyOrNullOperation_ThrowsValidationError(string? operation)
    {
        var request = ValidRequest();
        request.Operation = operation!;

        var act = async () => await _sut.Execute(request);

        var ex = await act.Should().ThrowAsync<ErrorOnValidationException>();
        ex.Which.GetErrors().Should().ContainSingle(e => e.Contains("operação"));
    }

    // ── validação: IDs ────────────────────────────────────────────────────────

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Execute_InvalidIdProduct_ThrowsValidationError(int idProduct)
    {
        var request = ValidRequest();
        request.IdProduct = idProduct;

        var act = async () => await _sut.Execute(request);

        var ex = await act.Should().ThrowAsync<ErrorOnValidationException>();
        ex.Which.GetErrors().Should().ContainSingle(e => e.Contains("produto"));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public async Task Execute_InvalidIdLocation_ThrowsValidationError(int idLocation)
    {
        var request = ValidRequest();
        request.IdLocation = idLocation;

        var act = async () => await _sut.Execute(request);

        var ex = await act.Should().ThrowAsync<ErrorOnValidationException>();
        ex.Which.GetErrors().Should().ContainSingle(e => e.Contains("localização"));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-99)]
    public async Task Execute_InvalidIdUser_ThrowsValidationError(int idUser)
    {
        var request = ValidRequest();
        request.IdUser = idUser;

        var act = async () => await _sut.Execute(request);

        var ex = await act.Should().ThrowAsync<ErrorOnValidationException>();
        ex.Which.GetErrors().Should().ContainSingle(e => e.Contains("usuário"));
    }

    // ── validação de existência ───────────────────────────────────────────────

    /// <summary>Endereço não encontrado → NotFoundException (404).</summary>
    [Fact]
    public async Task Execute_LocationNotFound_ThrowsNotFoundException()
    {
        _warehouseLocationRepoMock.Setup(r => r.ExistsById(It.IsAny<int>())).ReturnsAsync(false);

        var act = async () => await _sut.Execute(ValidRequest());

        await act.Should().ThrowAsync<NotFoundException>()
            .Where(e => e.StatusCode == 404);
    }

    /// <summary>Produto não encontrado → NotFoundException (404).</summary>
    [Fact]
    public async Task Execute_ProductNotFound_ThrowsNotFoundException()
    {
        _warehouseLocationRepoMock.Setup(r => r.ExistsById(It.IsAny<int>())).ReturnsAsync(true);
        _productRepoMock.Setup(r => r.ExistsAsync(It.IsAny<int>())).ReturnsAsync(false);

        var act = async () => await _sut.Execute(ValidRequest());

        await act.Should().ThrowAsync<NotFoundException>()
            .Where(e => e.StatusCode == 404);
    }

    /// <summary>Produto não endereçado no local → NotFoundException (404).</summary>
    [Fact]
    public async Task Execute_InventoryLocationNotFound_ThrowsNotFoundException()
    {
        _warehouseLocationRepoMock.Setup(r => r.ExistsById(It.IsAny<int>())).ReturnsAsync(true);
        _productRepoMock.Setup(r => r.ExistsAsync(It.IsAny<int>())).ReturnsAsync(true);
        _inventoryLocationRepoMock
            .Setup(r => r.GetByKey(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync((Domain.Entities.InventoryLocation?)null);

        var act = async () => await _sut.Execute(ValidRequest());

        await act.Should().ThrowAsync<NotFoundException>()
            .Where(e => e.StatusCode == 404);
    }

    // ── validação de saldo ────────────────────────────────────────────────────

    /// <summary>Saída que tornaria o saldo negativo → 400 com mensagem da quantidade disponível.</summary>
    [Fact]
    public async Task Execute_SaidaWithInsufficientBalance_ThrowsValidationError()
    {
        var location = CreateInventoryLocation(5, 10, 10m);
        _warehouseLocationRepoMock.Setup(r => r.ExistsById(5)).ReturnsAsync(true);
        _productRepoMock.Setup(r => r.ExistsAsync(10)).ReturnsAsync(true);
        _inventoryLocationRepoMock.Setup(r => r.GetByKey(5, 10)).ReturnsAsync(location);

        var request = ValidRequest();
        request.MovementDirection = 'S';
        request.Quantity = 50m;

        var act = async () => await _sut.Execute(request);

        var ex = await act.Should().ThrowAsync<ErrorOnValidationException>();
        ex.Which.GetErrors().Should().ContainSingle(e => e.Contains("10"));
    }

    /// <summary>Saída exata do saldo disponível → sucesso, saldo fica em zero.</summary>
    [Fact]
    public async Task Execute_SaidaExactBalance_SucceedsWithZeroBalance()
    {
        var location = CreateInventoryLocation(5, 10, 100m);
        _warehouseLocationRepoMock.Setup(r => r.ExistsById(5)).ReturnsAsync(true);
        _productRepoMock.Setup(r => r.ExistsAsync(10)).ReturnsAsync(true);
        _inventoryLocationRepoMock.Setup(r => r.GetByKey(5, 10)).ReturnsAsync(location);

        var request = ValidRequest();
        request.MovementDirection = 'S';
        request.Quantity = 100m;

        await _sut.Execute(request);

        location.OnHandQuantity.Should().Be(0m);
    }

    /// <summary>Saldo insuficiente → Add e Commit NÃO são chamados.</summary>
    [Fact]
    public async Task Execute_SaidaWithInsufficientBalance_RepositoryNeverCalled()
    {
        var location = CreateInventoryLocation(5, 10, 5m);
        _warehouseLocationRepoMock.Setup(r => r.ExistsById(5)).ReturnsAsync(true);
        _productRepoMock.Setup(r => r.ExistsAsync(10)).ReturnsAsync(true);
        _inventoryLocationRepoMock.Setup(r => r.GetByKey(5, 10)).ReturnsAsync(location);

        var request = ValidRequest();
        request.MovementDirection = 'S';
        request.Quantity = 10m;

        await Assert.ThrowsAsync<ErrorOnValidationException>(() => _sut.Execute(request));

        _repositoryMock.Verify(r => r.Add(It.IsAny<Domain.Entities.InventoryMovements>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.Commit(), Times.Never);
    }

    // ── múltiplos erros ───────────────────────────────────────────────────────

    /// <summary>Vários campos inválidos → todos os erros acumulados e retornados de uma vez.</summary>
    [Fact]
    public async Task Execute_MultipleInvalidFields_AccumulatesAllErrors()
    {
        var request = new RequestAddInventoryMovementJsons
        {
            IdProduct = 0,
            IdLocation = 0,
            IdUser = 0,
            Operation = "",
            MovementDirection = 'X',
            Quantity = -1m
        };

        var ex = await Assert.ThrowsAsync<ErrorOnValidationException>(() => _sut.Execute(request));

        ex.GetErrors().Should().HaveCountGreaterThan(1);
    }

    /// <summary>Quando a validação de campo falha, o repositório NÃO é chamado.</summary>
    [Fact]
    public async Task Execute_ValidationFails_RepositoryNeverCalled()
    {
        var request = ValidRequest();
        request.Quantity = 0;

        await Assert.ThrowsAsync<ErrorOnValidationException>(() => _sut.Execute(request));

        _repositoryMock.Verify(r => r.Add(It.IsAny<Domain.Entities.InventoryMovements>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.Commit(), Times.Never);
    }
}
