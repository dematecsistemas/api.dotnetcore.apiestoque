using DematecStock.Api.Controllers;
using DematecStock.Application.UseCases.ProductsAddress.GetAllStorageLocationsByProduct;
using DematecStock.Application.UseCases.ProductsAddress.GetAllStorageProductsByLocation;
using DematecStock.Communication.Responses;
using DematecStock.Exception.ExceptionsBase;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DematecStock.Tests.Controllers;

/// <summary>
/// Testes unitários para ProductsLocations controller.
/// Cobre: busca por localização, busca por produto, validações de rota e query.
/// </summary>
public class ProductsLocationsControllerTests
{
    private readonly Mock<IGetAllStorageProductsByLocationUseCase> _byLocationUseCaseMock;
    private readonly Mock<IGetAllLocationsByProductUseCase> _byProductUseCaseMock;
    private readonly ProductsLocationsController _sut;

    public ProductsLocationsControllerTests()
    {
        _byLocationUseCaseMock = new Mock<IGetAllStorageProductsByLocationUseCase>();
        _byProductUseCaseMock = new Mock<IGetAllLocationsByProductUseCase>();
        _sut = new ProductsLocationsController();
    }

    // --- GetStoregeProductsByLocation ---

    /// <summary>
    /// Sucesso: idLocation válido → 200 OK com produtos da localização.
    /// </summary>
    [Fact]
    public async Task GetStoregeProductsByLocation_ValidId_Returns200Ok()
    {
        var response = new ResponseLocationProduct
        {
            IdLocation = 1, LocationName = "R1-P1-N1-A1",
            Aisle = 1, Building = 1, Level = 1, Bin = 1,
            IsActive = "S", AllowsStockMovement = "S",
            AllowsReplenishment = "S", IsPickingLocation = "N"
        };
        _byLocationUseCaseMock.Setup(u => u.Execute(1, null)).ReturnsAsync(response);

        var result = await _sut.GetStoregeProductsByLocation(_byLocationUseCaseMock.Object, 1, null);

        var ok = result.Should().BeOfType<OkObjectResult>().Subject;
        ok.StatusCode.Should().Be(StatusCodes.Status200OK);
        ok.Value.Should().BeEquivalentTo(response);
    }

    /// <summary>
    /// Erro: localização não encontrada → NotFoundException propagada para o ExceptionFilter.
    /// </summary>
    [Fact]
    public async Task GetStoregeProductsByLocation_NotFound_PropagatesNotFoundException()
    {
        _byLocationUseCaseMock.Setup(u => u.Execute(999, It.IsAny<string?>()))
            .ThrowsAsync(new NotFoundException("Localização não encontrada."));

        var act = async () => await _sut.GetStoregeProductsByLocation(_byLocationUseCaseMock.Object, 999, null);

        await act.Should().ThrowAsync<NotFoundException>();
    }

    // --- Search ---

    /// <summary>
    /// Sucesso: busca por idProduct → 200 OK com localizações do produto.
    /// </summary>
    [Fact]
    public async Task Search_ByIdProduct_Returns200Ok()
    {
        var response = new ResponseProductLocations { IdProduct = 10, ProductDescription = "Produto A" };
        _byProductUseCaseMock.Setup(u => u.Execute(10, null, null, null, null, null, null, null)).ReturnsAsync(response);

        var result = await _sut.Search(_byProductUseCaseMock.Object, 10, null, null, null, null, null, null, null);

        result.Should().BeOfType<OkObjectResult>()
            .Which.StatusCode.Should().Be(StatusCodes.Status200OK);
    }

    /// <summary>
    /// Sucesso: busca por reference → 200 OK.
    /// </summary>
    [Fact]
    public async Task Search_ByReference_Returns200Ok()
    {
        var response = new ResponseProductLocations { IdProduct = 20 };
        _byProductUseCaseMock.Setup(u => u.Execute(null, "REF001", null, null, null, null, null, null)).ReturnsAsync(response);

        var result = await _sut.Search(_byProductUseCaseMock.Object, null, "REF001", null, null, null, null, null, null);

        result.Should().BeOfType<OkObjectResult>();
    }

    /// <summary>
    /// Erro: nenhum filtro informado → 400 BadRequest.
    /// </summary>
    [Fact]
    public async Task Search_NoFilters_Returns400()
    {
        var result = await _sut.Search(_byProductUseCaseMock.Object, null, null, null, null, null, null, null, null);

        var bad = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var error = bad.Value.Should().BeOfType<ResponseErrorJson>().Subject;
        error.ErrorMessages.Should().ContainSingle()
            .Which.Should().Contain("filtro");
    }

    /// <summary>
    /// Erro: idProduct = 0 → 400 BadRequest (deve ser maior que zero).
    /// </summary>
    [Fact]
    public async Task Search_IdProductZero_Returns400()
    {
        var result = await _sut.Search(_byProductUseCaseMock.Object, 0, null, null, null, null, null, null, null);

        result.Should().BeOfType<BadRequestObjectResult>();
        _byProductUseCaseMock.Verify(u => u.Execute(It.IsAny<int?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>()), Times.Never);
    }

    /// <summary>
    /// Erro: idProduct negativo → 400 BadRequest.
    /// </summary>
    [Fact]
    public async Task Search_NegativeIdProduct_Returns400()
    {
        var result = await _sut.Search(_byProductUseCaseMock.Object, -1, null, null, null, null, null, null, null);

        result.Should().BeOfType<BadRequestObjectResult>();
    }

    /// <summary>
    /// Edge: reference com espaços → é trimada antes de passar ao use case.
    /// </summary>
    [Fact]
    public async Task Search_ReferenceWithWhitespace_IsTrimmedBeforePassingToUseCase()
    {
        var response = new ResponseProductLocations { IdProduct = 1 };
        _byProductUseCaseMock.Setup(u => u.Execute(null, "REF001", null, null, null, null, null, null)).ReturnsAsync(response);

        await _sut.Search(_byProductUseCaseMock.Object, null, "  REF001  ", null, null, null, null, null, null);

        _byProductUseCaseMock.Verify(u => u.Execute(null, "REF001", null, null, null, null, null, null), Times.Once);
    }

    /// <summary>
    /// Edge: ean13code com espaços → é trimado antes de passar ao use case.
    /// </summary>
    [Fact]
    public async Task Search_Ean13WithWhitespace_IsTrimmedBeforePassingToUseCase()
    {
        var response = new ResponseProductLocations { IdProduct = 1 };
        _byProductUseCaseMock.Setup(u => u.Execute(null, null, "7891234567890", null, null, null, null, null)).ReturnsAsync(response);

        await _sut.Search(_byProductUseCaseMock.Object, null, null, "  7891234567890  ", null, null, null, null, null);

        _byProductUseCaseMock.Verify(u => u.Execute(null, null, "7891234567890", null, null, null, null, null), Times.Once);
    }

    /// <summary>
    /// Edge: filtro apenas com whitespace → tratado como ausente → 400 BadRequest.
    /// </summary>
    [Fact]
    public async Task Search_OnlyWhitespaceFilters_Returns400()
    {
        var result = await _sut.Search(_byProductUseCaseMock.Object, null, "   ", "   ", null, null, null, null, null);

        result.Should().BeOfType<BadRequestObjectResult>();
    }
}
