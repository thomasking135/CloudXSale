using System;
using CloudXSale.Models;
using CloudXSale.Services;
using Xunit;

namespace CloudXSale.Tests
{
    // Tests the BasketOptimise service (auto-pick logic)
    public class OptimiseTests
    {
        [Fact]
        public void Finds_Best_Combo_Closest_To_200_With_Min_5_Items()
        {
            // Catalog designed so an exact 200 subtotal with 5 items exists
            var catalog = new Product[]
            {
                new Product(1,"A",10m), new Product(2,"B",20m), new Product(3,"C",30m),
                new Product(4,"D",40m), new Product(5,"E",50m), new Product(6,"F",60m),
                new Product(7,"G",70m)
            };

            var basket = BasketOptimise.FindBest(catalog, 5, 200m);

            Assert.True(basket.Items.Count >= 5);
            Assert.True(basket.Subtotal >= 200m);
            Assert.Equal(200m, basket.Subtotal); // exact match
            Assert.True(basket.EligibleForDiscount);
            Assert.Equal(150m, basket.Total);    // 200 - 50
        }

        [Fact]
        public void Returns_Empty_When_No_Combo_Meets_Rules()
        {
            // With these prices, no 5-item combo can reach 200
            var catalog = new Product[]
            {
                new Product(1,"A",33m), new Product(2,"B",33m),
                new Product(3,"C",33m), new Product(4,"D",33m),
                new Product(5,"E",33m)
            };

            var basket = BasketOptimise.FindBest(catalog, 5, 200m);

            Assert.Empty(basket.Items);
            Assert.False(basket.EligibleForDiscount);
        }
    }
}
