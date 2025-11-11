using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.DbContexts
{
    public class CongestionTaxDbContextFactory : IDesignTimeDbContextFactory<CongestionTaxDbContext>
    {
        public CongestionTaxDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<CongestionTaxDbContext>();

            optionsBuilder.UseSqlServer("Data Source=localhost;Initial Catalog=TaxCalculatorDb;user id=sa;password=Aa@112233445566; Encrypt=True;TrustServerCertificate=True; Connection Timeout=120");

            return new CongestionTaxDbContext(optionsBuilder.Options);
        }
    }
}
