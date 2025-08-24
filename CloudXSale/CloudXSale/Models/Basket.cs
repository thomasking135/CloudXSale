using System;
using System.Collections.Generic;
using System.Linq;

namespace CloudXSale.Models
{
    // Represents a shopping basket (collection of products)
    public sealed class Basket
    {
        // Business rules (constants)
        public const int MinItemsForDiscount = 5;
        public const decimal Threshold = 200m;
        public const decimal DiscountAmount = 50m;

        // List of products in the basket
        public List<Product> Items { get; } = new List<Product>();

        // Subtotal = sum of all product prices
        public decimal Subtotal { get { return Items.Sum(p => p.Price); } }

        // Rule checks
        public bool MeetsQuantityRule { get { return Items.Count >= MinItemsForDiscount; } }
        public bool MeetsSpendRule { get { return Subtotal >= Threshold; } }

        // A basket qualifies only if both rules are met
        public bool EligibleForDiscount { get { return MeetsQuantityRule && MeetsSpendRule; } }

        // Discount amount (either $50 or $0)
        public decimal Discount { get { return EligibleForDiscount ? DiscountAmount : 0m; } }

        // Final total after applying discount
        public decimal Total { get { return Subtotal - Discount; } }
    }
}
