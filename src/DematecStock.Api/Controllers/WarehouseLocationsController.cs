using Azure;
using DematecStock.Application.UseCases.WarehouseLocations.CreateLocation;
using DematecStock.Application.UseCases.WarehouseLocations.GetAllLocations;
using DematecStock.Application.UseCases.WarehouseLocations.UpdateLocation;
using DematecStock.Communication.Requests.WarehouseLocations;
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

        [HttpPost]
        [ProducesResponseType(typeof(ResponseLocationsJson), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create(
            [FromServices] ICreateLocationUseCase useCase,
            [FromBody] RequestWriteWarehouseLocationJson request)
        {
            await useCase.Execute(request);
            return Ok();
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(ResponseLocationsJson), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(
            [FromServices] IUpdateWarehouseLocationUseCase useCase,
            [FromRoute] int id,
            [FromBody] RequestUpdateWarehouseLocationJson request)
        {
            var response = await useCase.Execute(id, request);
            return Ok(response);
        }
    }
}
