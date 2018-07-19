using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Linq;
using System.Net;
using System.Threading;
using WynnSecurity.Api.Model.Customer;
using WynnSecurity.DataAccess;
using WynnSecurity.Domain.Interfaces;
using WynnSecurity.Domain.Service;

namespace WynnSecurity.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IReadDbFacade _readDbFacade;
        private readonly ICustomerService _customerService;
        private readonly IWynnContextScopeFactory _wynnContextScopeFactory;

        public CustomerController(
            IReadDbFacade readDbFacade,
            ICustomerService customerService,
            IWynnContextScopeFactory wynnContextScopeFactory)
        {
            _readDbFacade = readDbFacade;
            _customerService = customerService;
            _wynnContextScopeFactory = wynnContextScopeFactory;
        }

        [HttpGet("{id}")]
        [SwaggerResponse((int)HttpStatusCode.OK, "Get a customer by customer id",
            typeof(CustomerModel))]
        public ActionResult Get(long id)
        {
            //var customer = _readDbFacade.Customers.FirstOrDefault(c => c.Id == id);
            Thread.Sleep(2 * 60 * 1000);

            var customer = new CustomerModel();

            return Ok(customer);
        }

        [HttpPut("create")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, "Invalid parameters")]
        [SwaggerResponse((int)HttpStatusCode.NoContent, "Successfully created customer")]
        public ActionResult Create(CustomerModel customer)
        {
            if (!ValidateCustomer(customer))
                return BadRequest("Invalid parameters.");

            using (var dbScope =
                _wynnContextScopeFactory.CreateWithReadCommittedTransaction("-1"))
            {
                _customerService.AddCustomer(customer.Name, customer.Email);

                dbScope.SaveChanges();
            }

            return Ok();
        }

        private bool ValidateCustomer(CustomerModel customer)
        {
            return !string.IsNullOrEmpty(customer.Name);
        }
    }
}