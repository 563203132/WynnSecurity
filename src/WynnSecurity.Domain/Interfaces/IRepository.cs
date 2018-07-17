using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace WynnSecurity.Domain.Interfaces
{
    public interface IRepository<TAggregateRoot> where TAggregateRoot : AggregateRoot
    {
        bool DoesExist(long id);

        bool DoesExist(Expression<Func<TAggregateRoot, bool>> matchingCriteria);

        TAggregateRoot GetById(long id);

        void BulkInsert(IEnumerable<TAggregateRoot> aggregateRoots);

        void Insert(TAggregateRoot aggregateRoot);

        void InsertAndSave(TAggregateRoot aggregateRoot);

        void Delete(long id);

        void BulkDelete(IEnumerable<long> ids);

        void Save();
    }
}
