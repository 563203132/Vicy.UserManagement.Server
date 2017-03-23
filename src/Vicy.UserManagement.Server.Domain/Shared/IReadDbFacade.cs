using System.Linq;

namespace Vicy.UserManagement.Server.Domain.Shared
{
    public interface IReadDbFacade
    {
        IQueryable<User> Users { get; }
    }
}
