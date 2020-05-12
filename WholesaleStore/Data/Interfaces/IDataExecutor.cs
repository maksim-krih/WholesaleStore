using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace WholesaleStore.Data.Interfaces
{
    public interface IDataExecutor
    {
        Task<List<TSource>> ToListAsync<TSource>(IQueryable<TSource> collection, CancellationToken cancellationToken = default(CancellationToken));

        Task<TSource> FirstOrDefaultAsync<TSource>(IQueryable<TSource> collection, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken));

        Task<TSource> FirstOrDefaultAsync<TSource>(IQueryable<TSource> collection, CancellationToken cancellationToken = default(CancellationToken));

        Task<bool> AnyAsync<TSource>(IQueryable<TSource> collection, CancellationToken cancellationToken = default(CancellationToken));
        Task<bool> AnyAsync<TSource>(IQueryable<TSource> collection, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken));

        IQueryable<TSource> AsNoTracking<TSource>(IQueryable<TSource> collection, CancellationToken cancellationToken = default(CancellationToken)) where TSource : class;
    }
}
