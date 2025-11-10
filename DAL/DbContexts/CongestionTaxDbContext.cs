using DAL.Models;
using Microsoft.EntityFrameworkCore;

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
    }
}
