using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using Vicy.UserManagement.Server.DataAccess.EventStore;
using Vicy.UserManagement.Server.DataAccess.Write.Mapping;
using Vicy.UserManagement.Server.Domain;
using Vicy.UserManagement.Server.Domain.Shared;

namespace Vicy.UserManagement.Server.DataAccess.Write
{
    public class WriteDbContext : DbContext
    {
        private readonly IEventEngine _eventEngine;
        private readonly IEventSerializer _eventSerializer;

        public WriteDbContext(
            IEventEngine eventEngine,
            IEventSerializer serializer) : base("write")
        {
            Configuration.LazyLoadingEnabled = false;

            _eventEngine = eventEngine;
            _eventSerializer = serializer;
        }

        protected WriteDbContext(string nameOrConnectionStrng)
            : base(nameOrConnectionStrng)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Event> Events { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            ResetConventions(modelBuilder);

            AddConfigurations(modelBuilder);
        }

        public override int SaveChanges()
        {
            PersistEvents();

            var state = base.SaveChanges();

            if (_eventEngine != null)
                _eventEngine.Process();

            return state;
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
            modelBuilder.Configurations.Add(new EventMapping());
        }

        private void PersistEvents()
        {
            var uncommittedAggregates = GetUncommittedAggregates().ToList();

            foreach (var aggregate in uncommittedAggregates)
            {
                AddAggregateEvents(aggregate.Events);
                aggregate.ClearEvents();
            }
        }

        private IEnumerable<AggregateRoot> GetUncommittedAggregates()
        {
            return ChangeTracker.Entries<AggregateRoot>()
                .Select(po => po.Entity)
                .Where(po => po.Events.Any());
        }

        private void AddAggregateEvents(IEnumerable<IDomainEvent> events)
        {
            foreach(var @event in events)
            {
                Events.Add(
                    new Event(
                        _eventSerializer.Serialize(@event),
                        @event.GetType().AssemblyQualifiedName));
            }
        }
    }
}
