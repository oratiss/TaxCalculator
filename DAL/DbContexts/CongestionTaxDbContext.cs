using DAL.Models;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DAL.DbContexts
{
    public class CongestionTaxDbContext(DbContextOptions<CongestionTaxDbContext> dbContextOptions)
        : DbContext(dbContextOptions)
    {
        public DbSet<City> Cities { get; set; }
        public DbSet<TaxCalculationLog> TaxCalculationLogs { get; set; }
        public DbSet<TaxFreeDate> TaxFreeDates { get; set; }
        public DbSet<TaxFreeVehicleType> TaxFreeVehicleTypes { get; set; }
        public DbSet<TaxTimeSlot> TaxTimeSlots { get; set; }
        public DbSet<WeekendRule> WeekendRules { get; set; }

        public void SeedData()
        {
            // Check if cities already exist
            if (Cities.Any())
            {
                Console.WriteLine("Database already seeded. Skipping seed data.");
                return;
            }

            Console.WriteLine("Seeding database...");

            // Add cities
            var gothenburg = new City
            {
                Name = "Gothenburg",
                MaxDailyFee = 60,
                IsActive = true,
                Currency = "SEK",
                CountryCode = "SE",
                SingleChargeWindowMinutes = 60
            };

            var stockholm = new City
            {
                Name = "Stockholm",
                MaxDailyFee = 60,
                IsActive = true,
                Currency = "SEK",
                CountryCode = "SE",
                SingleChargeWindowMinutes = 60
            };

            Cities.AddRange(gothenburg, stockholm);
            SaveChanges();

            Console.WriteLine("Seeded 2 cities successfully!");


            // Add tax time slots for Gothenburg
            var timeSlots = new[]
            {
                new TaxTimeSlot
                {
                    CityId = gothenburg.CityId,
                    StartTime = new TimeSpan(6, 0, 0), // 06:00
                    EndTime = new TimeSpan(6, 29, 59), // 06:29
                    TaxAmount = 8,
                    DisplayOrder = 1,
                },
                new TaxTimeSlot
                {
                    CityId = gothenburg.CityId,
                    StartTime = new TimeSpan(6, 30, 0),  // 06:30
                    EndTime = new TimeSpan(6, 59, 59),   // 06:59
                    TaxAmount = 13,
                    DisplayOrder = 2
                },
                new TaxTimeSlot
                {
                    CityId = gothenburg.CityId,
                    StartTime = new TimeSpan(7, 0, 0),   // 07:00
                    EndTime = new TimeSpan(7, 59, 59),   // 07:59
                    TaxAmount = 18,
                    DisplayOrder = 3
                },
                new TaxTimeSlot
                {
                    CityId = gothenburg.CityId,
                    StartTime = new TimeSpan(8, 0, 0),   // 08:00
                    EndTime = new TimeSpan(8, 29, 59),   // 08:29
                    TaxAmount = 13,
                    DisplayOrder = 4
                },
                new TaxTimeSlot
                {
                    CityId = gothenburg.CityId,
                    StartTime = new TimeSpan(8, 30, 0),  // 08:30
                    EndTime = new TimeSpan(14, 59, 59),  // 14:59
                    TaxAmount = 8,
                    DisplayOrder = 5
                },
                new TaxTimeSlot
                {
                    CityId = gothenburg.CityId,
                    StartTime = new TimeSpan(15, 0, 0),  // 15:00
                    EndTime = new TimeSpan(15, 29, 59),  // 15:29
                    TaxAmount = 13,
                    DisplayOrder = 6
                },
                new TaxTimeSlot
                {
                    CityId = gothenburg.CityId,
                    StartTime = new TimeSpan(15, 30, 0), // 15:30
                    EndTime = new TimeSpan(16, 59, 59),  // 16:59
                    TaxAmount = 18,
                    DisplayOrder = 7
                },
                new TaxTimeSlot
                {
                    CityId = gothenburg.CityId,
                    StartTime = new TimeSpan(17, 0, 0),  // 17:00
                    EndTime = new TimeSpan(17, 59, 59),  // 17:59
                    TaxAmount = 13,
                    DisplayOrder = 8
                },
                new TaxTimeSlot
                {
                    CityId = gothenburg.CityId,
                    StartTime = new TimeSpan(18, 0, 0),  // 18:00
                    EndTime = new TimeSpan(18, 29, 59),  // 18:29
                    TaxAmount = 8,
                    DisplayOrder = 9
                },
                new TaxTimeSlot
                {
                    CityId = gothenburg.CityId,
                    StartTime = new TimeSpan(18, 30, 0), // 18:30
                    EndTime = new TimeSpan(5, 59, 59),   // 05:59 (next day)
                    TaxAmount = 0,
                    DisplayOrder = 10
                }
            };

            TaxTimeSlots.AddRange(timeSlots);
            SaveChanges();

            Console.WriteLine($"Seeded {timeSlots.Length} tax time slots for Gothenburg!");

            // Add weekend rules
            var weekendRule = new WeekendRule
            {
                CityId = gothenburg.CityId,
                IsSaturdayTaxFree = true,
                IsSundayTaxFree = true
            };

            WeekendRules.Add(weekendRule);
            SaveChanges();

            Console.WriteLine("Seeded weekend rules!");

            // Add tax-free vehicle types
            var taxFreeVehicles = new[]
            {
                new TaxFreeVehicleType
                {
                    CityId = gothenburg.CityId,
                    VehicleTypeName = "Motorbike",
                    Description = "Motorcycles are tax-free",
                    IsActive = true
                },
                new TaxFreeVehicleType
                {
                    CityId = gothenburg.CityId,
                    VehicleTypeName = "Diplomat",
                    Description = "Diplomatic vehicles are tax-free",
                    IsActive = true
                },
                new TaxFreeVehicleType
                {
                    CityId = gothenburg.CityId,
                    VehicleTypeName = "Emergency",
                    Description = "Emergency vehicles are tax-free",
                    IsActive = true
                },
                new TaxFreeVehicleType
                {
                    CityId = gothenburg.CityId,
                    VehicleTypeName = "Foreign",
                    Description = "Foreign vehicles are tax-free",
                    IsActive = true
                },
                new TaxFreeVehicleType
                {
                    CityId = gothenburg.CityId,
                    VehicleTypeName = "Military",
                    Description = "Military vehicles are tax-free",
                    IsActive = true
                }
            };

            TaxFreeVehicleTypes.AddRange(taxFreeVehicles);
            SaveChanges();

            Console.WriteLine($"Seeded {taxFreeVehicles.Length} tax-free vehicle types!");

            // Add tax-free dates (July and day before holidays)
            var taxFreeDates = new[]
            {
                new TaxFreeDate
                {
                    CityId = gothenburg.CityId,
                    IsRecurring = true,
                    RecurringMonth = 7,
                    Description = "July is tax-free",
                    Type = TaxFreeDateType.TaxFreeMonth,
                     Date = new DateTime(2013, 07, 01),
                },
                new TaxFreeDate
                {
                    CityId = gothenburg.CityId,
                    Date = new DateTime(2013, 3, 28),
                    Description = "Day before Good Friday",
                    Type = TaxFreeDateType.PublicHoliday
                }
            };

            TaxFreeDates.AddRange(taxFreeDates);
            SaveChanges();

            Console.WriteLine($"Seeded {taxFreeDates.Length} tax-free dates!");
            Console.WriteLine("=== Database seeding complete! ===\n");
        }
    }
}
