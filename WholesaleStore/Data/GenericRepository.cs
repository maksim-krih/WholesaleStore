using System;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WholesaleStore.Data.Interfaces;

namespace WholesaleStore.Data
{
    public class GenericEFRepository<TEntity> : IRepository<TEntity>
       where TEntity : class
    {
        private bool disposed = false;
        private DbContext _dbContext;
        public DbSet<TEntity> DbSet { get; set; }


        public GenericEFRepository(WholesaleStoreContext dbContext)
        {
            _dbContext = dbContext;
            DbSet = dbContext.Set<TEntity>();
        }

        public IQueryable<TEntity> Query => DbSet;

        public TEntity Create(TEntity domain)
        {
            var entity = DbSet.Add(domain);

            return entity;
        }

        public TEntity Remove(TEntity domain)
        {
            var entity = DbSet.Remove(domain);

            return entity;
        }

        public Task<int> CommitAsync(CancellationToken cancellation)
        {
            return _dbContext.SaveChangesAsync(cancellation);
        }

        #region Dispose

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                _dbContext.Dispose();
            }

            disposed = true;
        }

        #endregion
    }
}
