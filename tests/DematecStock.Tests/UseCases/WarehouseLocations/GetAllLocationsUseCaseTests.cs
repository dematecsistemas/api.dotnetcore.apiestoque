using AutoMapper;
using DematecStock.Application.AutoMapper;
using DematecStock.Application.UseCases.WarehouseLocations.GetAllLocations;
using DematecStock.Communication.Responses;
using DematecStock.Domain.Entities;
using DematecStock.Domain.Repositories.WarehouseLocations;
using FluentAssertions;
using Moq;

namespace DematecStock.Tests.UseCases.WarehouseLocations;

/// <summary>
/// Testes unitários para GetAllLocationsUseCase.
/// Cobre: lista com dados, lista vazia, mapeamento correto.
/// </summary>
public class GetAllLocationsUseCaseTests
{
    private readonly Mock<IWarehouseLocationsReadOnlyRepository> _repositoryMock;
    private readonly IMapper _mapper;
    private readonly GetAllLocationsUseCase _sut;

    public GetAllLocationsUseCaseTests()
    {
        _repositoryMock = new Mock<IWarehouseLocationsReadOnlyRepository>();
        _mapper = MapperFactory.Create();
        _sut = new GetAllLocationsUseCase(_repositoryMock.Object, _mapper);
    }

    /// <summary>
    /// Sucesso: repositório retorna localizações → resposta mapeada corretamente.
    /// </summary>
    [Fact]
    public async Task Execute_WithLocations_ReturnsMappedList()
    {
        var locations = new List<Domain.Entities.WarehouseLocations>
        {
            new(1, 2, 3, 4) { IsActive = "S" },
            new(5, 6, 7, 8) { IsActive = "N" }
        };

        _repositoryMock.Setup(r => r.GetAllWarehouseLocations(null, null, null, null)).ReturnsAsync(locations);

        var result = await _sut.Execute(null, null, null, null);

        result.Should().HaveCount(2);
        result[0].Aisle.Should().Be(1);
        result[0].Building.Should().Be(2);
        result[1].Aisle.Should().Be(5);
    }

    /// <summary>
    /// Edge: repositório retorna lista vazia → retorna lista vazia (200, não 404).
    /// Observação: o controller declara 404 mas o use case nunca lança NotFoundException.
    /// </summary>
    [Fact]
    public async Task Execute_NoLocations_ReturnsEmptyList()
    {
        _repositoryMock.Setup(r => r.GetAllWarehouseLocations(null, null, null, null)).ReturnsAsync([]);

        var result = await _sut.Execute(null, null, null, null);

        result.Should().BeEmpty();
    }

    /// <summary>
    /// Verifica que o nome da localização é gerado corretamente pelo construtor da entidade.
    /// </summary>
    [Fact]
    public async Task Execute_LocationNameGeneratedCorrectly()
    {
        var locations = new List<Domain.Entities.WarehouseLocations> { new(1, 2, 3, 4) };
        _repositoryMock.Setup(r => r.GetAllWarehouseLocations(null, null, null, null)).ReturnsAsync(locations);

        var result = await _sut.Execute(null, null, null, null);

        result[0].LocationName.Should().Be("R1-P2-N3-A4");
    }

    /// <summary>
    /// Verifica que o repositório é chamado exatamente uma vez.
    /// </summary>
    [Fact]
    public async Task Execute_CallsRepositoryOnce()
    {
        _repositoryMock.Setup(r => r.GetAllWarehouseLocations(null, null, null, null)).ReturnsAsync([]);

        await _sut.Execute(null, null, null, null);

        _repositoryMock.Verify(r => r.GetAllWarehouseLocations(null, null, null, null), Times.Once);
    }
}
