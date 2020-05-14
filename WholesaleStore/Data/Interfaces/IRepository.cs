using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WholesaleStore.Data.Interfaces
{
    public interface IRepository<TEntity> : IDisposable where TEntity : class
    {
        IQueryable<TEntity> Query { get; }

        TEntity Create(TEntity domain);

        TEntity Remove(TEntity entity);

        Task<int> CommitAsync(CancellationToken cancellation = default);
    }
}
