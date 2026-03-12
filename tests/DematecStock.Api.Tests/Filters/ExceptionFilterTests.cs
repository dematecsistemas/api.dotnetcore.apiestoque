using DematecStock.Api.Filters;
using DematecStock.Communication.Responses;
using DematecStock.Exception.ExceptionsBase;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace DematecStock.Api.Tests.Filters;

public class ExceptionFilterTests
{
    [Fact]
    public void OnException_ShouldReturnNotFound_ForProjectException()
    {
        var filter = new ExceptionFilter();
        var context = CreateExceptionContext(new NotFoundException("Produto não encontrado."));

        filter.OnException(context);

        Assert.Equal(StatusCodes.Status404NotFound, context.HttpContext.Response.StatusCode);
        var result = Assert.IsType<ObjectResult>(context.Result);
        var response = Assert.IsType<ResponseErrorJson>(result.Value);
        Assert.Contains("Produto não encontrado.", response.ErrorMessages.First());
    }

    [Fact]
    public void OnException_ShouldReturnInternalServerError_ForUnknownException()
    {
        var filter = new ExceptionFilter();
        var context = CreateExceptionContext(new InvalidOperationException("erro"));

        filter.OnException(context);

        Assert.Equal(StatusCodes.Status500InternalServerError, context.HttpContext.Response.StatusCode);
        var result = Assert.IsType<ObjectResult>(context.Result);
        var response = Assert.IsType<ResponseErrorJson>(result.Value);
        Assert.Contains("Ocorreu um erro desconhecido", response.ErrorMessages.First());
    }

    private static ExceptionContext CreateExceptionContext(System.Exception ex)
    {
        var actionContext = new ActionContext(
            new DefaultHttpContext(),
            new RouteData(),
            new ActionDescriptor());

        return new ExceptionContext(actionContext, new List<IFilterMetadata>())
        {
            Exception = ex
        };
    }
}
