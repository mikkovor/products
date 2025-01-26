using Microsoft.AspNetCore.Mvc;
using Products.Infrastructure;
using Products.Models.Requests;
using Products.Models.Responses;

namespace Products.Controllers
{
    [ApiController]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly ILogger<ProductsController> _logger;
        private readonly ProductService _productService;

        public ProductsController(ILogger<ProductsController> logger, ProductService productService)
        {
            _logger = logger;
            _productService = productService;
        }

        [HttpGet(Name = "Get")]
        public async Task<ActionResult<List<ViewProduct>>> Get([FromQuery] ProductsRequest productsRequest)
        {
            return await _productService.Get(productsRequest);
        }

        [HttpGet("{id:int}", Name = "GetById")]
        public async Task<ActionResult<ViewProduct>> GetById([FromRoute] int id)
        {
            var product = await _productService.GetById(id);
            return product == null ? NotFound() : Ok(product);
        }

        [HttpPost(Name = "Create")]
        public async Task<ActionResult<ViewProduct>> Create([FromBody] CreateProduct createProduct)
        {
            var product = await _productService.Create(createProduct);
            return CreatedAtRoute(nameof(GetById), new { id = product.Id }, product);
        }

        [HttpDelete("{id:int}", Name = "Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var isDeleted = await _productService.Delete(id);
            return isDeleted ? NoContent() : NotFound();
        }
    }
}