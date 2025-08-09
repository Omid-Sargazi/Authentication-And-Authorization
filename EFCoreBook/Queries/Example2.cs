namespace EFCoreBook.Queries
{
    public class Queries
    {
        public static void Run()
        {
            var customers = SeedData.GenerateCustomers(customers: 100, ordersPerCustomer: 10, itemsPerOrder: 5);
            Console.WriteLine($"Customers,{customers.Count}");

            var totalOrders = customers.Sum(c => c.Orders.Count);
            var totalItems = customers.Sum(c => c.Orders.Sum(o => o.Items.Count));
            Console.WriteLine($"Orders: {totalOrders}, Items: {totalItems}");
        }
    }
}