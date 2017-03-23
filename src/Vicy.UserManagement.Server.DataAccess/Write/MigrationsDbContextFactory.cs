using System.Data.Entity.Infrastructure;

namespace Vicy.UserManagement.Server.DataAccess.Write
{
    public class MigrationsDbContextFactory : IDbContextFactory<WriteDbContext>
    {
        public WriteDbContext Create()
        {
            // The dependencies are not necessary for db migration.
            return new WriteDbContext(null, null);
        }
    }
}
