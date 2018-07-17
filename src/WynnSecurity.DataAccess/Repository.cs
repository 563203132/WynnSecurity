using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using WynnSecurity.Domain.Interfaces;

namespace WynnSecurity.DataAccess
{
    public class Repository<TAggregateRoot> : IRepository<TAggregateRoot> where TAggregateRoot : AggregateRoot
    {
        protected readonly WynnDbContext Context;

        public Repository(WynnDbContext context)
        {
            Context = context;
        }

        protected DbSet<TAggregateRoot> Set => Context.Set<TAggregateRoot>();

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
