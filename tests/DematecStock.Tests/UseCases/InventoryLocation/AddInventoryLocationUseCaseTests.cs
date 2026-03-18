using DematecStock.Application.UseCases.InventoryLocation.AddInventoryLocation;
using DematecStock.Communication.Requests.InventoryLocation;
using DematecStock.Domain.Repositories;
using DematecStock.Domain.Repositories.InventoryLocation;
using DematecStock.Domain.Repositories.Product;
using DematecStock.Domain.Repositories.WarehouseLocations;
using DematecStock.Exception.ExceptionsBase;
using FluentAssertions;
using Moq;

namespace DematecStock.Tests.UseCases.InventoryLocation;

public class AddInventoryLocationUseCaseTests
{
    private readonly Mock<IInventoryLocationWriteOnlyRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IWarehouseLocationsReadOnlyRepository> _warehouseLocationRepoMock;
    private readonly Mock<IProductReadOnlyRepository> _productRepoMock;
    private readonly AddInventoryLocationUseCase _sut;

    public AddInventoryLocationUseCaseTests()
    {
        _repositoryMock = new Mock<IInventoryLocationWriteOnlyRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _warehouseLocationRepoMock = new Mock<IWarehouseLocationsReadOnlyRepository>();
        _productRepoMock = new Mock<IProductReadOnlyRepository>();

        _sut = new AddInventoryLocationUseCase(
            _repositoryMock.Object,
            MapperFactory.Create(),
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

    [Fact]
    public async Task Execute_ZeroQuantity_AddsAndCommits()
    {
        SetupExists();
        var request = new RequestAddInventoryLocationJson
        {
            IdLocation = 1, IdProduct = 10, Reference = "REF01",
            OnHandQuantity = 0, CreatedDate = DateTime.Today
        };

        await _sut.Execute(request);

        _repositoryMock.Verify(r => r.Add(It.IsAny<Domain.Entities.InventoryLocation>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
    }

    [Fact]
    public async Task Execute_PositiveQuantity_AddsAndCommits()
    {
        SetupExists();
        var request = new RequestAddInventoryLocationJson
        {
            IdLocation = 1, IdProduct = 10, Reference = "REF01",
            OnHandQuantity = 100.50m, CreatedDate = DateTime.Today
        };

        await _sut.Execute(request);

        _repositoryMock.Verify(r => r.Add(It.IsAny<Domain.Entities.InventoryLocation>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
    }

    [Fact]
    public async Task Execute_NegativeQuantity_ThrowsErrorOnValidationException()
    {
        var request = new RequestAddInventoryLocationJson
        {
            IdLocation = 1, IdProduct = 10, OnHandQuantity = -1
        };

        var act = async () => await _sut.Execute(request);

        await act.Should().ThrowAsync<ErrorOnValidationException>()
            .Where(e => e.StatusCode == 400);
    }

    [Fact]
    public async Task Execute_NegativeQuantity_ErrorMessageIsDescriptive()
    {
        var request = new RequestAddInventoryLocationJson
        {
            IdLocation = 1, IdProduct = 10, OnHandQuantity = -0.01m
        };

        var exception = await Assert.ThrowsAsync<ErrorOnValidationException>(
            () => _sut.Execute(request));

        exception.GetErrors().Should().ContainSingle()
            .Which.Should().Contain("negativo");
    }

    [Fact]
    public async Task Execute_NegativeQuantity_RepositoryNeverCalled()
    {
        var request = new RequestAddInventoryLocationJson
        {
            IdLocation = 1, IdProduct = 10, OnHandQuantity = -5
        };

        await Assert.ThrowsAsync<ErrorOnValidationException>(() => _sut.Execute(request));

        _repositoryMock.Verify(r => r.Add(It.IsAny<Domain.Entities.InventoryLocation>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.Commit(), Times.Never);
    }

    [Fact]
    public async Task Execute_LocationNotFound_ThrowsNotFoundException()
    {
        SetupExists(locationExists: false);
        var request = new RequestAddInventoryLocationJson
        {
            IdLocation = 99, IdProduct = 10, OnHandQuantity = 0
        };

        var act = async () => await _sut.Execute(request);

        await act.Should().ThrowAsync<NotFoundException>()
            .Where(e => e.StatusCode == 404);
    }

    [Fact]
    public async Task Execute_ProductNotFound_ThrowsNotFoundException()
    {
        SetupExists(locationExists: true, productExists: false);
        var request = new RequestAddInventoryLocationJson
        {
            IdLocation = 1, IdProduct = 99, OnHandQuantity = 0
        };

        var act = async () => await _sut.Execute(request);

        await act.Should().ThrowAsync<NotFoundException>()
            .Where(e => e.StatusCode == 404);
    }
}
