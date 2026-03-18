using DematecStock.Application.UseCases.WarehouseLocations.PatchLocation;
using DematecStock.Domain.Entities;
using DematecStock.Domain.Repositories;
using DematecStock.Domain.Repositories.WarehouseLocations;
using DematecStock.Exception.ExceptionsBase;
using FluentAssertions;
using Moq;

namespace DematecStock.Tests.UseCases.WarehouseLocations;

/// <summary>
/// Testes unitários para PatchWarehouseLocationUseCase.
/// Cobre: patch parcial com sucesso, localização não encontrada, cada campo individualmente.
/// </summary>
public class PatchWarehouseLocationUseCaseTests
{
    private readonly Mock<IPatchWarehouseLocationRepository> _repositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly PatchWarehouseLocationUseCase _sut;

    public PatchWarehouseLocationUseCaseTests()
    {
        _repositoryMock = new Mock<IPatchWarehouseLocationRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _sut = new PatchWarehouseLocationUseCase(_repositoryMock.Object, _unitOfWorkMock.Object);
    }

    /// <summary>
    /// Sucesso: patch com todos os campos preenchidos → entidade atualizada e Commit chamado.
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_AllFieldsProvided_UpdatesAndCommits()
    {
        var location = new DematecStock.Domain.Entities.WarehouseLocations(1, 1, 1, 1);
        _repositoryMock.Setup(r => r.GetById(1)).ReturnsAsync(location);

        var input = new PatchWarehouseLocationInput
        {
            IsOccupied = "S",
            IsActive = "N",
            IsMovementAllowed = "S",
            IsAllowReplenishment = "N",
            IsPickingLocation = "S"
        };

        await _sut.ExecuteAsync(1, input);

        location.IsOccupied.Should().Be("S");
        location.IsActive.Should().Be("N");
        location.IsMovementAllowed.Should().Be("S");
        location.IsAllowReplenishment.Should().Be("N");
        location.IsPickingLocation.Should().Be("S");

        _repositoryMock.Verify(r => r.Update(location), Times.Once);
        _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
    }

    /// <summary>
    /// Sucesso: patch parcial → apenas o campo informado é alterado, os demais permanecem null.
    /// Garante a semântica PATCH (só muda o que veio).
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_OnlyIsActiveProvided_OnlyIsActiveChanges()
    {
        var location = new DematecStock.Domain.Entities.WarehouseLocations(1, 1, 1, 1);
        _repositoryMock.Setup(r => r.GetById(5)).ReturnsAsync(location);

        var input = new PatchWarehouseLocationInput { IsActive = "N" };

        await _sut.ExecuteAsync(5, input);

        location.IsActive.Should().Be("N");
        location.IsOccupied.Should().BeNull();
        location.IsMovementAllowed.Should().BeNull();
        location.IsAllowReplenishment.Should().BeNull();
        location.IsPickingLocation.Should().BeNull();
    }

    /// <summary>
    /// Erro: localização não encontrada → lança NotFoundException (404).
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_LocationNotFound_ThrowsNotFoundException()
    {
        _repositoryMock.Setup(r => r.GetById(It.IsAny<int>()))
            .ReturnsAsync((DematecStock.Domain.Entities.WarehouseLocations?)null);

        var act = async () => await _sut.ExecuteAsync(999, new PatchWarehouseLocationInput());

        await act.Should().ThrowAsync<NotFoundException>()
            .Where(e => e.StatusCode == 404);
    }

    /// <summary>
    /// Edge: input vazio (todos null) → nada é alterado, mas Update e Commit ainda são chamados.
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_EmptyInput_NoFieldChangedButStillCommits()
    {
        var location = new DematecStock.Domain.Entities.WarehouseLocations(1, 2, 3, 4);
        _repositoryMock.Setup(r => r.GetById(1)).ReturnsAsync(location);

        await _sut.ExecuteAsync(1, new PatchWarehouseLocationInput());

        location.IsActive.Should().BeNull();
        _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
    }

    /// <summary>
    /// Garante que Commit não é chamado quando a localização não existe.
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_LocationNotFound_CommitNeverCalled()
    {
        _repositoryMock.Setup(r => r.GetById(It.IsAny<int>()))
            .ReturnsAsync((DematecStock.Domain.Entities.WarehouseLocations?)null);

        await Assert.ThrowsAsync<NotFoundException>(
            () => _sut.ExecuteAsync(1, new PatchWarehouseLocationInput()));

        _unitOfWorkMock.Verify(u => u.Commit(), Times.Never);
    }
}
