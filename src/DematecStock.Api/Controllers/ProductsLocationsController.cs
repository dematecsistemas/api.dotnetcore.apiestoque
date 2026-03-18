using DematecStock.Application.UseCases.ProductsAddress.GetAllStorageLocationsByProduct;
using DematecStock.Application.UseCases.ProductsAddress.GetAllStorageProductsByLocation;
using DematecStock.Application.UseCases.ProductsAddress.GetStorageProductsByLocationQuery;
using DematecStock.Communication.Responses;
using Microsoft.AspNetCore.Mvc;

namespace DematecStock.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsLocationsController : ControllerBase
    {

        // GET: api/ProductsLocations/{idLocation}
        // Retorna os produtos armazenados em uma localização
        [HttpGet("{idLocation}")]
        [ProducesResponseType(typeof(ResponseLocationProduct), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStoregeProductsByLocation(
        [FromServices] IGetAllStorageProductsByLocationUseCase getAllStorageProductsByLocationUseCase,
        [FromRoute] int idLocation,
        [FromQuery] string? isProductInactive)
        {
            if (IsInvalidSN(isProductInactive)) return BadRequest(new ResponseErrorJson("isProductInactive deve ser S ou N."));

            var locationWithStoregeProduct = await getAllStorageProductsByLocationUseCase.Execute(idLocation, isProductInactive);

            return Ok(locationWithStoregeProduct);
        }


        // GET: api/ProductsLocations/search
        // Retorna as localizações onde o produto está armazenad
        [HttpGet("search")]
        [ProducesResponseType(typeof(ResponseProductLocations), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Search(
            [FromServices] IGetAllLocationsByProductUseCase getAllLocationsByProductUseCase,
            [FromQuery] int? idProduct,
            [FromQuery] string? reference,
            [FromQuery] string? ean13code,
            [FromQuery] string? isActive,
            [FromQuery] string? isMovementAllowed,
            [FromQuery] string? isAllowReplenishment,
            [FromQuery] string? isPickingLocation,
            [FromQuery] string? isProductInactive)
        {
            if (idProduct is <= 0)
                return BadRequest(new ResponseErrorJson("idProduct deve ser maior que zero."));

            reference = reference?.Trim();
            ean13code = ean13code?.Trim();

            if (idProduct is null && string.IsNullOrWhiteSpace(reference) && string.IsNullOrWhiteSpace(ean13code))
                return BadRequest(new ResponseErrorJson("Informe ao menos um filtro: idProduct, reference ou ean13code."));

            if (IsInvalidSN(isActive)) return BadRequest(new ResponseErrorJson("isActive deve ser S ou N."));
            if (IsInvalidSN(isMovementAllowed)) return BadRequest(new ResponseErrorJson("isMovementAllowed deve ser S ou N."));
            if (IsInvalidSN(isAllowReplenishment)) return BadRequest(new ResponseErrorJson("isAllowReplenishment deve ser S ou N."));
            if (IsInvalidSN(isPickingLocation)) return BadRequest(new ResponseErrorJson("isPickingLocation deve ser S ou N."));
            if (IsInvalidSN(isProductInactive)) return BadRequest(new ResponseErrorJson("isProductInactive deve ser S ou N."));

            var result = await getAllLocationsByProductUseCase.Execute(idProduct, reference, ean13code, isActive, isMovementAllowed, isAllowReplenishment, isPickingLocation, isProductInactive);
            return Ok(result);
        }


        // GET: api/ProductsLocations/find/{query}
        // Busca localizações por idLocation ou por parte do LocationName
        [HttpGet("find/{query}")]
        [ProducesResponseType(typeof(List<ResponseLocationProduct>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> FindByLocationQuery(
            [FromServices] IGetStorageProductsByLocationQueryUseCase getStorageProductsByLocationQueryUseCase,
            [FromRoute] string query,
            [FromQuery] string? isActive,
            [FromQuery] string? isMovementAllowed,
            [FromQuery] string? isAllowReplenishment,
            [FromQuery] string? isPickingLocation,
            [FromQuery] string? isProductInactive)
        {
            query = query.Trim();

            if (string.IsNullOrWhiteSpace(query))
                return BadRequest(new ResponseErrorJson("Informe um valor para busca."));

            if (IsInvalidSN(isActive)) return BadRequest(new ResponseErrorJson("isActive deve ser S ou N."));
            if (IsInvalidSN(isMovementAllowed)) return BadRequest(new ResponseErrorJson("isMovementAllowed deve ser S ou N."));
            if (IsInvalidSN(isAllowReplenishment)) return BadRequest(new ResponseErrorJson("isAllowReplenishment deve ser S ou N."));
            if (IsInvalidSN(isPickingLocation)) return BadRequest(new ResponseErrorJson("isPickingLocation deve ser S ou N."));
            if (IsInvalidSN(isProductInactive)) return BadRequest(new ResponseErrorJson("isProductInactive deve ser S ou N."));

            var result = await getStorageProductsByLocationQueryUseCase.Execute(query, isActive, isMovementAllowed, isAllowReplenishment, isPickingLocation, isProductInactive);
            return Ok(result);
        }

        private static bool IsInvalidSN(string? value) => value is not null && value != "S" && value != "N";
    }
}
