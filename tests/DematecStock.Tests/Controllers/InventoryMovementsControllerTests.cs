using DematecStock.Api.Controllers;
using DematecStock.Application.UseCases.InventoryMovement;
using DematecStock.Communication.Requests.InventoryMovement;
using DematecStock.Communication.Responses;
using DematecStock.Exception.ExceptionsBase;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace DematecStock.Tests.Controllers;

/// <summary>
/// Testes unitários para InventoryMovementsController.
/// Cobre: sucesso (201), validação (400), exceção inesperada (500)
/// e garantia de que o UseCase é invocado corretamente.
/// </summary>
public class InventoryMovementsControllerTests
{
    private readonly Mock<IAddInventoryMovementsUseCase> _useCaseMock;
    private readonly InventoryMovementsController _sut;

    public InventoryMovementsControllerTests()
    {
        _useCaseMock = new Mock<IAddInventoryMovementsUseCase>();
        _sut = new InventoryMovementsController();
    }

    // ── helpers ──────────────────────────────────────────────────────────────

    private static RequestAddInventoryMovementJsons ValidRequest() => new()
    {
        IdProduct = 10,
        IdLocation = 5,
        IdUser = 1,
        Operation = "TRANSFERÊNCIA",
        MovementDirection = 'E',
        Quantity = 50m,
        Remarks = null
    };

    // ── testes de sucesso ─────────────────────────────────────────────────────

    /// <summary>
    /// Fluxo feliz: UseCase executa sem erro → controller retorna 201 Created.
    /// </summary>
    [Fact]
    public async Task Add_ValidRequest_Returns201Created()
    {
        _useCaseMock
            .Setup(u => u.Execute(It.IsAny<RequestAddInventoryMovementJsons>()))
            .Returns(Task.CompletedTask);

        var result = await _sut.Add(_useCaseMock.Object, ValidRequest());

        var statusResult = result.Should().BeOfType<StatusCodeResult>().Subject;
        statusResult.StatusCode.Should().Be(StatusCodes.Status201Created);
    }

    /// <summary>
    /// O UseCase deve ser chamado exatamente uma vez com a requisição recebida.
    /// Garante que o controller não filtra, altera ou ignora o request antes de delegar.
    /// </summary>
    [Fact]
    public async Task Add_ValidRequest_DelegatesExactRequestToUseCase()
    {
        RequestAddInventoryMovementJsons? captured = null;

        _useCaseMock
            .Setup(u => u.Execute(It.IsAny<RequestAddInventoryMovementJsons>()))
            .Callback<RequestAddInventoryMovementJsons>(r => captured = r)
            .Returns(Task.CompletedTask);

        var request = ValidRequest();
        await _sut.Add(_useCaseMock.Object, request);

        _useCaseMock.Verify(u => u.Execute(It.IsAny<RequestAddInventoryMovementJsons>()), Times.Once);
        captured.Should().BeSameAs(request);
    }

    // ── testes de validação (400) ─────────────────────────────────────────────

    /// <summary>
    /// UseCase lança ErrorOnValidationException (validação de negócio) →
    /// a exceção deve se propagar até o ExceptionFilter, que retorna 400.
    /// No teste unitário do controller verificamos apenas que a exceção não é engolida.
    /// </summary>
    [Fact]
    public async Task Add_UseCaseThrowsValidationException_PropagatesException()
    {
        var errors = new List<string> { "A quantidade do movimento deve ser maior que zero." };

        _useCaseMock
            .Setup(u => u.Execute(It.IsAny<RequestAddInventoryMovementJsons>()))
            .ThrowsAsync(new ErrorOnValidationException(errors));

        var act = async () => await _sut.Add(_useCaseMock.Object, ValidRequest());

        await act.Should().ThrowAsync<ErrorOnValidationException>()
            .WithMessage("*");
    }

    /// <summary>
    /// Múltiplos erros de validação → todos os erros são propagados sem perda.
    /// </summary>
    [Fact]
    public async Task Add_UseCaseThrowsMultipleValidationErrors_AllErrorsPropagated()
    {
        var errors = new List<string>
        {
            "A quantidade do movimento deve ser maior que zero.",
            "A direção do movimento deve ser 'E' para entrada ou 'S' para saída.",
            "O produto informado é inválido."
        };

        _useCaseMock
            .Setup(u => u.Execute(It.IsAny<RequestAddInventoryMovementJsons>()))
            .ThrowsAsync(new ErrorOnValidationException(errors));

        var act = async () => await _sut.Add(_useCaseMock.Object, ValidRequest());

        var ex = await act.Should().ThrowAsync<ErrorOnValidationException>();
        ex.Which.GetErrors().Should().HaveCount(3);
    }

    // ── testes de exceção inesperada (500) ────────────────────────────────────

    /// <summary>
    /// UseCase lança exceção genérica (ex: falha de banco) →
    /// deve propagar sem ser capturada pelo controller.
    /// O ExceptionFilter (ThrowUnknownError) é responsável pelo 500.
    /// </summary>
    [Fact]
    public async Task Add_UseCaseThrowsUnexpectedException_PropagatesException()
    {
        _useCaseMock
            .Setup(u => u.Execute(It.IsAny<RequestAddInventoryMovementJsons>()))
            .ThrowsAsync(new InvalidOperationException("Erro de infraestrutura simulado"));

        var act = async () => await _sut.Add(_useCaseMock.Object, ValidRequest());

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Erro de infraestrutura simulado");
    }

    // ── testes de edge case ───────────────────────────────────────────────────

    /// <summary>
    /// Remarks nulo: não deve causar falha no controller nem no UseCase.
    /// </summary>
    [Fact]
    public async Task Add_NullRemarks_Returns201Created()
    {
        _useCaseMock
            .Setup(u => u.Execute(It.IsAny<RequestAddInventoryMovementJsons>()))
            .Returns(Task.CompletedTask);

        var request = ValidRequest();
        request.Remarks = null;

        var result = await _sut.Add(_useCaseMock.Object, request);

        result.Should().BeOfType<StatusCodeResult>()
            .Which.StatusCode.Should().Be(StatusCodes.Status201Created);
    }

    /// <summary>
    /// Direção 'S' (saída): deve funcionar igual à entrada.
    /// </summary>
    [Fact]
    public async Task Add_DirectionSaida_Returns201Created()
    {
        _useCaseMock
            .Setup(u => u.Execute(It.IsAny<RequestAddInventoryMovementJsons>()))
            .Returns(Task.CompletedTask);

        var request = ValidRequest();
        request.MovementDirection = 'S';

        var result = await _sut.Add(_useCaseMock.Object, request);

        result.Should().BeOfType<StatusCodeResult>()
            .Which.StatusCode.Should().Be(StatusCodes.Status201Created);
    }

    /// <summary>
    /// Quantidade decimal com muitas casas: o controller não deve truncar ou arredondar.
    /// </summary>
    [Fact]
    public async Task Add_DecimalQuantity_PassesExactValueToUseCase()
    {
        RequestAddInventoryMovementJsons? captured = null;

        _useCaseMock
            .Setup(u => u.Execute(It.IsAny<RequestAddInventoryMovementJsons>()))
            .Callback<RequestAddInventoryMovementJsons>(r => captured = r)
            .Returns(Task.CompletedTask);

        var request = ValidRequest();
        request.Quantity = 123.4567m;

        await _sut.Add(_useCaseMock.Object, request);

        captured!.Quantity.Should().Be(123.4567m);
    }
}
