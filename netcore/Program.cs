using DAL.DbContexts;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace congestion.calculator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Setup dependency injection
            var services = new ServiceCollection();

            // Use SQLite for this example (easier to set up)
            var connectionString = "Data Source=congestion_tax.db";

            services.AddDbContext<CongestionTaxDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddScoped<CongestionTaxCalculatorV2>();

            var serviceProvider = services.BuildServiceProvider();

            // Initialize database
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<CongestionTaxDbContext>();

                // Create database and apply migrations
                context.Database.EnsureCreated();

                Console.WriteLine("Database initialized successfully!");
            }

            // Run tests
            RunTests(serviceProvider);
        }

        private static void RunTests(ServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var calculator = scope.ServiceProvider.GetRequiredService<CongestionTaxCalculatorV2>();
            var car = new Car();
            var motorbike = new Motorbike();

            Console.WriteLine("\n=== Database-Driven Congestion Tax Calculator Tests ===\n");

            // Test 1: Single pass
            Console.WriteLine("Test 1: Single morning pass in Gothenburg");
            var dates1 = new[] { DateTime.Parse("2013-02-07 06:23:27") };
            var result1 = calculator.GetTax("Gothenburg", car, dates1);
            Console.WriteLine($"Result: {result1} SEK (Expected: 8 SEK)");
            Console.WriteLine($"Status: {(result1 == 8 ? "✓ PASS" : "✗ FAIL")}\n");

            // Test 2: Multiple passes with 60-minute rule
            Console.WriteLine("Test 2: Two passes within 60-minute window");
            var dates2 = new[]
            {
                DateTime.Parse("2013-02-08 06:20:27"),
                DateTime.Parse("2013-02-08 06:27:00")
            };
            var result2 = calculator.GetTax("Gothenburg", car, dates2);
            Console.WriteLine($"Result: {result2} SEK (Expected: 8 SEK - single charge rule)");
            Console.WriteLine($"Status: {(result2 == 8 ? "✓ PASS" : "✗ FAIL")}\n");

            // Test 3: Complex day
            Console.WriteLine("Test 3: Complex day with multiple windows");
            var dates3 = new[]
            {
                DateTime.Parse("2013-02-08 06:27:00"),
                DateTime.Parse("2013-02-08 06:20:27"),
                DateTime.Parse("2013-02-08 14:35:00"),
                DateTime.Parse("2013-02-08 15:29:00"),
                DateTime.Parse("2013-02-08 15:47:00"),
                DateTime.Parse("2013-02-08 16:01:00"),
                DateTime.Parse("2013-02-08 16:48:00"),
                DateTime.Parse("2013-02-08 17:49:00"),
                DateTime.Parse("2013-02-08 18:29:00"),
                DateTime.Parse("2013-02-08 18:35:00")
            };
            var result3 = calculator.GetTax("Gothenburg", car, dates3);
            Console.WriteLine($"Result: {result3} SEK (Expected: 47 SEK)");
            Console.WriteLine($"Status: {(result3 == 47 ? "✓ PASS" : "✗ FAIL")}\n");

            // Test 4: Weekend
            Console.WriteLine("Test 4: Weekend pass");
            var dates4 = new[] { DateTime.Parse("2013-02-09 07:00:00") };
            var result4 = calculator.GetTax("Gothenburg", car, dates4);
            Console.WriteLine($"Result: {result4} SEK (Expected: 0 SEK - Saturday)");
            Console.WriteLine($"Status: {(result4 == 0 ? "✓ PASS" : "✗ FAIL")}\n");

            // Test 5: Public holiday
            Console.WriteLine("Test 5: Public holiday");
            var dates5 = new[] { DateTime.Parse("2013-03-28 14:07:27") };
            var result5 = calculator.GetTax("Gothenburg", car, dates5);
            Console.WriteLine($"Result: {result5} SEK (Expected: 0 SEK - day before Good Friday)");
            Console.WriteLine($"Status: {(result5 == 0 ? "✓ PASS" : "✗ FAIL")}\n");

            // Test 6: July
            Console.WriteLine("Test 6: July (tax-free month)");
            var dates6 = new[] { DateTime.Parse("2013-07-15 07:00:00") };
            var result6 = calculator.GetTax("Gothenburg", car, dates6);
            Console.WriteLine($"Result: {result6} SEK (Expected: 0 SEK - July)");
            Console.WriteLine($"Status: {(result6 == 0 ? "✓ PASS" : "✗ FAIL")}\n");

            // Test 7: Toll-free vehicle
            Console.WriteLine("Test 7: Toll-free vehicle (Motorbike)");
            var dates7 = new[] { DateTime.Parse("2013-02-07 07:00:00") };
            var result7 = calculator.GetTax("Gothenburg", motorbike, dates7);
            Console.WriteLine($"Result: {result7} SEK (Expected: 0 SEK - Motorbike)");
            Console.WriteLine($"Status: {(result7 == 0 ? "✓ PASS" : "✗ FAIL")}\n");

            // Test 8: Maximum daily fee
            Console.WriteLine("Test 8: Maximum daily fee");
            var dates8 = new[]
            {
                DateTime.Parse("2013-02-07 06:00:00"),
                DateTime.Parse("2013-02-07 08:00:00"),
                DateTime.Parse("2013-02-07 10:00:00"),
                DateTime.Parse("2013-02-07 12:00:00"),
                DateTime.Parse("2013-02-07 15:00:00"),
                DateTime.Parse("2013-02-07 17:00:00")
            };
            var result8 = calculator.GetTax("Gothenburg", car, dates8);
            Console.WriteLine($"Result: {result8} SEK (Expected: 60 SEK - daily cap)");
            Console.WriteLine($"Status: {(result8 == 60 ? "✓ PASS" : "✗ FAIL")}\n");

            Console.WriteLine("=== All Tests Complete ===");
        }
    }
}
