using WynnSecurity.Domain;

namespace WynnSecurity.DataAccess.Repositories
{
    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        public CustomerRepository(WynnDbContext context) : base(context)
        {

        }
    }
}
