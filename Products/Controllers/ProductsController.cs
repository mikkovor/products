using Microsoft.AspNetCore.Mvc;
using Products.Infrastructure;
using Products.Models.Requests;
using Products.Models.Responses;

namespace Products.Controllers
{
    [ApiController]
    [Tags("Products")]
    [Produces("application/json")]
    [Route("api/products")]
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductsController(ProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Retrieves a list of products with optional sorting
        /// </summary>
        /// <param name="productsRequest">Query parameters for sorting the results</param>
        /// <returns>List of products</returns>
        /// <response code="200">Returns the list of products</response>
        [ProducesResponseType(typeof(List<ViewProduct>), StatusCodes.Status200OK)]
        [HttpGet(Name = "Get")]
        public async Task<ActionResult<List<ViewProduct>>> Get([FromQuery] ProductsRequest productsRequest)
        {
            return await _productService.Get(productsRequest);
        }

        /// <summary>
        /// Retrieves a specific product by its ID
        /// </summary>
        /// <param name="id">The ID of the product to retrieve</param>
        /// <returns>The requested product</returns>
        /// <response code="200">Returns the requested product</response>
        /// <response code="404">Product not found</response>
        [ProducesResponseType(typeof(ViewProduct), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("{id:int}", Name = "GetById")]
        public async Task<ActionResult<ViewProduct>> GetById([FromRoute] int id)
        {
            var product = await _productService.GetById(id);
            return product == null ? NotFound() : Ok(product);
        }

        /// <summary>
        /// Creates a new product
        /// </summary>
        /// <param name="createProduct">The product information</param>
        /// <returns>The newly created product</returns>
        /// <response code="201">Returns the newly created product</response>
        /// <response code="400">If the product information is invalid</response>
        [ProducesResponseType(typeof(ViewProduct), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost(Name = "Create")]
        public async Task<ActionResult<ViewProduct>> Create([FromBody] CreateProduct createProduct)
        {
            var product = await _productService.Create(createProduct);
            return CreatedAtRoute(nameof(GetById), new { id = product.Id }, product);
        }

        /// <summary>
        /// Deletes a specific product
        /// </summary>
        /// <param name="id">The ID of the product to delete</param>
        /// <response code="204">Product successfully deleted</response>
        /// <response code="404">Product not found</response>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpDelete("{id:int}", Name = "Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var isDeleted = await _productService.Delete(id);
            return isDeleted ? NoContent() : NotFound();
        }
    }
}