using FluentAssertions;
using Products.Dtos;
using Products.IntegrationTests;
using System.Net;
using System.Net.Http.Json;

namespace Products.Tests
{
    public class ProductsControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public ProductsControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Create_ShouldReturnCreatedProduct()
        {
            var createProduct = new CreateProduct("Test Product", 99.99m);
            var response = await _client.PostAsJsonAsync("/api/products", createProduct);
            var product = await response.Content.ReadFromJsonAsync<ViewProduct>();

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Headers.Location!.ToString().EndsWith($"/api/products/{product!.Id}").Should().BeTrue();

            product.Should().NotBeNull();
            product.Id.Should().BeGreaterThan(0);
            product.Name.Should().Be("Test Product");
            product.Price.Should().Be(99.99m);
        }

        [Fact]
        public async Task GetById_ShouldReturnProductById()
        {
            var createProduct = new CreateProduct("Test Product", 99.99m);
            var response = await _client.PostAsJsonAsync("/api/products", createProduct);

            var product = await response.Content.ReadFromJsonAsync<ViewProduct>();
            product.Should().NotBeNull();

            var foundProduct = await _client.GetFromJsonAsync<ViewProduct>($"/api/products/{product!.Id}");
            foundProduct.Should().NotBeNull();
            foundProduct!.Id.Should().Be(product.Id);
            foundProduct.Name.Should().Be("Test Product");
            foundProduct.Price.Should().Be(99.99m);
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFoundWhenProductIsNotFound()
        {
            var createProduct = new CreateProduct("Test Product", 99.99m);
            var response = await _client.PostAsJsonAsync("/api/products", createProduct);

            var product = await response.Content.ReadFromJsonAsync<ViewProduct>();
            product.Should().NotBeNull();

            var notFound = await _client.GetAsync($"/api/products/{product!.Id + 1}");
            notFound.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Delete_ShouldReturn204StatusCodeWhenDeleted()
        {
            var createProduct = new CreateProduct("Test Product", 99.99m);
            var response = await _client.PostAsJsonAsync("/api/products", createProduct);

            var product = await response.Content.ReadFromJsonAsync<ViewProduct>();
            product.Should().NotBeNull();

            var deletedResponse = await _client.DeleteAsync($"/api/products/{product!.Id}");
            deletedResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var notFound = await _client.GetAsync($"/api/products/{product!.Id}");
            notFound.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Delete_ShouldReturnNotFoundWhenRequestedProductToBeDeletedItNotFound()
        {
            var createProduct = new CreateProduct("Test Product", 99.99m);
            var response = await _client.PostAsJsonAsync("/api/products", createProduct);

            var product = await response.Content.ReadFromJsonAsync<ViewProduct>();
            product.Should().NotBeNull();

            var deletedResponse = await _client.DeleteAsync($"/api/products/{product!.Id + 1}");
            deletedResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);

            var foundProduct = await _client.GetFromJsonAsync<ViewProduct>($"/api/products/{product!.Id}");
            foundProduct.Should().NotBeNull();
            foundProduct!.Id.Should().Be(product.Id);
            foundProduct.Name.Should().Be("Test Product");
            foundProduct.Price.Should().Be(99.99m);
        }
    }
}