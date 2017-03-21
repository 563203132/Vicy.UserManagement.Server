using Mehdime.Entity;
using Vicy.UserManagement.Server.Domain;

namespace Vicy.UserManagement.Server.DataAccess.Write.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(IAmbientDbContextLocator ambientDbContextLocator) : base(ambientDbContextLocator)
        {
        }
    }
}
