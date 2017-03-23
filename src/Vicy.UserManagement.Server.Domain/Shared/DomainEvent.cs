using System;

namespace Vicy.UserManagement.Server.Domain.Shared
{
    public class DomainEvent : IDomainEvent
    {
        public DomainEvent()
        {
            DateTimeEventOccurred = DateTime.UtcNow;
        }

        public DateTime DateTimeEventOccurred { get; }
    }
}
