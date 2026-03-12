using DematecStock.Api.Controllers;
using DematecStock.Application.UseCases.Login.DoLogin;
using DematecStock.Communication.Requests;
using DematecStock.Communication.Responses;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DematecStock.Api.Tests.Controllers;

public class LoginControllerTests
{
    [Fact]
    public async Task Login_ShouldReturnOk_WithTokenPayload()
    {
        var controller = new LoginController();
        var request = new RequestLoginJson { Username = "admin", Password = "123" };
        var response = new ResponseLoginUserJson { UserId = 1, Name = "Admin", Token = "jwt-token" };

        var useCaseMock = new Mock<IDoLoginUseCase>();
        useCaseMock.Setup(x => x.Execute(request)).ReturnsAsync(response);

        var result = await controller.Login(useCaseMock.Object, request);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(response, ok.Value);
        useCaseMock.Verify(x => x.Execute(request), Times.Once);
    }
}
