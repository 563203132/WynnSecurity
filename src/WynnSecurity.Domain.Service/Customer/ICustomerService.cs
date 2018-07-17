using System;
using System.Collections.Generic;
using System.Text;

namespace WynnSecurity.Domain.Service
{
    public interface ICustomerService
    {
        void AddCustomer(string name, string email);
    }
}
