using AutoMapper;
using DematecStock.Application.AutoMapper;
using DematecStock.Application.UseCases.ProductsAddress.GetAllLocationsByProducts;
using DematecStock.Domain.DTOs;
using DematecStock.Domain.Repositories.ProductAddress;
using DematecStock.Exception.ExceptionsBase;
using FluentAssertions;
using Moq;

namespace DematecStock.Tests.UseCases.ProductsAddress;

/// <summary>
/// Testes unitários para GetAllLocationsByProductUseCase.
/// Cobre: busca por idProduct, reference, ean13, produto não encontrado.
/// </summary>
public class GetAllLocationsByProductUseCaseTests
{
    private readonly Mock<IProductAddressReadOnlyRepository> _repositoryMock;
    private readonly IMapper _mapper;
    private readonly GetAllLocationsByProductUseCase _sut;

    public GetAllLocationsByProductUseCaseTests()
    {
        _repositoryMock = new Mock<IProductAddressReadOnlyRepository>();
        _mapper = MapperFactory.Create();
        _sut = new GetAllLocationsByProductUseCase(_repositoryMock.Object, _mapper);
    }

    /// <summary>
    /// Sucesso: busca por idProduct → retorna produto com suas localizações.
    /// </summary>
    [Fact]
    public async Task Execute_ByIdProduct_ReturnsMappedResult()
    {
        var queryResult = BuildProductLocationsResult(idProduct: 10);
        _repositoryMock.Setup(r => r.GetStoredItems(10, null, null, null, null, null, null, null)).ReturnsAsync(queryResult);

        var result = await _sut.Execute(10, null, null, null, null, null, null, null);

        result.Should().NotBeNull();
        result.IdProduct.Should().Be(10);
        result.StorageBin.Should().HaveCount(1);
    }

    /// <summary>
    /// Sucesso: busca por reference → repassa parâmetros ao repositório corretamente.
    /// </summary>
    [Fact]
    public async Task Execute_ByReference_CallsRepositoryWithReference()
    {
        var queryResult = BuildProductLocationsResult(idProduct: 20);
        _repositoryMock.Setup(r => r.GetStoredItems(null, "REF001", null, null, null, null, null, null)).ReturnsAsync(queryResult);

        var result = await _sut.Execute(null, "REF001", null, null, null, null, null, null);

        _repositoryMock.Verify(r => r.GetStoredItems(null, "REF001", null, null, null, null, null, null), Times.Once);
    }

    /// <summary>
    /// Sucesso: busca por EAN13 → repassa parâmetros ao repositório corretamente.
    /// </summary>
    [Fact]
    public async Task Execute_ByEan13_CallsRepositoryWithEan13()
    {
        var queryResult = BuildProductLocationsResult(idProduct: 30);
        _repositoryMock.Setup(r => r.GetStoredItems(null, null, "7891234567890", null, null, null, null, null)).ReturnsAsync(queryResult);

        await _sut.Execute(null, null, "7891234567890", null, null, null, null, null);

        _repositoryMock.Verify(r => r.GetStoredItems(null, null, "7891234567890", null, null, null, null, null), Times.Once);
    }

    /// <summary>
    /// Erro: produto não encontrado em nenhuma localização → NotFoundException (404) propagada.
    /// </summary>
    [Fact]
    public async Task Execute_ProductNotFound_PropagatesNotFoundException()
    {
        _repositoryMock.Setup(r => r.GetStoredItems(It.IsAny<int?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>()))
            .ThrowsAsync(new NotFoundException("Produto não encontrado em nenhuma localização."));

        var act = async () => await _sut.Execute(999, null, null, null, null, null, null, null);

        await act.Should().ThrowAsync<NotFoundException>()
            .Where(e => e.StatusCode == 404);
    }

    /// <summary>
    /// Verifica o mapeamento correto de múltiplas localizações do produto.
    /// </summary>
    [Fact]
    public async Task Execute_ProductWithMultipleLocations_MapsAllLocations()
    {
        var queryResult = new ProductLocationsQueryResult
        {
            IdProduct = 10,
            ProductDescription = "Produto Teste",
            StorageBin =
            [
                new ProductWithLocationsQueryResult { IdLocation = 1 },
                new ProductWithLocationsQueryResult { IdLocation = 2 },
                new ProductWithLocationsQueryResult { IdLocation = 3 }
            ]
        };
        _repositoryMock.Setup(r => r.GetStoredItems(10, null, null, null, null, null, null, null)).ReturnsAsync(queryResult);

        var result = await _sut.Execute(10, null, null, null, null, null, null, null);

        result.StorageBin.Should().HaveCount(3);
    }

    private static ProductLocationsQueryResult BuildProductLocationsResult(int idProduct) => new()
    {
        IdProduct = idProduct,
        ProductDescription = $"Produto {idProduct}",
        StorageBin = [new ProductWithLocationsQueryResult { IdLocation = 1 }]
    };
}
