using Microsoft.AspNetCore.Mvc;
using WynnSecurity.Api.Model.Customer;
using WynnSecurity.Domain.Service;

namespace WynnSecurity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        [Route("{id}")]
        public ActionResult Get(long id)
        {
            var customer = new CustomerModel();

            return Ok(customer);
        }

        [HttpPut]
        [Route("create")]
        public ActionResult Create(CustomerModel customer)
        {
            if (!ValidateCustomer(customer))
                return new ChallengeResult();

            _customerService.AddCustomer(customer.Name, customer.Email);

            return Ok();
        }

        private bool ValidateCustomer(CustomerModel customer)
        {
            return !string.IsNullOrEmpty(customer.Name);
        }
    }
}