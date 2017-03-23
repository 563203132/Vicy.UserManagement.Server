using System.Data.Entity;

namespace Vicy.UserManagement.Server.DataAccess.Write
{
    public interface IDbContextFactory
    {
        TDbContext CreateDbContext<TDbContext>() where TDbContext : DbContext;
    }
}
