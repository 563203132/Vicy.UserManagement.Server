using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using Vicy.UserManagement.Server.DataAccess.Write.Mapping;
using Vicy.UserManagement.Server.Domain;

namespace Vicy.UserManagement.Server.DataAccess.Write
{
    public class WriteDbContext : DbContext
    {
        public WriteDbContext() : base("write")
        {
            Configuration.LazyLoadingEnabled = false;
        }

        protected WriteDbContext(string nameOrConnectionStrng)
            : base(nameOrConnectionStrng)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            ResetConventions(modelBuilder);

            AddConfigurations(modelBuilder);
        }

        public override int SaveChanges()
        {
            return base.SaveChanges();
        }

        private void ResetConventions(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

        private void AddConfigurations(DbModelBuilder modelBuilder)
        {
            //Map to DB
            modelBuilder.Configurations.Add(new UserMapping());
        }
    }
}
