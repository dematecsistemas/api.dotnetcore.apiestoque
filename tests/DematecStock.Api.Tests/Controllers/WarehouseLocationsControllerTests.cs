using DematecStock.Api.Controllers;
using DematecStock.Application.UseCases.WarehouseLocations.CreateLocation;
using DematecStock.Application.UseCases.WarehouseLocations.GetAllLocations;
using DematecStock.Application.UseCases.WarehouseLocations.PatchLocation;
using DematecStock.Communication.Requests.WarehouseLocations;
using DematecStock.Communication.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DematecStock.Api.Tests.Controllers;

public class WarehouseLocationsControllerTests
{
    [Fact]
    public async Task GetLocationWithProducts_ShouldReturnOk_WithList()
    {
        var controller = new WarehouseLocationsController();
        var locations = new List<ResponseLocationsJson>
        {
            new() { IdLocation = 1, LocationName = "A-01" }
        };

        var useCaseMock = new Mock<IGetAllLocationsUseCase>();
        useCaseMock.Setup(x => x.Execute()).ReturnsAsync(locations);

        var result = await controller.GetLocationWithProducts(useCaseMock.Object);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(locations, ok.Value);
    }

    [Fact]
    public async Task Create_ShouldReturn201Created()
    {
        var controller = new WarehouseLocationsController();
        var request = new RequestWriteWarehouseLocationJson { IdLocation = 11 };

        var useCaseMock = new Mock<ICreateLocationUseCase>();
        useCaseMock.Setup(x => x.Execute(request)).Returns(Task.CompletedTask);

        var result = await controller.Create(useCaseMock.Object, request);

        var statusCode = Assert.IsType<StatusCodeResult>(result);
        Assert.Equal(StatusCodes.Status201Created, statusCode.StatusCode);
    }

    [Fact]
    public async Task Patch_ShouldReturnNoContent_AndCallUseCase()
    {
        var controller = new WarehouseLocationsController();
        var input = new PatchWarehouseLocationInput { IsActive = "S" };

        var useCaseMock = new Mock<IPatchWarehouseLocationUseCase>();
        useCaseMock.Setup(x => x.ExecuteAsync(5, input)).Returns(Task.CompletedTask);

        var result = await controller.Patch(useCaseMock.Object, 5, input);

        Assert.IsType<NoContentResult>(result);
        useCaseMock.Verify(x => x.ExecuteAsync(5, input), Times.Once);
    }
}
