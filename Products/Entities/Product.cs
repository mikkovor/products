using System.Diagnostics.CodeAnalysis;

namespace Products.Entities
{
    public class Product
    {
        private Product()
        {
        }

        [SetsRequiredMembers]
        public Product(string name, decimal price)
        {
            Name = name;
            Price = price;
        }

        public int Id { get; set; }
        public required string Name { get; set; }
        public decimal Price { get; set; }
    }
}