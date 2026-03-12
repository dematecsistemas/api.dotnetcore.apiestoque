using DematecStock.Application.UseCases.ProductSearch.GetProductSearch;
using DematecStock.Communication.Responses;
using Microsoft.AspNetCore.Mvc;

namespace DematecStock.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductSearchController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(ResponseProductSearchPagedJson), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(
            [FromServices] IGetProductSearchUseCase useCase,
            [FromQuery] string? q,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken ct = default)
        {
            if (page <= 0)
                return BadRequest(new ResponseErrorJson("Pagina deve ser maior que zero."));

            if (pageSize <= 0)
                return BadRequest(new ResponseErrorJson("Pagina deve ser maior que zero."));

            var response = await useCase.Execute(q, page, pageSize, ct);
            return Ok(response);
        }
    }
}