using DematecStock.Api.Controllers;
using DematecStock.Application.UseCases.ProductSearch.GetProductSearch;
using DematecStock.Communication.Responses;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DematecStock.Tests.Controllers;

/// <summary>
/// Testes unitários para ProductSearchController.
/// Cobre: paginação válida, page inválido, pageSize inválido, mensagens de erro corretas.
/// </summary>
public class ProductSearchControllerTests
{
    private readonly Mock<IGetProductSearchUseCase> _useCaseMock;
    private readonly ProductSearchController _sut;

    public ProductSearchControllerTests()
    {
        _useCaseMock = new Mock<IGetProductSearchUseCase>();
        _sut = new ProductSearchController();
    }

    /// <summary>
    /// Sucesso: parâmetros válidos → 200 OK com dados.
    /// </summary>
    [Fact]
    public async Task Get_ValidParameters_Returns200Ok()
    {
        var response = new ResponseProductSearchPagedJson { Page = 1, PageSize = 20, Data = [] };
        _useCaseMock.Setup(u => u.Execute("caneta", 1, 20, null, default)).ReturnsAsync(response);

        var result = await _sut.Get(_useCaseMock.Object, "caneta", 1, 20, null, default);

        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(StatusCodes.Status200OK);
        okResult.Value.Should().BeEquivalentTo(response);
    }

    /// <summary>
    /// Erro: page = 0 → 400 BadRequest com mensagem sobre página.
    /// </summary>
    [Fact]
    public async Task Get_PageZero_Returns400WithCorrectMessage()
    {
        var result = await _sut.Get(_useCaseMock.Object, null, 0, 20, null, default);

        var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var error = badRequest.Value.Should().BeOfType<ResponseErrorJson>().Subject;
        error.ErrorMessages.Should().ContainSingle().Which.Should().Contain("Pagina");
    }

    /// <summary>
    /// Erro: page negativa → 400 BadRequest.
    /// </summary>
    [Fact]
    public async Task Get_NegativePage_Returns400()
    {
        var result = await _sut.Get(_useCaseMock.Object, null, -5, 20, null, default);

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    /// <summary>
    /// Erro: pageSize = 0 → 400 BadRequest com mensagem sobre tamanho da página.
    /// Verifica que a mensagem foi corrigida (não confunde page com pageSize).
    /// </summary>
    [Fact]
    public async Task Get_PageSizeZero_Returns400WithCorrectMessage()
    {
        var result = await _sut.Get(_useCaseMock.Object, null, 1, 0, null, default);

        var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var error = badRequest.Value.Should().BeOfType<ResponseErrorJson>().Subject;
        error.ErrorMessages.Should().ContainSingle()
            .Which.Should().Contain("Tamanho da página");
    }

    /// <summary>
    /// Erro: pageSize negativo → 400 BadRequest.
    /// </summary>
    [Fact]
    public async Task Get_NegativePageSize_Returns400()
    {
        var result = await _sut.Get(_useCaseMock.Object, null, 1, -1, null, default);

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    /// <summary>
    /// Edge: query nula + parâmetros válidos → use case é chamado com null.
    /// </summary>
    [Fact]
    public async Task Get_NullQuery_PassesNullToUseCase()
    {
        _useCaseMock.Setup(u => u.Execute(null, 1, 20, null, default))
            .ReturnsAsync(new ResponseProductSearchPagedJson());

        await _sut.Get(_useCaseMock.Object, null, 1, 20, null, default);

        _useCaseMock.Verify(u => u.Execute(null, 1, 20, null, default), Times.Once);
    }

    /// <summary>
    /// Garante que o use case NÃO é chamado quando a validação falha.
    /// </summary>
    [Fact]
    public async Task Get_InvalidPage_UseCaseNeverCalled()
    {
        await _sut.Get(_useCaseMock.Object, "q", 0, 20, null, default);

        _useCaseMock.Verify(u => u.Execute(It.IsAny<string?>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
