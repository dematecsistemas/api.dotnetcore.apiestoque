using DematecStock.Application.UseCases.ProductSearch.GetProductSearch;
using DematecStock.Domain.DTOs;
using DematecStock.Domain.Repositories.ProductSearch;
using FluentAssertions;
using Moq;

namespace DematecStock.Tests.UseCases.ProductSearch;

/// <summary>
/// Testes unitários para GetProductSearchUseCase.
/// Cobre: paginação, query vazia, page/pageSize máximo, normalização de parâmetros inválidos.
/// </summary>
public class GetProductSearchUseCaseTests
{
    private readonly Mock<IProductSearchReadOnlyRepository> _repositoryMock;
    private readonly GetProductSearchUseCase _sut;

    public GetProductSearchUseCaseTests()
    {
        _repositoryMock = new Mock<IProductSearchReadOnlyRepository>();
        _sut = new GetProductSearchUseCase(_repositoryMock.Object);
    }

    /// <summary>
    /// Sucesso: busca com query e página válidos → retorna dados paginados.
    /// </summary>
    [Fact]
    public async Task Execute_ValidQuery_ReturnsPaginatedData()
    {
        var rows = BuildProductRows(3);
        _repositoryMock.Setup(r => r.SearchAsync("caneta", 1, 20, null, default)).ReturnsAsync(rows);

        var result = await _sut.Execute("caneta", 1, 20, null, default);

        result.Should().NotBeNull();
        result.Page.Should().Be(1);
        result.PageSize.Should().Be(20);
        result.Data.Should().HaveCount(3);
    }

    /// <summary>
    /// Sucesso: query nula → repassa null ao repositório (busca todos).
    /// </summary>
    [Fact]
    public async Task Execute_NullQuery_PassesNullToRepository()
    {
        _repositoryMock.Setup(r => r.SearchAsync(null, 1, 20, null, default)).ReturnsAsync([]);

        var result = await _sut.Execute(null, 1, 20, null, default);

        result.Data.Should().BeEmpty();
        _repositoryMock.Verify(r => r.SearchAsync(null, 1, 20, null, default), Times.Once);
    }

    /// <summary>
    /// Sucesso: query com espaços extras → deve ser trimada antes de chegar ao repositório.
    /// </summary>
    [Fact]
    public async Task Execute_QueryWithWhitespace_QueryIsTrimmed()
    {
        _repositoryMock.Setup(r => r.SearchAsync("caneta", 1, 20, null, default)).ReturnsAsync([]);

        await _sut.Execute("  caneta  ", 1, 20, null, default);

        _repositoryMock.Verify(r => r.SearchAsync("caneta", 1, 20, null, default), Times.Once);
    }

    /// <summary>
    /// Edge: pageSize maior que 50 → truncado para 50 (proteção contra sobrecarga).
    /// </summary>
    [Fact]
    public async Task Execute_PageSizeOver50_ClampsTo50()
    {
        _repositoryMock.Setup(r => r.SearchAsync(It.IsAny<string>(), 1, 50, null, default)).ReturnsAsync([]);

        var result = await _sut.Execute("x", 1, 200, null, default);

        result.PageSize.Should().Be(50);
        _repositoryMock.Verify(r => r.SearchAsync(It.IsAny<string>(), 1, 50, null, default), Times.Once);
    }

    /// <summary>
    /// Edge: page menor que 1 → normalizado para 1 pelo use case (defesa em profundidade).
    /// </summary>
    [Fact]
    public async Task Execute_PageZeroOrNegative_NormalizedToOne()
    {
        _repositoryMock.Setup(r => r.SearchAsync(It.IsAny<string?>(), 1, 20, null, default)).ReturnsAsync([]);

        var result = await _sut.Execute(null, 0, 20, null, default);

        result.Page.Should().Be(1);
    }

    /// <summary>
    /// Verifica o mapeamento correto dos campos de produto no retorno.
    /// </summary>
    [Fact]
    public async Task Execute_ValidData_MapsAllProductFields()
    {
        var row = new ProductSearchQueryResult
        {
            IdProduct = 42,
            ProductDescription = "Caneta Azul",
            BaseUoM = "UN",
            Ean13Code = "7891234567890",
            IsProductInactive = false
        };
        _repositoryMock.Setup(r => r.SearchAsync("caneta", 1, 20, null, default)).ReturnsAsync([row]);

        var result = await _sut.Execute("caneta", 1, 20, null, default);

        var item = result.Data.Should().ContainSingle().Subject;
        item.IdProduct.Should().Be(42);
        item.ProductDescription.Should().Be("Caneta Azul");
        item.BaseUoM.Should().Be("UN");
        item.Ean13Code.Should().Be("7891234567890");
        item.IsProductInactive.Should().BeFalse();
    }

    /// <summary>
    /// Verifica que CancellationToken é passado ao repositório.
    /// </summary>
    [Fact]
    public async Task Execute_WithCancellationToken_PassesTokenToRepository()
    {
        using var cts = new CancellationTokenSource();
        _repositoryMock.Setup(r => r.SearchAsync(It.IsAny<string?>(), 1, 10, null, cts.Token)).ReturnsAsync([]);

        await _sut.Execute("q", 1, 10, null, cts.Token);

        _repositoryMock.Verify(r => r.SearchAsync(It.IsAny<string?>(), 1, 10, null, cts.Token), Times.Once);
    }

    private static List<ProductSearchQueryResult> BuildProductRows(int count)
        => Enumerable.Range(1, count)
            .Select(i => new ProductSearchQueryResult { IdProduct = i, ProductDescription = $"Produto {i}" })
            .ToList();
}
