using AutoMapper;
using DematecStock.Application.AutoMapper;
using DematecStock.Application.UseCases.ProductsAddress.GetAllStorageProductsByLocation;
using DematecStock.Domain.DTOs;
using DematecStock.Domain.Repositories.ProductAddress;
using DematecStock.Exception.ExceptionsBase;
using FluentAssertions;
using Moq;

namespace DematecStock.Tests.UseCases.ProductsAddress;

/// <summary>
/// Testes unitários para GetAllStorageProductsByLocationUseCase.
/// Cobre: localização com produtos, localização não encontrada, mapeamento.
/// </summary>
public class GetAllStorageProductsByLocationUseCaseTests
{
    private readonly Mock<IProductAddressReadOnlyRepository> _repositoryMock;
    private readonly IMapper _mapper;
    private readonly GetAllStorageProductsByLocationUseCase _sut;

    public GetAllStorageProductsByLocationUseCaseTests()
    {
        _repositoryMock = new Mock<IProductAddressReadOnlyRepository>();
        _mapper = MapperFactory.Create();
        _sut = new GetAllStorageProductsByLocationUseCase(_repositoryMock.Object, _mapper);
    }

    /// <summary>
    /// Sucesso: localização com produtos → resposta mapeada corretamente.
    /// </summary>
    [Fact]
    public async Task Execute_LocationWithProducts_ReturnsMappedResponse()
    {
        var queryResult = new LocationQueryResult
        {
            IdLocation = 1,
            LocationName = "R1-P1-N1-A1",
            Aisle = 1, Building = 1, Level = 1, Bin = 1,
            IsActive = "S",
            StoreProducts = [new LocationWithProductsQueryResult { IdProduct = 100, ProductDescription = "Produto A" }]
        };
        _repositoryMock.Setup(r => r.GetStoredItemsByLocation(1, null)).ReturnsAsync(queryResult);

        var result = await _sut.Execute(1, null);

        result.Should().NotBeNull();
        result.IdLocation.Should().Be(1);
        result.LocationName.Should().Be("R1-P1-N1-A1");
        result.StoreProducts.Should().HaveCount(1);
        result.StoreProducts![0].IdProduct.Should().Be(100);
    }

    /// <summary>
    /// Erro: localização não encontrada ou sem produtos → repositório lança NotFoundException (404).
    /// Nota: o repositório conflate "localização vazia" com "não encontrada" — problema de design documentado.
    /// </summary>
    [Fact]
    public async Task Execute_LocationNotFound_PropagatesNotFoundException()
    {
        _repositoryMock.Setup(r => r.GetStoredItemsByLocation(It.IsAny<int>(), It.IsAny<string?>()))
            .ThrowsAsync(new NotFoundException("Localização não encontrada."));

        var act = async () => await _sut.Execute(999, null);

        await act.Should().ThrowAsync<NotFoundException>()
            .Where(e => e.StatusCode == 404);
    }

    /// <summary>
    /// Verifica que o repositório é chamado com o idLocation correto.
    /// </summary>
    [Fact]
    public async Task Execute_CallsRepositoryWithCorrectId()
    {
        var queryResult = new LocationQueryResult { IdLocation = 42, StoreProducts = [] };
        _repositoryMock.Setup(r => r.GetStoredItemsByLocation(42, null)).ReturnsAsync(queryResult);

        await _sut.Execute(42, null);

        _repositoryMock.Verify(r => r.GetStoredItemsByLocation(42, It.IsAny<string?>()), Times.Once);
    }

    /// <summary>
    /// Edge: localização sem produtos na lista → retorna resposta com lista vazia.
    /// </summary>
    [Fact]
    public async Task Execute_LocationWithEmptyProductList_ReturnsEmptyStoreProducts()
    {
        var queryResult = new LocationQueryResult
        {
            IdLocation = 5, LocationName = "R5-P5-N5-A5",
            StoreProducts = []
        };
        _repositoryMock.Setup(r => r.GetStoredItemsByLocation(5, null)).ReturnsAsync(queryResult);

        var result = await _sut.Execute(5, null);

        result.StoreProducts.Should().BeEmpty();
    }
}
