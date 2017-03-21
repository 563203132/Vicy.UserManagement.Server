using System.Data.Entity;
using Vicy.UserManagement.Server.DataAccess.Write;

namespace Vicy.UserManagement.Server.DataAccess.Read
{
    public class ReadDbContext : WriteDbContext
    {
        static ReadDbContext()
        {
            Database.SetInitializer<ReadDbContext>(null);
        }

        internal ReadDbContext() : base("read")
        {
        }
    }
}
