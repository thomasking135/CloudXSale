using System;
using System.Collections.Generic;
using CloudXSale.Models;

namespace CloudXSale.Services
{
    /// <summary>
    /// Optimiser service: tries every possible combination of products
    /// and selects the basket that is >= threshold, has ≥ min items,
    /// and is closest to the threshold (minimises overshoot).
    /// </summary>
    public static class BasketOptimise
    {
        public static Basket FindBest(IList<Product> catalog,
                                      int minItems = Basket.MinItemsForDiscount,
                                      decimal threshold = Basket.Threshold)
        {
            if (catalog == null || catalog.Count == 0) return new Basket();

            Basket best = null;
            decimal bestOverage = decimal.MaxValue;
            decimal bestSubtotal = decimal.MaxValue;

            // Brute-force all subsets of products (2^N combinations)
            int combos = 1 << catalog.Count;

            for (int mask = 1; mask < combos; mask++)
            {
                var b = new Basket();

                for (int i = 0; i < catalog.Count; i++)
                {
                    if ((mask & (1 << i)) != 0) b.Items.Add(catalog[i]);
                }

                // Skip baskets that don't meet the rules
                if (b.Items.Count < minItems) continue;
                if (b.Subtotal < threshold) continue;

                decimal overage = b.Subtotal - threshold;

                bool better =
                    overage < bestOverage ||
                    (overage == bestOverage && b.Subtotal < bestSubtotal) ||
                    (overage == bestOverage && b.Subtotal == bestSubtotal &&
                        (best == null || b.Items.Count > best.Items.Count));

                if (better)
                {
                    best = b;
                    bestOverage = overage;
                    bestSubtotal = b.Subtotal;
                }
            }

            return best ?? new Basket(); // Return empty if no valid basket
        }
    }
}
