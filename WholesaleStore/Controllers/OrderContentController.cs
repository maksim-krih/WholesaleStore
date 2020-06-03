using PagedList;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using WholesaleStore.Controllers.Base;
using WholesaleStore.Data.Interfaces;

namespace WholesaleStore.Controllers
{
    public class OrderContentController : BaseController
    {
        public OrderContentController(
            IDataExecutor dataExecutor,
            IGridManager gridManager,
            IDataBaseManager dataBaseManager) : base(gridManager, dataExecutor, dataBaseManager)
        {

        }

        public async Task<ActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.OrderSortParm = sortOrder == "Order" ? "order_desc" : "Order";
            ViewBag.ProductSortParm = sortOrder == "Product" ? "product_desc" : "Product";
            ViewBag.CountSortParm = sortOrder == "Count" ? "count_desc" : "Count";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var orderContentsQuery = _dataBaseManager.OrderContentRepository.Query
                .Include(o => o.Order).Include(o => o.Product);

            if (!String.IsNullOrEmpty(searchString))
            {
                orderContentsQuery = orderContentsQuery.Where(x => x.Product.Name.Contains(searchString) ||
                    x.Product.Code.ToString().Contains(searchString));
            }

            switch (sortOrder)
            {
                case "order_desc":
                    orderContentsQuery = orderContentsQuery.OrderByDescending(x => x.Order.Number);
                    break;
                case "Order":
                    orderContentsQuery = orderContentsQuery.OrderBy(s => s.Order.Number);
                    break;
                case "product_desc":
                    orderContentsQuery = orderContentsQuery.OrderByDescending(x => x.Product.Code);
                    break;
                case "Product":
                    orderContentsQuery = orderContentsQuery.OrderBy(s => s.Product.Code);
                    break;
                case "count_desc":
                    orderContentsQuery = orderContentsQuery.OrderByDescending(x => x.Count);
                    break;
                case "Count":
                    orderContentsQuery = orderContentsQuery.OrderBy(s => s.Count);
                    break;
            }

            var orderContents = await _dataExecutor.ToListAsync(orderContentsQuery);

            int pageSize = 7;
            int pageNumber = (page ?? 1);

            return View(orderContents.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Create()
        {
            ViewBag.OrderId = new SelectList(_dataBaseManager.OrderRepository.Query, "Id", "Number");
            ViewBag.ProductId = new SelectList(_dataBaseManager.ProductRepository.Query, "Id", "FullName");
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(OrderContent orderContent)
        {
            if (ModelState.IsValid)
            {
                _dataBaseManager.OrderContentRepository.Create(orderContent);

                await _dataBaseManager.OrderContentRepository.CommitAsync();

                return RedirectToAction("Index");
            }

            ViewBag.OrderId = new SelectList(_dataBaseManager.OrderRepository.Query, "Id", "Number", orderContent.OrderId);
            ViewBag.ProductId = new SelectList(_dataBaseManager.ProductRepository.Query, "Id", "FullName", orderContent.ProductId);
            
            return View(orderContent);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var orderContent = await _dataExecutor.FirstOrDefaultAsync(_dataBaseManager.OrderContentRepository.Query.Include(o => o.Order).Include(o => o.Product), x => x.Id == id);

            if (orderContent == null)
            {
                return HttpNotFound();
            }
            
            ViewBag.OrderId = new SelectList(_dataBaseManager.OrderRepository.Query, "Id", "Number", orderContent.OrderId);
            ViewBag.ProductId = new SelectList(_dataBaseManager.ProductRepository.Query, "Id", "FullName", orderContent.ProductId);
            
            return View(orderContent);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(OrderContent orderContent)
        {
            if (ModelState.IsValid)
            {
                var entity = await _dataExecutor.FirstOrDefaultAsync(_dataBaseManager.OrderContentRepository.Query, x => x.Id == orderContent.Id);

                entity.OrderId = orderContent.OrderId;
                entity.ProductId = orderContent.ProductId;
                entity.Count = orderContent.Count;

                await _dataBaseManager.OrderContentRepository.CommitAsync();

                return RedirectToAction("Index");
            }
            
            ViewBag.OrderId = new SelectList(_dataBaseManager.OrderRepository.Query, "Id", "Number", orderContent.OrderId);
            ViewBag.ProductId = new SelectList(_dataBaseManager.ProductRepository.Query, "Id", "FullName", orderContent.ProductId);
            
            return View(orderContent);
        }

        [HttpPost]
        public async Task<bool> Delete(int id)
        {
            var orderContent = await _dataExecutor.FirstOrDefaultAsync(_dataBaseManager.OrderContentRepository.Query.Include(o => o.Order).Include(o => o.Product), x => x.Id == id);

            _dataBaseManager.OrderContentRepository.Remove(orderContent);

            await _dataBaseManager.OrderContentRepository.CommitAsync();

            return true;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _dataBaseManager.Dispose();
            }
            
            base.Dispose(disposing);
        }
    }
}
