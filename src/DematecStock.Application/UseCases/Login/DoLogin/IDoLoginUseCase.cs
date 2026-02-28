using DematecStock.Communication.Requests;
using DematecStock.Communication.Responses;

namespace DematecStock.Application.UseCases.Login.DoLogin
{
    public interface IDoLoginUseCase
    {
        Task<ResponseLoginUserJson> Execute(RequestLoginJson request);
    }
}
