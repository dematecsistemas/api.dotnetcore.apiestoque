using DematecStock.Application.UseCases.WarehouseLocations.GetAllLocations;
using DematecStock.Communication.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DematecStock.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WarehouseLocationsController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(List<ResponseLocationsJson>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetLocationWithProducts(
        [FromServices] IGetAllLocationsUseCase getAllLocationsUseCase)
        {
            var location = await getAllLocationsUseCase.Execute();

            return Ok(location);
        }

    }
}
