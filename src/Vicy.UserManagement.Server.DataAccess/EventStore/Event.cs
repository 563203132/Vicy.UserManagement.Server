using System;

namespace Vicy.UserManagement.Server.DataAccess.EventStore
{
    public class Event
    {
        private Event()
        {
        }

        public Event(string domainEvent, string eventType)
        {
            Id = Guid.NewGuid();
            IsPublished = false;
            CreateTimeUtc = DateTime.UtcNow;
            UpdateTimeUtc = DateTime.UtcNow;
            DomainEvent = domainEvent;
            EventType = eventType;
        }

        public Guid Id { get; set; }
        public string DomainEvent { get; private set; }
        public string EventType { get; private set; }
        public bool IsPublished { get; private set; }
        public DateTime CreateTimeUtc { get; private set; }
        public DateTime UpdateTimeUtc { get; private set; }

        public void MarkedAsPublished()
        {
            IsPublished = true;
            UpdateTimeUtc = DateTime.UtcNow;
        }
    }
}
