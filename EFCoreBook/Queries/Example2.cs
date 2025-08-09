using System.Diagnostics;
using EFCoreBook.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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


            // var options = new DbContextOptionsBuilder<AppDbContext>()
            // .UseInMemoryDatabase("PerfDemoDb")
            // .UseLazyLoadingProxies()
            // .Options;

            var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite("DataSource=memory")
            .UseLazyLoadingProxies()
            .LogTo(Console.WriteLine, LogLevel.Information)
            .EnableSensitiveDataLogging()
            .Options;



            using var context = new AppDbContext(options);
            context.Database.OpenConnection();
            context.Database.EnsureCreated();

            var customers = SeedData.GenerateCustomers(customers: 50, ordersPerCustomer: 5, itemsPerOrder: 5);

            // var allCustomers = context.Customers.ToList();
            // foreach (var c in allCustomers)
            // {
            //     Console.WriteLine($"Customer: {c.Name}, Orders: {c.Orders.Count}");
            // }

            context.Customers.AddRange(customers);
            context.SaveChanges();

            // 4. تست سریع
            Console.WriteLine($"Customers in DB: {context.Customers.Count()}");
            Console.WriteLine($"Orders in DB: {context.Orders.Count()}");
            Console.WriteLine($"Items in DB: {context.Items.Count()}");

            var sw = Stopwatch.StartNew();


            var allCustomers = context.Customers.ToList();
            foreach (var c in allCustomers)
            {
                Console.WriteLine($"Customer:{c.Name},Order:{c.Orders.Count}");
            }
            sw.Stop();
            Console.WriteLine($"Elapsed: {sw.ElapsedMilliseconds} ms");



            //===============================================
            sw.Reset();
            sw.Start();
            var allCustomersEager = context.Customers.Include(c => c.Orders)
            .ToList();

            foreach (var c in allCustomersEager)
            {
                Console.WriteLine($"Customer: {c.Name}, Orders: {c.Orders.Count}");
            }

            sw.Stop();
            Console.WriteLine($"Elapsed: {sw.ElapsedMilliseconds} ms");

            sw.Reset();
            sw.Start();
            var allCustomersAsNoTracking = context.Customers
            .AsNoTracking()
            .Include(c => c.Orders)
            .ToList();

            foreach (var c in allCustomersAsNoTracking)
            {
                Console.WriteLine($"Customer: {c.Name}, Orders: {c.Orders.Count}");
            }
            sw.Stop();
            Console.WriteLine($"Elapsed: {sw.ElapsedMilliseconds} ms");

            sw.Reset();
            sw.Start();

            var consumerQuerySpliting = context.Customers
            .AsSplitQuery()
            .Include(c => c.Orders)
            .ThenInclude(o => o.Items)
            .ToList();

            foreach (var c in consumerQuerySpliting)
            {
                Console.WriteLine($"Customer: {c.Name}, Orders: {c.Orders.Count}");
            }

            sw.Stop();
            Console.WriteLine($"Elapsed: {sw.ElapsedMilliseconds} ms");

            






        }
    }

    
}