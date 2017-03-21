using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Mehdime.Entity;
using Vicy.UserManagement.Server.Domain.Shared;

namespace Vicy.UserManagement.Server.DataAccess.Write
{
    public class Repository<TAggregateRoot> : IRepository<TAggregateRoot> where TAggregateRoot : AggregateRoot
    {
        private readonly IAmbientDbContextLocator _ambientDbContextLocator;

        public Repository(IAmbientDbContextLocator ambientDbContextLocator)
        {
            if (ambientDbContextLocator == null)
                throw new ArgumentNullException("ambientDbContextLocator");

            _ambientDbContextLocator = ambientDbContextLocator;
        }

        protected DbSet<TAggregateRoot> Set
        {
            get { return _context.Set<TAggregateRoot>(); }
        }

        private WriteDbContext _context
        {
            get
            {
                var dbContext = _ambientDbContextLocator.Get<WriteDbContext>();

                if (dbContext == null)
                    throw new InvalidOperationException("No ambient DbContext of type WriteDbContext found. This means that this repository method has been called outside of the scope of a DbContextScope. A repository must only be accessed within the scope of a DbContextScope, which takes care of creating the DbContext instances that the repositories need and making them available as ambient contexts. This is what ensures that, for any given DbContext-derived type, the same instance is used throughout the duration of a business transaction. To fix this issue, use IDbContextScopeFactory in your top-level business logic service method to create a DbContextScope that wraps the entire business transaction that your service method implements. Then access this repository within that scope. Refer to the comments in the IDbContextScope.cs file for more details.");

                return dbContext;
            }
        }

        public void BulkDelete(IEnumerable<long> ids)
        {
            var entities = Set.Where(s => ids.Contains(s.Id));
            Set.RemoveRange(entities);
        }

        public void BulkInsert(IEnumerable<TAggregateRoot> aggregateRoots)
        {
            Set.AddRange(aggregateRoots);
        }

        public void Delete(long id)
        {
            var entityToDelete = Set.Find(id);
            Delete(entityToDelete);
        }

        public void Delete(TAggregateRoot aggregateRoot)
        {
            Set.Remove(aggregateRoot);
        }

        public bool DoesExist(Expression<Func<TAggregateRoot, bool>> matchingCriteria)
        {
            return Set.Any(matchingCriteria);
        }

        public bool DoesExist(long id)
        {
            return Set.Count(a => a.Id == id) == 1;
        }

        public TAggregateRoot GetById(long id)
        {
            return Set.Find(id);
        }

        public void Insert(TAggregateRoot aggregateRoot)
        {
            Set.Add(aggregateRoot);
        }

        public void InsertAndSave(TAggregateRoot aggregateRoot)
        {
            Set.Add(aggregateRoot);
            _context.SaveChanges();
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
