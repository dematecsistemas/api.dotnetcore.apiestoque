using DematecStock.Communication.Requests;
using DematecStock.Communication.Responses;
using DematecStock.Domain.Repositories.Users;
using DematecStock.Domain.Security.Cryptography;
using DematecStock.Domain.Security.Tokens;
using DematecStock.Exception.ExceptionsBase;

namespace DematecStock.Application.UseCases.Login.DoLogin
{
    public class DoLoginUseCase : IDoLoginUseCase
    {
        private readonly IUserReadOnlyRepository _userReadOnlyRepository;
        private readonly IPasswordEncrypter _passwordEncrypter;
        private readonly IAccessTokenGenerator _accessTokenGenerator;

        public DoLoginUseCase(
            IUserReadOnlyRepository userReadOnlyRepository,
            IPasswordEncrypter passwordEncrypter,
            IAccessTokenGenerator accessTokenGenerator)
        {
            _userReadOnlyRepository = userReadOnlyRepository;
            _passwordEncrypter = passwordEncrypter;
            _accessTokenGenerator = accessTokenGenerator;
        }

        public async Task<ResponseLoginUserJson> Execute(RequestLoginJson request)
        {
            // NÃO É PRECISO FAZER UM VALIDATE POIS A CLASSE DE REQUEST JÁ NÃO PERMITE NULOS OU VAZIOS

            var user = await _userReadOnlyRepository.GetUserByUsername(request.Username);

            if (user is null)
            {
                throw new InvalidLoginException();
            }

            if(user.AppPassword is null)
            {
                throw new ErrorOnValidationException("O usuário não possui uma senha");
            }

            var passwordMatch = _passwordEncrypter.Verify(request.Password, user.AppPassword);

            if(passwordMatch == false)
            {
                throw new InvalidLoginException();
            }

            return new ResponseLoginUserJson
            {
                UserId = user.Id,
                Name = user.Name,
                Token = _accessTokenGenerator.Generate(user)
            };

        }
    }
}
