using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using WholesaleStore.Data.Interfaces;

namespace WholesaleStore.Data
{
	public class DataExecutor : IDataExecutor
	{
		public Task<List<TSource>> ToListAsync<TSource>(IQueryable<TSource> collection, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (collection == null)
			{ 
				throw new ArgumentNullException(nameof(collection)); 
			}

			return QueryableExtensions.ToListAsync(collection, cancellationToken);
		}

		public Task<TSource> FirstOrDefaultAsync<TSource>(IQueryable<TSource> collection, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
		{
			return QueryableExtensions.FirstOrDefaultAsync(collection, predicate, cancellationToken);
		}

		public Task<TSource> FirstOrDefaultAsync<TSource>(IQueryable<TSource> collection, CancellationToken cancellationToken = default(CancellationToken))
		{
			return QueryableExtensions.FirstOrDefaultAsync(collection, cancellationToken);
		}

		public Task<bool> AnyAsync<TSource>(IQueryable<TSource> collection, CancellationToken cancellationToken = default(CancellationToken))
		{
			return QueryableExtensions.AnyAsync(collection, cancellationToken);
		}

		public Task<bool> AnyAsync<TSource>(IQueryable<TSource> collection, Expression<Func<TSource, bool>> predicate, CancellationToken cancellationToken = default(CancellationToken))
		{
			return QueryableExtensions.AnyAsync(collection, predicate, cancellationToken);
		}

		public IQueryable<TSource> AsNoTracking<TSource>(IQueryable<TSource> collection, CancellationToken cancellationToken = default(CancellationToken)) where TSource : class
		{
			return QueryableExtensions.AsNoTracking(collection);
		}
	}
}
