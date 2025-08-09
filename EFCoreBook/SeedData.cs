using EFCoreBook.Models;

namespace EFCoreBook
{
    public static class SeedData
    {
         public static List<Customer> GenerateCustomers(
            int customers = 50,
            int ordersPerCustomer = 5,
            int itemsPerOrder = 5,
            int randomSeed = 12345)
        {
            var rng = new Random(randomSeed);

            var productNames = new[]
            {
                "Pen", "Notebook", "Mouse", "Keyboard", "Monitor",
                "Chair", "Table", "Lamp", "Phone", "USB Cable"
            };

            var customersList = new List<Customer>();

            for (int i = 1; i <= customers; i++)
            {
                var customer = new Customer
                {
                    // Id را نگذاریم تا وقتی خواستیم به DbContext اضافه کنیم، EF خودش مقداردهی کند.
                    Name = $"Customer {i}"
                };

                for (int o = 1; o <= ordersPerCustomer; o++)
                {
                    var order = new Order
                    {
                        OrderDate = DateTime.UtcNow.AddDays(-rng.Next(0, 365)),
                        Customer = customer // navigation property را ست می‌کنیم
                    };

                    for (int it = 1; it <= itemsPerOrder; it++)
                    {
                        var item = new Item
                        {
                            ProductName = productNames[rng.Next(productNames.Length)],
                            Price = Math.Round((decimal)(rng.NextDouble() * 100), 2),
                            Order = order
                        };
                        order.Items.Add(item);
                    }

                    customer.Orders.Add(order);
                }

                customersList.Add(customer);
            }

            return customersList;
        }
    }
}