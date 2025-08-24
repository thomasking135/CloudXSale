using System;

namespace CloudXSale.Models
{
    // Represents a single product in the catalog
    public sealed class Product
    {
        public int Id { get; }          // Unique identifier for the product
        public string Name { get; }     // Product name/description
        public decimal Price { get; }   // Price of the product

        public Product(int id, string name, decimal price)
        {
            Id = id;
            Name = name;
            Price = price;
        }

        public override string ToString()
        {
            return $"#{Id} {Name} - ${Price:F2}";
        }
    }
}
