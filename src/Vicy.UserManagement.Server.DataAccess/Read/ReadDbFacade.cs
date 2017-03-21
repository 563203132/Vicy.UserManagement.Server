using System;
using System.Linq;
using Vicy.UserManagement.Server.Domain;
using Vicy.UserManagement.Server.Domain.Shared;

namespace Vicy.UserManagement.Server.DataAccess.Read
{
    public class ReadDbFacade : IReadDbFacade, IDisposable
    {
        private readonly ReadDbContext _context = new ReadDbContext();

        public IQueryable<User> User => _context.Users;

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
