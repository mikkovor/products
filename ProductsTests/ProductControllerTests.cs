using FluentAssertions;
using Products.Dtos;
using Products.IntegrationTests;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

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
            var content = new StringContent(
                JsonSerializer.Serialize(createProduct),
                Encoding.UTF8,
                "application/json");

            var response = await _client.PostAsync("/api/products", content);
            var product = await response.Content.ReadFromJsonAsync<ViewProduct>();

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Headers.Location!.ToString().EndsWith($"/api/products/{product!.Id}").Should().BeTrue();
            product.Should().NotBeNull();
            product.Name.Should().Be("Test Product");
            product.Price.Should().Be(99.99m);
        }
    }
}