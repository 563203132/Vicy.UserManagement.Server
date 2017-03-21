using System.Collections.Generic;

namespace Vicy.UserManagement.Server.Domain.Shared
{
    public abstract class AggregateRoot : Entity
    {
        private readonly List<IDomainEvent> _events = new List<IDomainEvent>();

        public virtual IReadOnlyList<IDomainEvent> Events => _events;

        public virtual void ClearEvents()
        {
            _events.Clear();
        }

        protected virtual void AddEvent(IDomainEvent newEvent)
        {
            _events.Add(newEvent);
        }
    }
}
