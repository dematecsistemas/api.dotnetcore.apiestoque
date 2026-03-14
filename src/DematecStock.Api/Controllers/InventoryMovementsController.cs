using DematecStock.Application.UseCases.InventoryMovement;
using DematecStock.Communication.Requests.InventoryMovement;
using DematecStock.Communication.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DematecStock.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class InventoryMovementsController : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Add(
            [FromServices] IAddInventoryMovementsUseCase useCase,
            [FromBody] RequestAddInventoryMovementJsons request)
        {

            await useCase.Execute(request);
            return StatusCode(StatusCodes.Status201Created);
        }
    }
}
