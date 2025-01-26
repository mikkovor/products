using Products.Entities;
using Products.Models.Responses;

namespace Products.Extensions.Mapping
{
    public static class ProductMappers
    {
        public static ViewProduct ToViewProduct(this Product product)
        {
            return new ViewProduct(product.Id, product.Name, product.Price);
        }
    }
}