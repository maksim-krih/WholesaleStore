using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace WholesaleStore.Data
{
    public class EfDbQuery<TSource> : IQueryable<TSource>
		 where TSource : class
	{
		public IQueryable<TSource> _query;

		public EfDbQuery(IQueryable<TSource> query)
		{
			_query = query;
		}

		public Type ElementType => _query.ElementType;

		public Expression Expression => _query.Expression;

		public static explicit operator EfDbQuery<TSource>(DbSet<TSource> v)
		{
			return new EfDbQuery<TSource>(v.AsQueryable());
		}

		public IQueryProvider Provider => _query.Provider;


		public IEnumerator<TSource> GetEnumerator()
		{
			return _query.GetEnumerator();
		}

		public IQueryable<TSource> Include(string path)
		{
			return new EfDbQuery<TSource>(QueryableExtensions.Include(this, path));
		}

		public IQueryable<TSource> Include<TProperty>(Expression<Func<TSource, TProperty>> navigationPropertyPath)
		{
			return new EfDbQuery<TSource>(QueryableExtensions.Include(this, navigationPropertyPath));
		}

		public IQueryable<TSource> Where(Expression<Func<TSource, bool>> predicate)
		{
			return new EfDbQuery<TSource>(Queryable.Where(this.AsQueryable(), predicate));
		}
		public IQueryable<TSource> Where(Expression<Func<TSource, int, bool>> predicate)
		{
			return new EfDbQuery<TSource>(Queryable.Where(this.AsQueryable(), predicate));
		}
		public IQueryable<TSource> OrderBy<TKey>(Expression<Func<TSource, TKey>> keySelector)
		{
			return new EfDbQuery<TSource>(Queryable.OrderBy(this.AsQueryable(), keySelector));
		}

		public IQueryable<TSource> OrderBy<OrderByTKey, ThenByTKey>(Expression<Func<TSource, OrderByTKey>> orderByKeySelector, Expression<Func<TSource, ThenByTKey>> thenByKeySelector)
		{
			return new EfDbQuery<TSource>(Queryable.ThenBy(Queryable.OrderBy(this.AsQueryable(), orderByKeySelector), thenByKeySelector));
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _query.GetEnumerator();
		}
	}
}