﻿using EntityFramework.DbContextScope.Interfaces;
using WynnSecurity.Domain;

namespace WynnSecurity.DataAccess.Write.Repositories
{
    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        public CustomerRepository(IAmbientDbContextLocator dbContextLocator) : base(dbContextLocator)
        {

        }
    }
}
