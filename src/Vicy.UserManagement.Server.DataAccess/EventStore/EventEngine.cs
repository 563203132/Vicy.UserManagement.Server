using System;
using System.Data;
using System.Linq;
using Mehdime.Entity;
using Vicy.UserManagement.Server.DataAccess.Write;
using Vicy.UserManagement.Server.Domain.Shared;

namespace Vicy.UserManagement.Server.DataAccess.EventStore
{
    public class EventEngine : IEventEngine
    {
        private readonly IAmbientDbContextLocator _dbContextLocator;
        private readonly IEventDispatcher _eventDispatcher;
        private readonly IEventSerializer _serializer;

        public EventEngine(
            IAmbientDbContextLocator dbContextLocator,
            IEventDispatcher eventDispatcher,
            IEventSerializer serializer)
        {
            _dbContextLocator = dbContextLocator;
            _eventDispatcher = eventDispatcher;
            _serializer = serializer;
        }

        public void Process()
        {
            DispatchEvents();
        }

        private void DispatchEvents()
        {
            var writeDbContext = _dbContextLocator.Get<WriteDbContext>();
            if (writeDbContext == null)
                return;

            var unPublishEvents = writeDbContext.Events.Local.Where(e => e.IsPublished == false).ToArray();
            if (!unPublishEvents.Any())
                return;

            foreach (var @event in unPublishEvents)
            {
                using (
                    var writeDbContextTransaction =
                        writeDbContext.Database.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    _eventDispatcher.Dispatch(_serializer.Deserialize(@event.DomainEvent, @event.EventType));
                    @event.MarkedAsPublished();
                    writeDbContext.SaveChanges();
                    writeDbContextTransaction.Commit();
                }
            }

            RaiseNestedEvent();
        }

        private void RaiseNestedEvent()
        {
            var writeDbContext = _dbContextLocator.Get<WriteDbContext>();
            var unPublishEvents = writeDbContext.Events.Local.Where(e => e.IsPublished == false).ToArray();
            if (!unPublishEvents.Any())
                return;

            DispatchEvents();
        }
    }
}
