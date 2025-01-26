using Microsoft.EntityFrameworkCore;
using Products.Entities;
using Products.Extensions.Mapping;
using Products.Infrastructure.Persistence;
using Products.Models.Requests;
using Products.Models.Responses;

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

        public async Task<ViewProduct?> GetById(int id)
        {
            var product = await productDbContext.Products.FirstOrDefaultAsync(x => x.Id == id);
            await productDbContext.SaveChangesAsync();
            return product?.ToViewProduct();
        }

        public async Task<bool> Delete(int id)
        {
            var product = await productDbContext.Products.FirstOrDefaultAsync(x => x.Id == id);
            if (product == null) return false;

            productDbContext.Products.Remove(product);
            await productDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<List<ViewProduct>> Get(ProductsRequest productsRequest)
        {
            var query = productDbContext.Products.AsQueryable();

            query = productsRequest.SortBy.ToLower() switch
            {
                // ProductsRequest defaults to name
                "name" => productsRequest.SortDescending
                    ? query.OrderByDescending(p => p.Name)
                    : query.OrderBy(p => p.Name),
                "price" => productsRequest.SortDescending
                    ? query.OrderByDescending(p => p.Price)
                    : query.OrderBy(p => p.Price),
                _ => throw new ArgumentException($"Cannot sort products by {productsRequest.SortBy}")
            };

            var products = await query.ToListAsync();

            List<ViewProduct> viewProducts = [];

            foreach (var product in products)
            {
                viewProducts.Add(product.ToViewProduct());
            }

            return viewProducts;
        }
    }
}