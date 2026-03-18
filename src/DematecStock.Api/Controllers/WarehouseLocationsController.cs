using DematecStock.Application.UseCases.WarehouseLocations.CreateLocation;
using DematecStock.Application.UseCases.WarehouseLocations.GetAllLocations;
using DematecStock.Application.UseCases.WarehouseLocations.PatchLocation;
using DematecStock.Application.UseCases.WarehouseLocations.SearchLocationsByName;
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
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetLocationWithProducts(
        [FromServices] IGetAllLocationsUseCase getAllLocationsUseCase,
        [FromQuery] string? isActive,
        [FromQuery] string? isMovementAllowed,
        [FromQuery] string? isAllowReplenishment,
        [FromQuery] string? isPickingLocation)
        {
            if (IsInvalidSN(isActive)) return BadRequest(new ResponseErrorJson("isActive deve ser S ou N."));
            if (IsInvalidSN(isMovementAllowed)) return BadRequest(new ResponseErrorJson("isMovementAllowed deve ser S ou N."));
            if (IsInvalidSN(isAllowReplenishment)) return BadRequest(new ResponseErrorJson("isAllowReplenishment deve ser S ou N."));
            if (IsInvalidSN(isPickingLocation)) return BadRequest(new ResponseErrorJson("isPickingLocation deve ser S ou N."));

            var location = await getAllLocationsUseCase.Execute(isActive, isMovementAllowed, isAllowReplenishment, isPickingLocation);

            return Ok(location);
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(List<ResponseLocationsJson>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SearchByLocationName(
            [FromServices] ISearchLocationsByNameUseCase useCase,
            [FromQuery] string query,
            [FromQuery] string? isActive,
            [FromQuery] string? isMovementAllowed,
            [FromQuery] string? isAllowReplenishment,
            [FromQuery] string? isPickingLocation)
        {
            query = query?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(query))
                return BadRequest(new ResponseErrorJson("Informe parte do nome do endereço para busca."));

            if (IsInvalidSN(isActive)) return BadRequest(new ResponseErrorJson("isActive deve ser S ou N."));
            if (IsInvalidSN(isMovementAllowed)) return BadRequest(new ResponseErrorJson("isMovementAllowed deve ser S ou N."));
            if (IsInvalidSN(isAllowReplenishment)) return BadRequest(new ResponseErrorJson("isAllowReplenishment deve ser S ou N."));
            if (IsInvalidSN(isPickingLocation)) return BadRequest(new ResponseErrorJson("isPickingLocation deve ser S ou N."));

            var result = await useCase.Execute(query, isActive, isMovementAllowed, isAllowReplenishment, isPickingLocation);
            return Ok(result);
        }

        private static bool IsInvalidSN(string? value) => value is not null && value != "S" && value != "N";

        [HttpPost]
        [ProducesResponseType(typeof(ResponseLocationsJson), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create(
            [FromServices] ICreateLocationUseCase useCase,
            [FromBody] RequestWriteWarehouseLocationJson request)
        {
            await useCase.Execute(request);
            return StatusCode(StatusCodes.Status201Created);
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(typeof(ResponseLocationsJson), StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Patch(
            [FromServices] IPatchWarehouseLocationUseCase useCase,
            [FromRoute] int id,
            [FromBody] PatchWarehouseLocationInput input)
        {
            await useCase.ExecuteAsync(id, input);
            return NoContent();
        }
    }
}
