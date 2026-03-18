using AutoMapper;
using DematecStock.Application.AutoMapper;
using DematecStock.Application.UseCases.WarehouseLocations.CreateLocation;
using DematecStock.Communication.Requests.WarehouseLocations;
using DematecStock.Domain.Repositories;
using DematecStock.Domain.Repositories.WarehouseLocations;
using FluentAssertions;
using Moq;

namespace DematecStock.Tests.UseCases.WarehouseLocations;

/// <summary>
/// Testes unitários para CreateLocationUseCase.
/// Cobre: criação com sucesso, verificação de commit, mapeamento request → entidade.
/// </summary>
public class CreateLocationUseCaseTests
{
    private readonly Mock<IWarehouseLocationsWriteOnlyRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly IMapper _mapper;
    private readonly CreateLocationUseCase _sut;

    public CreateLocationUseCaseTests()
    {
        _repositoryMock = new Mock<IWarehouseLocationsWriteOnlyRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapper = MapperFactory.Create();

        _sut = new CreateLocationUseCase(
            _repositoryMock.Object,
            _mapper,
            _unitOfWorkMock.Object);
    }

    /// <summary>
    /// Sucesso: request válido → Add e Commit são chamados uma vez cada.
    /// </summary>
    [Fact]
    public async Task Execute_ValidRequest_AddsAndCommits()
    {
        var request = new RequestWriteWarehouseLocationJson
        {
            Aisle = 1, Building = 2, Level = 3, Bin = 4,
            IsActive = "S", IsOccupied = "N"
        };

        await _sut.Execute(request);

        _repositoryMock.Verify(r => r.Add(It.IsAny<Domain.Entities.WarehouseLocations>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
    }

    /// <summary>
    /// Sucesso: verifica que o nome é gerado a partir dos campos Aisle/Building/Level/Bin.
    /// </summary>
    [Fact]
    public async Task Execute_ValidRequest_EntityNameGeneratedFromCoordinates()
    {
        Domain.Entities.WarehouseLocations? captured = null;
        _repositoryMock
            .Setup(r => r.Add(It.IsAny<Domain.Entities.WarehouseLocations>()))
            .Callback<Domain.Entities.WarehouseLocations>(e => captured = e)
            .Returns(Task.CompletedTask);

        var request = new RequestWriteWarehouseLocationJson
        {
            Aisle = 10, Building = 20, Level = 30, Bin = 40
        };

        await _sut.Execute(request);

        captured.Should().NotBeNull();
        captured!.LocationName.Should().Be("R10-P20-N30-A40");
    }

    /// <summary>
    /// Edge: Commit nunca é chamado se Add lançar exceção.
    /// Garante que transação é atômica.
    /// </summary>
    [Fact]
    public async Task Execute_AddThrows_CommitIsNeverCalled()
    {
        _repositoryMock.Setup(r => r.Add(It.IsAny<Domain.Entities.WarehouseLocations>()))
            .ThrowsAsync(new System.Exception("DB error"));

        var act = async () => await _sut.Execute(new RequestWriteWarehouseLocationJson
        {
            Aisle = 1, Building = 1, Level = 1, Bin = 1
        });

        await act.Should().ThrowAsync<System.Exception>();
        _unitOfWorkMock.Verify(u => u.Commit(), Times.Never);
    }
}
