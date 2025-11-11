using DAL.DbContexts;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Linq;

namespace congestion.calculator
{

    public class Program
    {
        public static void Main(string[] args)
        {
            HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

            builder.Configuration
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args);

            // Configure services
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<CongestionTaxDbContext>(options => options.UseSqlServer(connectionString));

            builder.Services.AddScoped<CongestionTaxCalculatorV2>();

            var host = builder.Build();

            // Initialize database
            using (var scope = host.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<CongestionTaxDbContext>();
                // Create database and apply migrations
                context.Database.EnsureCreated();
                Console.WriteLine("Database initialized successfully!");

                context.SeedData();
            }


            // Run tests
            host.Services.RunTests();

            host.Run();
        }



       

    }
}