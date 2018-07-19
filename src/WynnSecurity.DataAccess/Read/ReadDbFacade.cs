using System;
using System.Linq;
using WynnSecurity.Domain;
using WynnSecurity.Domain.Interfaces;

namespace WynnSecurity.DataAccess.Read
{
    public class ReadDbFacade : IReadDbFacade, IDisposable
    {
        private readonly ReadDbContext _readDbContext;

        public ReadDbFacade(ReadDbContext readDbContext)
        {
            _readDbContext = readDbContext;
        }

        public IQueryable<Customer> Customers => _readDbContext.Customers;

        public void Dispose()
        {
            _readDbContext.Dispose();
        }
    }
}
