using System.Data.Entity.ModelConfiguration;
using Vicy.UserManagement.Server.DataAccess.EventStore;

namespace Vicy.UserManagement.Server.DataAccess.Write.Mapping
{
    public class EventMapping : EntityTypeConfiguration<Event>
    {
        public EventMapping()
        {
            HasKey(e => new { e.Id });
            Property(e => e.EventType).HasColumnName("EventType").HasMaxLength(2000).IsRequired();
            Property(e => e.DomainEvent).HasColumnName("DomainEvent").HasMaxLength(500).IsRequired();
        }
    }
}
