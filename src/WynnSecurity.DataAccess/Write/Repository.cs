using EntityFramework.DbContextScope.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using WynnSecurity.Domain.Interfaces;

namespace WynnSecurity.DataAccess.Write
{
    public class Repository<TAggregateRoot> : IRepository<TAggregateRoot> where TAggregateRoot : AggregateRoot
    {
        protected readonly IAmbientDbContextLocator _dbContextLocator;

        public Repository(IAmbientDbContextLocator dbContextLocator)
        {
            if (dbContextLocator == null)
                throw new ArgumentNullException(nameof(dbContextLocator) + " is null");

            _dbContextLocator = dbContextLocator;
        }

        protected DbSet<TAggregateRoot> Set => Context.Set<TAggregateRoot>();

        protected WynnDbContext Context
        {
            get
            {
                var dbContext = _dbContextLocator.Get<WynnDbContext>();

                if (dbContext == null)
                    throw new InvalidOperationException(@"No ambient DbContext of type NewOdinPoCContext found. 
                                                        This means that this repository method has been called outside of 
                                                        the scope of a DbContextScope. A repository must only be accessed 
                                                        within the scope of a DbContextScope, which takes care of creating the 
                                                        DbContext instances that the repositories need and making them available 
                                                        as ambient contexts. This is what ensures that, for any given 
                                                        DbContext-derived type, the same instance is used throughout the duration 
                                                        of a business transaction. To fix this issue, use IDbContextScopeFactory 
                                                        in your top-level business logic service method to create a DbContextScope 
                                                        that wraps the entire business transaction that your service method implements.
                                                        Then access this repository within that scope. Refer to the comments in the 
                                                        IDbContextScope.cs file for more details.");

                return dbContext;
            }
        }

        public virtual void Save()
        {
            Context.SaveChanges();
        }

        public virtual bool DoesExist(long id)
        {
            return Set.Any(a => a.Id == id);
        }

        public bool DoesExist(Expression<Func<TAggregateRoot, bool>> matchingCriteria)
        {
            return Set.Any(matchingCriteria);
        }

        public virtual TAggregateRoot GetById(long id)
        {
            return Set.Find(id);
        }

        public void BulkInsert(IEnumerable<TAggregateRoot> aggregateRoots)
        {
            Set.AddRange(aggregateRoots);
        }

        public void Insert(TAggregateRoot entity)
        {
            Set.Add(entity);
        }

        public void InsertAndSave(TAggregateRoot aggregateRoot)
        {
            Set.Add(aggregateRoot);
            Context.SaveChanges();
        }

        public virtual void Delete(long id)
        {
            var entityToDelete = Set.Find(id);
            Delete(entityToDelete);
        }

        public void Delete(TAggregateRoot aggregateRoot)
        {
            Set.Remove(aggregateRoot);
        }

        public void BulkDelete(IEnumerable<long> ids)
        {
            var entities = Set.Where(s => ids.Contains(s.Id));
            Set.RemoveRange(entities);
        }
    }
}
