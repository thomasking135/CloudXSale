using System;
using CloudXSale.Models;
using Xunit;

namespace CloudXSale.Tests
{
    // Tests the Basket class rules and discount logic
    public class BasketTests
    {
        [Fact]
        public void Discount_Applies_When_Rules_Met()
        {
            // Arrange: 5 items worth $50 each (subtotal 250)
            var b = new Basket();
            for (int i = 0; i < Basket.MinItemsForDiscount; i++)
            {
                b.Items.Add(new Product(i + 1, "P" + (i + 1), 50m));
            }

            // Assert rules and maths
            Assert.True(b.MeetsQuantityRule);
            Assert.True(b.MeetsSpendRule);
            Assert.True(b.EligibleForDiscount);
            Assert.Equal(250m, b.Subtotal);
            Assert.Equal(Basket.DiscountAmount, b.Discount);
            Assert.Equal(200m, b.Total); // 250 - 50
        }

        [Fact]
        public void No_Discount_When_Spend_Too_Low()
        {
            // Arrange: 5 items worth $30 each (subtotal 150 < 200)
            var b = new Basket();
            for (int i = 0; i < Basket.MinItemsForDiscount; i++)
            {
                b.Items.Add(new Product(i + 1, "P" + (i + 1), 30m));
            }

            // Assert: discount not applied
            Assert.False(b.MeetsSpendRule);
            Assert.False(b.EligibleForDiscount);
            Assert.Equal(150m, b.Total);
        }
    }
}
