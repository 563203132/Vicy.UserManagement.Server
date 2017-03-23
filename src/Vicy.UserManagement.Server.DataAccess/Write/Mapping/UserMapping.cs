using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using Vicy.UserManagement.Server.Domain;

namespace Vicy.UserManagement.Server.DataAccess.Write.Mapping
{
    public class UserMapping : EntityTypeConfiguration<User>
    {
        public UserMapping()
        {
            HasKey(e => e.Id).Property(e => e.Id).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            Property(e => e.LastName).IsRequired().HasMaxLength(100);
            Property(e => e.Email).HasMaxLength(50);
            Property(e => e.PhoneNumber).HasMaxLength(100);
        }
    }
}
