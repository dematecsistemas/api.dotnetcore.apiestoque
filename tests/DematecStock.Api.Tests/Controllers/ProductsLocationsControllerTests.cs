using DematecStock.Api.Controllers;
using DematecStock.Application.UseCases.ProductsAddress.GetAllStorageLocationsByProduct;
using DematecStock.Application.UseCases.ProductsAddress.GetAllStorageProductsByLocation;
using DematecStock.Communication.Responses;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DematecStock.Api.Tests.Controllers;

public class ProductsLocationsControllerTests
{
    [Fact]
    public async Task Search_ShouldReturnBadRequest_WhenNoFilterIsProvided()
    {
        var controller = new ProductsLocations();
        var useCaseMock = new Mock<IGetAllLocationsByProductUseCase>(MockBehavior.Strict);

        var result = await controller.Search(useCaseMock.Object, null, null, null);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<ResponseErrorJson>(badRequest.Value);
        Assert.Contains("Informe ao menos um filtro", response.ErrorMessages.First());
    }

    [Fact]
    public async Task Search_ShouldReturnBadRequest_WhenIdProductIsInvalid()
    {
        var controller = new ProductsLocations();
        var useCaseMock = new Mock<IGetAllLocationsByProductUseCase>(MockBehavior.Strict);

        var result = await controller.Search(useCaseMock.Object, 0, null, null);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        var response = Assert.IsType<ResponseErrorJson>(badRequest.Value);
        Assert.Contains("idProduct deve ser maior que zero", response.ErrorMessages.First());
    }

    [Fact]
    public async Task Search_ShouldReturnOk_AndCallUseCaseWithTrimmedValues()
    {
        var controller = new ProductsLocations();
        var response = new ResponseProductLocations { IdProduct = 10 };

        var useCaseMock = new Mock<IGetAllLocationsByProductUseCase>();
        useCaseMock
            .Setup(x => x.Execute(10, "REF-01", "789"))
            .ReturnsAsync(response);

        var result = await controller.Search(useCaseMock.Object, 10, " REF-01 ", " 789 ");

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(response, ok.Value);

        useCaseMock.Verify(x => x.Execute(10, "REF-01", "789"), Times.Once);
    }

    [Fact]
    public async Task GetStoregeProductsByLocation_ShouldReturnOk()
    {
        var controller = new ProductsLocations();
        var response = new ResponseLocationProduct
        {
            IdLocation = 1,
            LocationName = "A1",
            Aisle = 1,
            Building = 1,
            Level = 1,
            Bin = 1,
            IsActive = "S",
            AllowsStockMovement = "S",
            AllowsReplenishment = "S",
            IsPickingLocation = "N"
        };

        var useCaseMock = new Mock<IGetAllStorageProductsByLocationUseCase>();
        useCaseMock.Setup(x => x.Execute(1)).ReturnsAsync(response);

        var result = await controller.GetStoregeProductsByLocation(useCaseMock.Object, 1);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(response, ok.Value);
    }
}
