using Microsoft.AspNetCore.Mvc;

namespace Products.Controllers
{
    [ApiController]
    [Route("products")]
    public class ProductsController : ControllerBase
    {
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(ILogger<ProductsController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "Get")]
        public Task<ActionResult<>> Get()
        {
        }

        [HttpGet(Name = "GetById")]
        [Route("/{id:int}")]
        public Task<ActionResult<>> GetById(int id)
        {
        }

        [HttpPost(Name = "Create")]
        public Task<ActionResult<>> Create(int id)
        {
        }

        [HttpDelete(Name = "Delete")]
        public Task<ActionResult<>> Delete(int id)
        {
        }
    }
}