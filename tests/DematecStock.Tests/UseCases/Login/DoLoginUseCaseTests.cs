using DematecStock.Application.UseCases.Login.DoLogin;
using DematecStock.Communication.Requests;
using DematecStock.Domain.Entities;
using DematecStock.Domain.Repositories.Users;
using DematecStock.Domain.Security.Cryptography;
using DematecStock.Domain.Security.Tokens;
using DematecStock.Exception.ExceptionsBase;
using FluentAssertions;
using Moq;

namespace DematecStock.Tests.UseCases.Login;

/// <summary>
/// Testes unitários para DoLoginUseCase.
/// Cobre: login com sucesso, usuário não encontrado, senha nula, senha incorreta.
/// </summary>
public class DoLoginUseCaseTests
{
    private readonly Mock<IUserReadOnlyRepository> _userRepositoryMock;
    private readonly Mock<IPasswordEncrypter> _passwordEncrypterMock;
    private readonly Mock<IAccessTokenGenerator> _tokenGeneratorMock;
    private readonly DoLoginUseCase _sut;

    public DoLoginUseCaseTests()
    {
        _userRepositoryMock = new Mock<IUserReadOnlyRepository>();
        _passwordEncrypterMock = new Mock<IPasswordEncrypter>();
        _tokenGeneratorMock = new Mock<IAccessTokenGenerator>();

        _sut = new DoLoginUseCase(
            _userRepositoryMock.Object,
            _passwordEncrypterMock.Object,
            _tokenGeneratorMock.Object);
    }

    /// <summary>
    /// Sucesso: credenciais válidas → retorna token e dados do usuário.
    /// </summary>
    [Fact]
    public async Task Execute_ValidCredentials_ReturnsTokenResponse()
    {
        var user = new User { Id = 1, Login = "user1", AppPassword = "hashed", Name = "João" };
        var request = new RequestLoginJson { Username = "user1", Password = "secret" };

        _userRepositoryMock.Setup(r => r.GetUserByUsername("user1")).ReturnsAsync(user);
        _passwordEncrypterMock.Setup(p => p.Verify("secret", "hashed")).Returns(true);
        _tokenGeneratorMock.Setup(t => t.Generate(user)).Returns("jwt-token");

        var response = await _sut.Execute(request);

        response.Should().NotBeNull();
        response.UserId.Should().Be(1);
        response.Name.Should().Be("João");
        response.Token.Should().Be("jwt-token");
    }

    /// <summary>
    /// Erro: usuário não existe → lança InvalidLoginException (401).
    /// Garante que a mensagem de login inválido não vaza qual campo está errado.
    /// </summary>
    [Fact]
    public async Task Execute_UserNotFound_ThrowsInvalidLoginException()
    {
        _userRepositoryMock.Setup(r => r.GetUserByUsername(It.IsAny<string>())).ReturnsAsync((User?)null);

        var act = async () => await _sut.Execute(new RequestLoginJson { Username = "x", Password = "y" });

        await act.Should().ThrowAsync<InvalidLoginException>()
            .Where(e => e.StatusCode == 401);
    }

    /// <summary>
    /// Erro: usuário existe mas não tem senha cadastrada (AppPassword == null).
    /// Esperado: ErrorOnValidationException com 400.
    /// Bug fixado: antes lançava NullReferenceException.
    /// </summary>
    [Fact]
    public async Task Execute_UserWithNullPassword_ThrowsErrorOnValidationException()
    {
        var user = new User { Id = 2, Login = "semsenha", AppPassword = null };
        _userRepositoryMock.Setup(r => r.GetUserByUsername("semsenha")).ReturnsAsync(user);

        var act = async () => await _sut.Execute(new RequestLoginJson { Username = "semsenha", Password = "123" });

        await act.Should().ThrowAsync<ErrorOnValidationException>()
            .Where(e => e.StatusCode == 400);
    }

    /// <summary>
    /// Erro: senha incorreta → lança InvalidLoginException (401).
    /// </summary>
    [Fact]
    public async Task Execute_WrongPassword_ThrowsInvalidLoginException()
    {
        var user = new User { Id = 3, Login = "user3", AppPassword = "hashed" };
        _userRepositoryMock.Setup(r => r.GetUserByUsername("user3")).ReturnsAsync(user);
        _passwordEncrypterMock.Setup(p => p.Verify("wrong", "hashed")).Returns(false);

        var act = async () => await _sut.Execute(new RequestLoginJson { Username = "user3", Password = "wrong" });

        await act.Should().ThrowAsync<InvalidLoginException>()
            .Where(e => e.StatusCode == 401);
    }

    /// <summary>
    /// Edge: username vazio → usuário não encontrado → InvalidLoginException.
    /// Demonstra que a falta de validação em RequestLoginJson não causa falha silenciosa.
    /// </summary>
    [Fact]
    public async Task Execute_EmptyUsername_ThrowsInvalidLoginException()
    {
        _userRepositoryMock.Setup(r => r.GetUserByUsername(string.Empty)).ReturnsAsync((User?)null);

        var act = async () => await _sut.Execute(new RequestLoginJson { Username = string.Empty, Password = "x" });

        await act.Should().ThrowAsync<InvalidLoginException>();
    }

    /// <summary>
    /// Garante que o token NÃO é gerado quando a senha está errada.
    /// </summary>
    [Fact]
    public async Task Execute_WrongPassword_NeverCallsTokenGenerator()
    {
        var user = new User { Id = 1, Login = "u", AppPassword = "hash" };
        _userRepositoryMock.Setup(r => r.GetUserByUsername("u")).ReturnsAsync(user);
        _passwordEncrypterMock.Setup(p => p.Verify(It.IsAny<string>(), "hash")).Returns(false);

        await Assert.ThrowsAsync<InvalidLoginException>(
            () => _sut.Execute(new RequestLoginJson { Username = "u", Password = "wrong" }));

        _tokenGeneratorMock.Verify(t => t.Generate(It.IsAny<User>()), Times.Never);
    }
}
