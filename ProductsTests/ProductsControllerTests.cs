using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.DependencyInjection;
using Products.Infrastructure.Persistence;
using Products.Models.Requests;
using Products.Models.Responses;
using System.Net;
using System.Net.Http.Json;

namespace Products.IntegrationTests
{
    public class ProductsControllerTests : IClassFixture<CustomWebApplicationFactory<Program>>, IAsyncLifetime
    {
        private readonly HttpClient _client;
        private readonly ProductDbContext _context;
        private const string defaultProductName = "Test Product";
        private const decimal defaultProductPrice = 99.99m;

        public ProductsControllerTests(CustomWebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
            _context = factory.Services.CreateScope()
                .ServiceProvider.GetRequiredService<ProductDbContext>();
        }

        public async Task InitializeAsync()
        {
            await _context.Database.EnsureCreatedAsync();
        }

        public async Task DisposeAsync()
        {
            await _context.Database.EnsureDeletedAsync();
        }

        [Fact]
        public async Task Create_ShouldReturnCreatedProduct()
        {
            var createProduct = new CreateProduct(defaultProductName, defaultProductPrice);
            var response = await _client.PostAsJsonAsync("/api/products", createProduct);
            var product = await response.Content.ReadFromJsonAsync<ViewProduct>();

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Headers.Location!.ToString().EndsWith($"/api/products/{product!.Id}").Should().BeTrue();

            product.Should().NotBeNull();
            product.Id.Should().BeGreaterThan(0);
            product.Name.Should().Be(defaultProductName);
            product.Price.Should().Be(defaultProductPrice);
        }

        [Fact]
        public async Task GetById_ShouldReturnProductById()
        {
            var product = await CreateTestProduct();
            product.Should().NotBeNull();

            var foundProduct = await _client.GetFromJsonAsync<ViewProduct>($"/api/products/{product!.Id}");
            foundProduct.Should().NotBeNull();
            foundProduct!.Id.Should().Be(product.Id);
            foundProduct.Name.Should().Be(defaultProductName);
            foundProduct.Price.Should().Be(defaultProductPrice);
        }

        [Fact]
        public async Task GetById_ShouldReturnNotFoundWhenProductIsNotFound()
        {
            var product = await CreateTestProduct();
            product.Should().NotBeNull();

            var notFound = await _client.GetAsync($"/api/products/{product!.Id + 1}");
            notFound.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Delete_ShouldReturn204StatusCodeWhenDeleted()
        {
            var product = await CreateTestProduct();
            product.Should().NotBeNull();

            var deletedResponse = await _client.DeleteAsync($"/api/products/{product!.Id}");
            deletedResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var notFound = await _client.GetAsync($"/api/products/{product!.Id}");
            notFound.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Delete_ShouldReturnNotFoundWhenRequestedProductToBeDeletedItNotFound()
        {
            var product = await CreateTestProduct();
            product.Should().NotBeNull();

            var deletedResponse = await _client.DeleteAsync($"/api/products/{product!.Id + 1}");
            deletedResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);

            var foundProduct = await _client.GetFromJsonAsync<ViewProduct>($"/api/products/{product!.Id}");
            foundProduct.Should().NotBeNull();
            foundProduct!.Id.Should().Be(product.Id);
            foundProduct.Name.Should().Be(defaultProductName);
            foundProduct.Price.Should().Be(defaultProductPrice);
        }

        [Fact]
        public async Task Get_ShouldListAllProducts()
        {
            var createProduct1 = await CreateTestProduct();

            var createProduct2 = await CreateTestProduct("Another Product", 10.99m);

            var products = await _client.GetFromJsonAsync<List<ViewProduct>>("/api/products");

            products.Should().NotBeNull();
            products!.Count.Should().Be(2);
            products.Any(x => x.Name == createProduct1!.Name && x.Price == createProduct1.Price && x.Id > 0).Should().BeTrue();
            products.Any(x => x.Name == createProduct2!.Name && x.Price == createProduct2.Price && x.Id > 0).Should().BeTrue();
        }

        [Fact]
        public async Task Get_ShouldListAllProductsOrderedByName()
        {
            var productsRequest = new ProductsRequest("Name");

            await CreateTestProduct();
            await CreateTestProduct("Another Product", 10.99m);

            var products = await _client.GetFromJsonAsync<List<ViewProduct>>(CreateUriWithQueryParameters(productsRequest, "/api/products"));

            products.Should().NotBeNull();
            products!.Count.Should().Be(2);
            products[0].Name.Should().Be("Another Product");
            products[1].Name.Should().Be(defaultProductName);
        }

        [Fact]
        public async Task Get_ShouldListAllProductsOrderedByPrice()
        {
            var productsRequest = new ProductsRequest("Price");

            await CreateTestProduct();
            await CreateTestProduct("Another Product", 10.99m);

            var products = await _client.GetFromJsonAsync<List<ViewProduct>>(CreateUriWithQueryParameters(productsRequest, "/api/products"));

            products.Should().NotBeNull();
            products!.Count.Should().Be(2);
            products[0].Price.Should().Be(10.99m);
            products[1].Price.Should().Be(defaultProductPrice);
        }

        [Fact]
        public async Task Get_ShouldAllowSortingOnlyByKnownProperties()
        {
            var productsRequest = new ProductsRequest("lastName");

            await CreateTestProduct();
            await CreateTestProduct("Another Product", 10.99m);

            var response = await _client.GetAsync(CreateUriWithQueryParameters(productsRequest, "/api/products"));
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

            var problemDetails = await response.Content.ReadFromJsonAsync<ProblemDetails>();
            problemDetails.Should().NotBeNull();
            problemDetails!.Status.Should().Be((int)HttpStatusCode.BadRequest);
            problemDetails.Title.Should().Be("Invalid argument");
            problemDetails.Detail.Should().Be($"Cannot sort products by {productsRequest.SortBy}");
        }

        private static string CreateUriWithQueryParameters<T>(T values, string uri) where T : class
        {
            var props = values!.GetType().GetProperties();
            var query = props.ToDictionary(
                p => p.Name.ToLower(),
                p => p.GetValue(values)?.ToString()
            );

            return QueryHelpers.AddQueryString(uri, query);
        }

        private async Task<ViewProduct?> CreateTestProduct(string name = defaultProductName, decimal price = defaultProductPrice)
        {
            var response = await _client.PostAsJsonAsync("/api/products", new CreateProduct(name, price));
            return await response.Content.ReadFromJsonAsync<ViewProduct>();
        }
    }
}