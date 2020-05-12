using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading.Tasks;
using WholesaleStore.Common;
using WholesaleStore.Data.Interfaces;

namespace WholesaleStore.Data
{
    public class GridManager : IGridManager
    {

        public Task<int> GetTotal<TSource>(IQueryable<TSource> collection)
        {
            return collection.CountAsync();
        }

        public async Task<IEnumerable<TSource>> Paging<TSource>(IQueryable<TSource> collection, int page, int size)
        {
            return await collection.Skip(size * (page - 1)).Take(size).ToListAsync();
        }

        public IQueryable<TSource> Sorting<TSource>(IQueryable<TSource> collection, IEnumerable<GridSortItem> sortItems)
        {
            if (sortItems != null && sortItems.Count() > 0)
                return sortItems.Aggregate(collection,
                    (current, sortItem) => current.OrderBy($"{sortItem.Field} {SortOrder(sortItem.Order)}"));
            return collection;
        }

        private string SortOrder(string operation)
        {
            switch (operation)
            {
                case "ascend":
                    return "asc";
                case "descend":
                    return "desc";
                default:
                    return string.Empty;
            }
        }
    }
}