using EntityFramework.DbContextScope.Interfaces;
using System;
using System.Linq;

namespace WynnSecurity.DataAccess
{
    public class DbContextFactory : IDbContextFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public DbContextFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public TDbContext CreateDbContext<TDbContext>() where TDbContext : class, IDbContext
        {
            var type = typeof(TDbContext);

            var constructors = type.GetConstructors();
            if (constructors.Length != 1)
            {
                throw new InvalidOperationException($"The type '{type.Name}' should have only one constructor.");
            }

            var constructor = constructors.Single();
            var parameters =
                constructor.GetParameters().Select(pi => _serviceProvider.GetService(pi.ParameterType)).ToArray();

            return (TDbContext)Activator.CreateInstance(type, parameters);
        }
    }
}
