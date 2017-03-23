using System;
using System.Data.Entity;
using System.Linq;
using Mehdime.Entity;

namespace Vicy.UserManagement.Server.DataAccess.Write
{
    public class DbContextFactory : IDbContextFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public DbContextFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public TDbContext CreateDbContext<TDbContext>() where TDbContext : DbContext
        {
            var type = typeof(TDbContext);
            var constructors = type.GetConstructors();
            if (constructors.Length != 1)
            {
                throw new InvalidOperationException($"The type '{type.Name}' should have only one constructor.");
            }

            var constructor = constructors.Single();
            var parameters = constructor.GetParameters().Select(p => _serviceProvider.GetService(p.ParameterType)).ToArray();

            return (TDbContext)Activator.CreateInstance(type, parameters);
        }
    }
}
