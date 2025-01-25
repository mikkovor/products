using Products.Dtos;
using Products.Entities;
using Products.Extensions.Mapping;
using Products.Infrastructure.Persistence;

namespace Products.Infrastructure
{
    public class ProductService(ProductDbContext productDbContext)
    {
        public async Task<ViewProduct> Create(CreateProduct createProduct)
        {
            // missing validation either here or in the actual product class/entity
            var product = new Product(createProduct.Name, createProduct.Price);
            await productDbContext.Products.AddAsync(product);
            await productDbContext.SaveChangesAsync();
            return product.ToViewProduct();
        }
    }
}