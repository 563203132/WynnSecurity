using System.Linq;

namespace WynnSecurity.Domain.Interfaces
{
    public interface IReadDbFacade
    {
        IQueryable<Customer> Customers { get; }
    }
}
