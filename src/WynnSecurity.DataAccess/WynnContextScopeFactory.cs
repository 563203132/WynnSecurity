using EntityFramework.DbContextScope;
using EntityFramework.DbContextScope.Interfaces;
using System.Data;

namespace WynnSecurity.DataAccess
{
    public interface IWynnContextScopeFactory
    {
        IWynnDbContextScope Create(string transactionUser,
            DbContextScopeOption joiningOption = DbContextScopeOption.JoinExisting);

        IWynnDbContextScope CreateWithReadCommittedTransaction(string transactionUser);
        IWynnDbContextScope CreateWithReadCommittedTransaction(long transactionUserId);
    }

    public class WynnContextScopeFactory : IWynnContextScopeFactory
    {
        protected readonly IAmbientDbContextLocator _dbContextLocator;
        private readonly IDbContextScopeFactory _dbContextScopeFactory;

        public WynnContextScopeFactory(
            IAmbientDbContextLocator dbContextLocator,
            IDbContextScopeFactory dbContextScopeFactory)
        {
            _dbContextLocator = dbContextLocator;
            _dbContextScopeFactory = dbContextScopeFactory;
        }

        public IWynnDbContextScope Create(string transactionUser, DbContextScopeOption joiningOption = DbContextScopeOption.JoinExisting)
        {
            var dbContextScope = _dbContextScopeFactory.Create(joiningOption);

            return new WynnDbContextScope(dbContextScope, _dbContextLocator);
        }

        public IWynnDbContextScope CreateWithReadCommittedTransaction(string transactionUser)
        {
            var dbContextScope = _dbContextScopeFactory.CreateWithTransaction(IsolationLevel.ReadCommitted);

            return new WynnDbContextScope(dbContextScope, _dbContextLocator);
        }

        public IWynnDbContextScope CreateWithReadCommittedTransaction(long transactionUserId)
        {
            return CreateWithReadCommittedTransaction(transactionUserId.ToString());
        }
    }
}
