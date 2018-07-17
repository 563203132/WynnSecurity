using System;
using WynnSecurity.Domain.Interfaces;

namespace WynnSecurity.Domain
{
    public class Customer : AggregateRoot
    {
        public string Name { get; set; }

        public string Email { get; set; }
    }
}
