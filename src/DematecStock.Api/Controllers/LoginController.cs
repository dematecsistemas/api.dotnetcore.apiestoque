using DematecStock.Application.UseCases.Login.DoLogin;
using DematecStock.Communication.Requests;
using DematecStock.Communication.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DematecStock.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(typeof(ResponseLoginUserJson), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login(
            [FromServices] IDoLoginUseCase doLoginUseCase,
            [FromBody] RequestLoginJson request
        )
        {
            var response = await doLoginUseCase.Execute(request);   
            return Ok(response);
        }
    }
}
