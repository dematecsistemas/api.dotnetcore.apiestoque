using DematecStock.Application.UseCases.InventoryLocation.UpdateOnHandQuantity;
using DematecStock.Communication.Requests.InventoryLocation;
using DematecStock.Domain.Repositories;
using DematecStock.Domain.Repositories.InventoryLocation;
using DematecStock.Domain.Repositories.Product;
using DematecStock.Domain.Repositories.WarehouseLocations;
using DematecStock.Exception.ExceptionsBase;
using FluentAssertions;
using Moq;

namespace DematecStock.Tests.UseCases.InventoryLocation;

public class UpdateOnHandQuantityUseCaseTests
{
    private readonly Mock<IInventoryLocationUpdateOnlyRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IWarehouseLocationsReadOnlyRepository> _warehouseLocationRepoMock;
    private readonly Mock<IProductReadOnlyRepository> _productRepoMock;
    private readonly UpdateOnHandQuantityUseCase _sut;

    public UpdateOnHandQuantityUseCaseTests()
    {
        _repositoryMock = new Mock<IInventoryLocationUpdateOnlyRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _warehouseLocationRepoMock = new Mock<IWarehouseLocationsReadOnlyRepository>();
        _productRepoMock = new Mock<IProductReadOnlyRepository>();

        _sut = new UpdateOnHandQuantityUseCase(
            _repositoryMock.Object,
            _unitOfWorkMock.Object,
            _warehouseLocationRepoMock.Object,
            _productRepoMock.Object);
    }

    private void SetupExists(bool locationExists = true, bool productExists = true)
    {
        _warehouseLocationRepoMock
            .Setup(r => r.ExistsById(It.IsAny<int>()))
            .ReturnsAsync(locationExists);
        _productRepoMock
            .Setup(r => r.ExistsAsync(It.IsAny<int>()))
            .ReturnsAsync(productExists);
    }

    private static Domain.Entities.InventoryLocation CreateInventoryLocation(int idLocation, int idProduct, decimal quantity)
    {
        var entity = new Domain.Entities.InventoryLocation
        {
            IdLocation = idLocation,
            IdProduct = idProduct,
            Reference = "REF"
        };
        entity.UpdateOnHandQuantity(quantity);
        return entity;
    }

    [Fact]
    public async Task Execute_ValidQuantity_UpdatesAndCommits()
    {
        SetupExists();
        var entity = CreateInventoryLocation(1, 10, 0m);
        _repositoryMock.Setup(r => r.GetByKey(1, 10)).ReturnsAsync(entity);

        await _sut.Execute(1, 10, new RequestUpdateOnHandQuantityJson { OnHandQuantity = 150m });

        entity.OnHandQuantity.Should().Be(150m);
        _repositoryMock.Verify(r => r.Update(entity), Times.Once);
        _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
    }

    [Fact]
    public async Task Execute_ZeroQuantity_UpdatesToZero()
    {
        SetupExists();
        var entity = CreateInventoryLocation(1, 10, 100m);
        _repositoryMock.Setup(r => r.GetByKey(1, 10)).ReturnsAsync(entity);

        await _sut.Execute(1, 10, new RequestUpdateOnHandQuantityJson { OnHandQuantity = 0m });

        entity.OnHandQuantity.Should().Be(0m);
    }

    [Fact]
    public async Task Execute_NegativeQuantity_ThrowsErrorOnValidationExceptionWithoutDbCall()
    {
        var act = async () => await _sut.Execute(1, 10, new RequestUpdateOnHandQuantityJson { OnHandQuantity = -1m });

        await act.Should().ThrowAsync<ErrorOnValidationException>()
            .Where(e => e.StatusCode == 400);

        _repositoryMock.Verify(r => r.GetByKey(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Execute_LocationNotFound_ThrowsNotFoundException()
    {
        SetupExists(locationExists: false);

        var act = async () => await _sut.Execute(1, 10, new RequestUpdateOnHandQuantityJson { OnHandQuantity = 10m });

        await act.Should().ThrowAsync<NotFoundException>()
            .Where(e => e.StatusCode == 404);
    }

    [Fact]
    public async Task Execute_ProductNotFound_ThrowsNotFoundException()
    {
        SetupExists(locationExists: true, productExists: false);

        var act = async () => await _sut.Execute(1, 10, new RequestUpdateOnHandQuantityJson { OnHandQuantity = 10m });

        await act.Should().ThrowAsync<NotFoundException>()
            .Where(e => e.StatusCode == 404);
    }

    [Fact]
    public async Task Execute_RecordNotFound_ThrowsNotFoundException()
    {
        SetupExists();
        _repositoryMock.Setup(r => r.GetByKey(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync((Domain.Entities.InventoryLocation?)null);

        var act = async () => await _sut.Execute(1, 10, new RequestUpdateOnHandQuantityJson { OnHandQuantity = 10m });

        await act.Should().ThrowAsync<NotFoundException>()
            .Where(e => e.StatusCode == 404);
    }

    [Fact]
    public async Task Execute_RecordNotFound_UpdateAndCommitNeverCalled()
    {
        SetupExists();
        _repositoryMock.Setup(r => r.GetByKey(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync((Domain.Entities.InventoryLocation?)null);

        await Assert.ThrowsAsync<NotFoundException>(
            () => _sut.Execute(1, 10, new RequestUpdateOnHandQuantityJson { OnHandQuantity = 5m }));

        _repositoryMock.Verify(r => r.Update(It.IsAny<Domain.Entities.InventoryLocation>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.Commit(), Times.Never);
    }

    [Fact]
    public async Task Execute_HighPrecisionDecimal_UpdatesCorrectly()
    {
        SetupExists();
        var entity = CreateInventoryLocation(1, 10, 0m);
        _repositoryMock.Setup(r => r.GetByKey(1, 10)).ReturnsAsync(entity);

        await _sut.Execute(1, 10, new RequestUpdateOnHandQuantityJson { OnHandQuantity = 9999.9999m });

        entity.OnHandQuantity.Should().Be(9999.9999m);
    }
}
