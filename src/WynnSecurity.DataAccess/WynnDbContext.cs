using Microsoft.EntityFrameworkCore;
using System;
using WynnSecurity.DataAccess.Mappings;
using WynnSecurity.Domain;

namespace WynnSecurity.DataAccess
{
    public class WynnDbContext : DbContext
    {
        public WynnDbContext(DbContextOptions options)
           : base(options)
        {
        }

        public DbSet<Customer> Customers;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ResetConventions(modelBuilder);

            AddConfigurations(modelBuilder);
        }

        private void ResetConventions(ModelBuilder modelBuilder)
        {
            //modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            //modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        private void AddConfigurations(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CustomerMapping());
        }
    }
}
