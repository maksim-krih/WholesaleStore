using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WholesaleStore.Common;

namespace WholesaleStore.Data.Interfaces
{
    public interface IGridManager
    {
        Task<int> GetTotal<TSource>(IQueryable<TSource> collection);

        Task<IEnumerable<TSource>> Paging<TSource>(IQueryable<TSource> collection, int page, int size);

        IQueryable<TSource> Sorting<TSource>(IQueryable<TSource> collection, IEnumerable<GridSortItem> sortItems);
    }
}