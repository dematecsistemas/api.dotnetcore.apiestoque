using DematecStock.Application.UseCases.ProductsAddress.GetAllStorageLocationsByProduct;
using DematecStock.Application.UseCases.ProductsAddress.GetAllStorageProductsByLocation;
using DematecStock.Communication.Responses;
using Microsoft.AspNetCore.Mvc;

namespace DematecStock.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsLocations : ControllerBase
    {

        // GET: api/ProductsLocations/{idLocation}
        // Retorna os produtos armazenados em uma localização
        [HttpGet("{idLocation}")]
        [ProducesResponseType(typeof(ResponseLocationProduct), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStoregeProductsByLocation(
        [FromServices] IGetAllStorageProductsByLocationUseCase getAllStorageProductsByLocationUseCase,
        [FromRoute] int idLocation)
        {
            var locationWithStoregeProduct = await getAllStorageProductsByLocationUseCase.Execute(idLocation);

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
            [FromQuery] string? ean13code)
        {
            if (idProduct is <= 0)
                return BadRequest(new ResponseErrorJson("idProduct deve ser maior que zero."));

            reference = reference?.Trim();
            ean13code = ean13code?.Trim();

            if (idProduct is null && string.IsNullOrWhiteSpace(reference) && string.IsNullOrWhiteSpace(ean13code))
                return BadRequest(new ResponseErrorJson("Informe ao menos um filtro: idProduct, reference ou ean13code."));

            var result = await getAllLocationsByProductUseCase.Execute(idProduct, reference, ean13code);
            return Ok(result);
        }
    }
}
