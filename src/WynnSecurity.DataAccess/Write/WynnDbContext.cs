using EntityFramework.DbContextScope.Interfaces;
using Microsoft.EntityFrameworkCore;
using WynnSecurity.DataAccess.Extensions;
using WynnSecurity.DataAccess.Write.Mappings;
using WynnSecurity.Domain;

namespace WynnSecurity.DataAccess.Write
{
    public class WynnDbContext : DbContext, IDbContext
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
            modelBuilder.SetPluralizingTableNameConvention();
            modelBuilder.SetOneToManyCascadeDeleteConvention();
        }

        private void AddConfigurations(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CustomerMapping());
        }
    }
}
