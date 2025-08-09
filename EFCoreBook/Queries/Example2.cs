using EFCoreBook.Models;
using Microsoft.EntityFrameworkCore;

namespace EFCoreBook.Queries
{
    public class Queries
    {
        public static void Run()
        {
            // var customers = SeedData.GenerateCustomers(customers: 100, ordersPerCustomer: 10, itemsPerOrder: 5);
            // Console.WriteLine($"Customers,{customers.Count}");

            // var totalOrders = customers.Sum(c => c.Orders.Count);
            // var totalItems = customers.Sum(c => c.Orders.Sum(o => o.Items.Count));
            // Console.WriteLine($"Orders: {totalOrders}, Items: {totalItems}");


            var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase("PerfDemoDb")
            .Options;

            using var context = new AppDbContext(options);
            var customers = SeedData.GenerateCustomers(customers: 50, ordersPerCustomer: 5, itemsPerOrder: 5);
              
            context.Customers.AddRange(customers);
            context.SaveChanges();

            // 4. تست سریع
            Console.WriteLine($"Customers in DB: {context.Customers.Count()}");
            Console.WriteLine($"Orders in DB: {context.Orders.Count()}");
            Console.WriteLine($"Items in DB: {context.Items.Count()}");



        }
    }

    
}