using System.Linq;

namespace Vicy.UserManagement.Server.Domain.Shared
{
    public class IReadDbFacade
    {
        IQueryable<User> Users { get; }
    }
}
