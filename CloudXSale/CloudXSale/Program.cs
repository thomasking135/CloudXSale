using System;
using System.Collections.Generic;
using System.Linq;
using CloudXSale.Models;
using CloudXSale.Services;

namespace CloudXSale
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var catalog = new List<Product>
            {
                new Product(1, "Running Socks", 12.99m),
                new Product(2, "Water Bottle", 15.49m),
                new Product(3, "Gym Towel", 19.99m),
                new Product(4, "Skipping Rope", 24.99m),
                new Product(5, "Cap", 26.50m),
                new Product(6, "Shin Guards", 29.99m),
                new Product(7, "Yoga Mat", 39.99m),
                new Product(8, "Cycling Gloves", 34.99m),
                new Product(9, "Compression Tee", 44.99m),
                new Product(10, "Football", 49.99m),
                new Product(11, "Tennis Racket", 79.99m),
                new Product(12, "Day Pack", 59.99m),
            };

            while (true)
            {
                Console.Clear();
                RunOnce(catalog);

                // Restart/quit validation loop (Q/Esc exits immediately)
                Console.WriteLine();
                Console.WriteLine("Press R to run again, or Q to quit:");
                while (true)
                {
                    var key = Console.ReadKey(intercept: true).Key;
                    if (key == ConsoleKey.R) break; // restart
                    if (key == ConsoleKey.Q || key == ConsoleKey.Escape) Environment.Exit(0);
                    Console.WriteLine("\n❌ Wrong key pressed. Please press R to run again, or Q to quit:");
                }
            }
        }

        /// <summary>
        /// Executes one full shopping/discount calculation cycle.
        /// </summary>
        private static void RunOnce(List<Product> catalog)
        {
            Console.WriteLine("=== CloudX Black Friday Optimiser ===\n");
            Console.WriteLine("Rules: buy at least 5 items AND spend at least $200 to get $50 off.\n");

            Console.WriteLine("Catalog:");
            foreach (var p in catalog) Console.WriteLine(p);
            Console.WriteLine();

            // Menu validation: only accept "1" or "2"
            string choice = string.Empty;
            while (choice != "1" && choice != "2")
            {
                Console.WriteLine("Choose an option:");
                Console.WriteLine("1) Auto-pick best combo (closest to $200 with ≥5 items)");
                Console.WriteLine("2) Manual selection by IDs (comma-separated)");
                Console.Write("> ");
                choice = (Console.ReadLine() ?? "").Trim();
                if (choice != "1" && choice != "2")
                    Console.WriteLine("\n❌ Invalid choice. Please enter 1 or 2.\n");
            }

            Basket basket;

            if (choice == "2")
            {
                // Manual selection with robust parsing + feedback
                Console.Write("Enter product IDs separated by commas (e.g., 1,3,5,7,9): ");
                var input = Console.ReadLine() ?? "";

                var tokens = input.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                basket = new Basket();

                var invalidTokens = new List<string>();  // non-numeric entries like 'r'
                var notFoundIds = new List<int>();     // numeric but not in catalog (e.g., 44)
                var duplicateIds = new List<int>();     // same ID entered twice
                var added = new HashSet<int>();  // track duplicates

                foreach (var tok in tokens)
                {
                    var trimmed = tok.Trim();

                    int id;
                    if (!int.TryParse(trimmed, out id))
                    {
                        invalidTokens.Add(trimmed);
                        continue;
                    }

                    if (added.Contains(id))
                    {
                        duplicateIds.Add(id);
                        continue;
                    }

                    var prod = catalog.FirstOrDefault(p => p.Id == id);
                    if (prod == null)
                    {
                        notFoundIds.Add(id);
                        continue;
                    }

                    basket.Items.Add(prod);
                    added.Add(id);
                }

                // Report ignored inputs
                if (invalidTokens.Count > 0 || notFoundIds.Count > 0 || duplicateIds.Count > 0)
                {
                    Console.WriteLine();
                    Console.WriteLine("Note:");
                    if (notFoundIds.Count > 0)
                        Console.WriteLine("- Ignored: IDs not found → {0}", string.Join(", ", notFoundIds));
                    if (invalidTokens.Count > 0)
                        Console.WriteLine("- Ignored: invalid tokens → {0}", string.Join(", ", invalidTokens));
                    if (duplicateIds.Count > 0)
                        Console.WriteLine("- Ignored: duplicate IDs → {0}", string.Join(", ", duplicateIds));
                }

                // If none valid, fall back to auto-pick (and say so)
                if (basket.Items.Count == 0)
                {
                    Console.WriteLine("\nNo valid items remained after filtering; using auto-pick.\n");
                    basket = BasketOptimise.FindBest(catalog);
                }
            }
            else
            {
                // Auto-pick
                basket = BasketOptimise.FindBest(catalog);
            }

            // Results
            Console.WriteLine("\nYour Basket:");
            foreach (var p in basket.Items) Console.WriteLine(p);
            Console.WriteLine("\nSubtotal: ${0:F2}", basket.Subtotal);
            Console.WriteLine("Eligible (≥{0} items & ≥${1:F0})? {2}",
                Basket.MinItemsForDiscount, Basket.Threshold, basket.EligibleForDiscount);
            Console.WriteLine("Discount: -${0:F2}", basket.Discount);
            Console.WriteLine("Total to pay: ${0:F2}", basket.Total);

            if (!basket.EligibleForDiscount)
            {
                int neededQty = Math.Max(0, Basket.MinItemsForDiscount - basket.Items.Count);
                decimal neededSpend = Math.Max(0m, Basket.Threshold - basket.Subtotal);
                Console.WriteLine("\nTips to qualify:");
                if (neededQty > 0) Console.WriteLine("- Add at least {0} more item(s).", neededQty);
                if (neededSpend > 0) Console.WriteLine("- Add about ${0:F2} more in value.", neededSpend);
            }

            Console.WriteLine("\nThanks for shopping with CloudX!");
        }
    }
}
