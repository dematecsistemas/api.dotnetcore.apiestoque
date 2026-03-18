using DematecStock.Exception.ExceptionsBase;
using FluentAssertions;

namespace DematecStock.Tests;

/// <summary>
/// Testes para as classes de exceção do domínio.
/// Garante que os construtores funcionam corretamente (especialmente o bug corrigido).
/// </summary>
public class ExceptionTests
{
    // --- ErrorOnValidationException ---

    /// <summary>
    /// Bug fixado: construtor de string único não deve lançar NullReferenceException.
    /// </summary>
    [Fact]
    public void ErrorOnValidationException_SingleStringCtor_DoesNotThrowNullReference()
    {
        var act = () => new ErrorOnValidationException("mensagem de erro");

        act.Should().NotThrow();
    }

    /// <summary>
    /// Construtor de string → GetErrors retorna lista com a mensagem.
    /// </summary>
    [Fact]
    public void ErrorOnValidationException_SingleStringCtor_GetErrorsReturnsMessage()
    {
        var ex = new ErrorOnValidationException("campo inválido");

        ex.GetErrors().Should().ContainSingle().Which.Should().Be("campo inválido");
    }

    /// <summary>
    /// Construtor de lista → GetErrors retorna todas as mensagens.
    /// </summary>
    [Fact]
    public void ErrorOnValidationException_ListCtor_GetErrorsReturnsAllMessages()
    {
        var errors = new List<string> { "erro 1", "erro 2", "erro 3" };

        var ex = new ErrorOnValidationException(errors);

        ex.GetErrors().Should().BeEquivalentTo(errors);
    }

    /// <summary>
    /// StatusCode deve ser 400.
    /// </summary>
    [Fact]
    public void ErrorOnValidationException_StatusCode_Is400()
    {
        var ex = new ErrorOnValidationException("x");

        ex.StatusCode.Should().Be(400);
    }

    // --- NotFoundException ---

    /// <summary>
    /// NotFoundException deve ter StatusCode 404.
    /// </summary>
    [Fact]
    public void NotFoundException_StatusCode_Is404()
    {
        var ex = new NotFoundException("recurso não encontrado");

        ex.StatusCode.Should().Be(404);
    }

    /// <summary>
    /// NotFoundException deve retornar a mensagem em GetErrors.
    /// </summary>
    [Fact]
    public void NotFoundException_GetErrors_ReturnsMessage()
    {
        var ex = new NotFoundException("not found");

        ex.GetErrors().Should().ContainSingle().Which.Should().Be("not found");
    }

    // --- InvalidLoginException ---

    /// <summary>
    /// InvalidLoginException deve ter StatusCode 401.
    /// </summary>
    [Fact]
    public void InvalidLoginException_StatusCode_Is401()
    {
        var ex = new InvalidLoginException();

        ex.StatusCode.Should().Be(401);
    }

    /// <summary>
    /// InvalidLoginException não deve revelar qual campo está errado (mensagem genérica combinada).
    /// A mensagem "Login ou senha inválidos" é correta: não diz "senha incorreta" nem "usuário inexistente".
    /// </summary>
    [Fact]
    public void InvalidLoginException_GetErrors_ReturnsGenericMessage()
    {
        var ex = new InvalidLoginException();
        var message = ex.GetErrors().Should().ContainSingle().Subject;

        // Mensagem genérica: menciona os dois campos, não revela qual está errado
        message.Should().Contain("Login");
        message.Should().Contain("senha");
        message.Should().NotContain("inválido o usuário")
            .And.NotContain("senha incorreta");
    }
}
