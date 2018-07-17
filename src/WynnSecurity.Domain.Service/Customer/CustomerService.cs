namespace WynnSecurity.Domain.Service
{
    public class CustomerService : ICustomerService
    {
        public readonly ICustomerRepository _customerRepository;

        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }

        public void AddCustomer(string name, string email)
        {
            var customer = new Customer() { Name = name, Email = email };

            _customerRepository.Insert(customer);
        }
    }
}
