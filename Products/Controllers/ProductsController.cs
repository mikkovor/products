using Microsoft.AspNetCore.Mvc;
using Products.Dtos;
using Products.Infrastructure;

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
        public Task<IActionResult> Get()
        {
            throw new NotImplementedException();
        }

        [HttpGet(Name = "GetById")]
        [Route("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            return Ok(id);
        }

        [HttpPost(Name = "Create")]
        public async Task<ActionResult<ViewProduct>> Create([FromBody] CreateProduct createProduct)
        {
            var product = await _productService.Create(createProduct);
            return CreatedAtRoute(nameof(GetById), new { id = product.Id }, product);
        }

        [HttpDelete(Name = "Delete")]
        [Route("{id:int}")]
        public Task<IActionResult> Delete(int id)
        {
            throw new NotImplementedException();
        }
    }
}