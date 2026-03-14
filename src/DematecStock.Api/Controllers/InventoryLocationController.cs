using DematecStock.Application.UseCases.InventoryLocation.AddInventoryLocation;
using DematecStock.Application.UseCases.InventoryLocation.DeleteInventoryLocation;
using DematecStock.Application.UseCases.InventoryLocation.UpdateOnHandQuantity;
using DematecStock.Communication.Requests.InventoryLocation;
using DematecStock.Communication.Responses;
using Microsoft.AspNetCore.Mvc;

namespace DematecStock.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryLocationController : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Add(
            [FromServices] IAddInventoryLocationUseCase useCase,
            [FromBody] RequestAddInventoryLocationJson request)
        {
            await useCase.Execute(request);
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpDelete("{idLocation}/{idProduct}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(
            [FromServices] IDeleteInventoryLocationUseCase useCase,
            [FromRoute] int idLocation,
            [FromRoute] int idProduct)
        {
            await useCase.Execute(idLocation, idProduct);
            return NoContent();
        }

        [HttpPatch("{idLocation}/{idProduct}/saldo")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateOnHandQuantity(
            [FromServices] IUpdateOnHandQuantityUseCase useCase,
            [FromRoute] int idLocation,
            [FromRoute] int idProduct,
            [FromBody] RequestUpdateOnHandQuantityJson request)
        {
            await useCase.Execute(idLocation, idProduct, request);
            return NoContent();
        }
    }
}
