using DematecStock.Application.UseCases.InventoryLocation.DeleteInventoryLocation;
using DematecStock.Domain.Repositories;
using DematecStock.Domain.Repositories.InventoryLocation;
using DematecStock.Domain.Repositories.Product;
using DematecStock.Domain.Repositories.WarehouseLocations;
using DematecStock.Exception.ExceptionsBase;
using FluentAssertions;
using Moq;

namespace DematecStock.Tests.UseCases.InventoryLocation;

public class DeleteInventoryLocationUseCaseTests
{
    private readonly Mock<IInventoryLocationWriteOnlyRepository> _writeRepositoryMock;
    private readonly Mock<IInventoryLocationUpdateOnlyRepository> _updateRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IWarehouseLocationsReadOnlyRepository> _warehouseLocationRepoMock;
    private readonly Mock<IProductReadOnlyRepository> _productRepoMock;
    private readonly DeleteInventoryLocationUseCase _sut;

    public DeleteInventoryLocationUseCaseTests()
    {
        _writeRepositoryMock = new Mock<IInventoryLocationWriteOnlyRepository>();
        _updateRepositoryMock = new Mock<IInventoryLocationUpdateOnlyRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _warehouseLocationRepoMock = new Mock<IWarehouseLocationsReadOnlyRepository>();
        _productRepoMock = new Mock<IProductReadOnlyRepository>();

        _sut = new DeleteInventoryLocationUseCase(
            _writeRepositoryMock.Object,
            _updateRepositoryMock.Object,
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
    public async Task Execute_ZeroBalance_DeletesAndCommits()
    {
        SetupExists();
        var entity = CreateInventoryLocation(1, 10, 0m);
        _updateRepositoryMock.Setup(r => r.GetByKey(1, 10)).ReturnsAsync(entity);

        await _sut.Execute(1, 10);

        _writeRepositoryMock.Verify(r => r.DeleteTracked(entity), Times.Once);
        _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
    }

    [Fact]
    public async Task Execute_LocationNotFound_ThrowsNotFoundException()
    {
        SetupExists(locationExists: false);

        var act = async () => await _sut.Execute(99, 10);

        await act.Should().ThrowAsync<NotFoundException>()
            .Where(e => e.StatusCode == 404);
    }

    [Fact]
    public async Task Execute_ProductNotFound_ThrowsNotFoundException()
    {
        SetupExists(locationExists: true, productExists: false);

        var act = async () => await _sut.Execute(1, 99);

        await act.Should().ThrowAsync<NotFoundException>()
            .Where(e => e.StatusCode == 404);
    }

    [Fact]
    public async Task Execute_RecordNotFound_ThrowsNotFoundException()
    {
        SetupExists();
        _updateRepositoryMock.Setup(r => r.GetByKey(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync((Domain.Entities.InventoryLocation?)null);

        var act = async () => await _sut.Execute(99, 99);

        await act.Should().ThrowAsync<NotFoundException>()
            .Where(e => e.StatusCode == 404);
    }

    [Fact]
    public async Task Execute_NonZeroBalance_ThrowsErrorOnValidationException()
    {
        SetupExists();
        var entity = CreateInventoryLocation(1, 10, 50m);
        _updateRepositoryMock.Setup(r => r.GetByKey(1, 10)).ReturnsAsync(entity);

        var act = async () => await _sut.Execute(1, 10);

        await act.Should().ThrowAsync<ErrorOnValidationException>()
            .Where(e => e.StatusCode == 400);
    }

    [Fact]
    public async Task Execute_NegativeBalance_ThrowsErrorOnValidationException()
    {
        SetupExists();
        var entity = CreateInventoryLocation(1, 10, -1m);
        _updateRepositoryMock.Setup(r => r.GetByKey(1, 10)).ReturnsAsync(entity);

        var act = async () => await _sut.Execute(1, 10);

        await act.Should().ThrowAsync<ErrorOnValidationException>();
    }

    [Fact]
    public async Task Execute_RecordNotFound_DeleteNeverCalled()
    {
        SetupExists();
        _updateRepositoryMock.Setup(r => r.GetByKey(It.IsAny<int>(), It.IsAny<int>()))
            .ReturnsAsync((Domain.Entities.InventoryLocation?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.Execute(1, 1));

        _writeRepositoryMock.Verify(r => r.DeleteTracked(It.IsAny<Domain.Entities.InventoryLocation>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.Commit(), Times.Never);
    }

    [Fact]
    public async Task Execute_ZeroBalance_GetByKeyCalledOnlyOnce()
    {
        SetupExists();
        var entity = CreateInventoryLocation(1, 10, 0m);
        _updateRepositoryMock.Setup(r => r.GetByKey(1, 10)).ReturnsAsync(entity);

        await _sut.Execute(1, 10);

        _updateRepositoryMock.Verify(r => r.GetByKey(1, 10), Times.Once);
    }
}
