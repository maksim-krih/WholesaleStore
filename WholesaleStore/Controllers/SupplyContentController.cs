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
    public class SupplyContentController : BaseController
    {
        public SupplyContentController(
            IDataExecutor dataExecutor,
            IGridManager gridManager,
            IDataBaseManager dataBaseManager) : base(gridManager, dataExecutor, dataBaseManager)
        {

        }

        public async Task<ActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.SupplySortParm = sortOrder == "Supply" ? "supply_desc" : "Supply";
            ViewBag.ProductSortParm = sortOrder == "Product" ? "product_desc" : "Product";
            ViewBag.CountSortParm = sortOrder == "Count" ? "count_desc" : "Count";
            ViewBag.PriceSortParm = sortOrder == "Price" ? "price_desc" : "Price";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var supplyContentsQuery = _dataBaseManager.SupplyContentRepository.Query
                .Include(s => s.Product).Include(s => s.Supply);


            if (!String.IsNullOrEmpty(searchString))
            {
                supplyContentsQuery = supplyContentsQuery.Where(x => x.Product.Name.Contains(searchString) ||
                    x.Product.Code.ToString().Contains(searchString));
            }

            switch (sortOrder)
            {
                case "supply_desc":
                    supplyContentsQuery = supplyContentsQuery.OrderByDescending(x => x.Supply.Number);
                    break;
                case "Supply":
                    supplyContentsQuery = supplyContentsQuery.OrderBy(s => s.Supply.Number);
                    break;
                case "product_desc":
                    supplyContentsQuery = supplyContentsQuery.OrderByDescending(x => x.Product.Code);
                    break;
                case "Product":
                    supplyContentsQuery = supplyContentsQuery.OrderBy(s => s.Product.Code);
                    break;
                case "count_desc":
                    supplyContentsQuery = supplyContentsQuery.OrderByDescending(x => x.Count);
                    break;
                case "Count":
                    supplyContentsQuery = supplyContentsQuery.OrderBy(s => s.Count);
                    break;
                case "price_desc":
                    supplyContentsQuery = supplyContentsQuery.OrderByDescending(x => x.SupplyPrice);
                    break;
                case "Price":
                    supplyContentsQuery = supplyContentsQuery.OrderBy(s => s.SupplyPrice);
                    break;
            }

            var supplyContents = await _dataExecutor.ToListAsync(supplyContentsQuery);

            int pageSize = 7;
            int pageNumber = (page ?? 1);

            return View(supplyContents.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Create()
        {
            ViewBag.ProductId = new SelectList(_dataBaseManager.ProductRepository.Query, "Id", "FullName");
            ViewBag.SupplyId = new SelectList(_dataBaseManager.SupplyRepository.Query, "Id", "Number");
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(SupplyContent supplyContent)
        {
            if (ModelState.IsValid)
            {
                _dataBaseManager.SupplyContentRepository.Create(supplyContent);

                await _dataBaseManager.SupplyContentRepository.CommitAsync();

                return RedirectToAction("Index");
            }

            ViewBag.ProductId = new SelectList(_dataBaseManager.ProductRepository.Query, "Id", "FullName", supplyContent.ProductId);
            ViewBag.SupplyId = new SelectList(_dataBaseManager.SupplyRepository.Query, "Id", "Number", supplyContent.SupplyId);
            
            return View(supplyContent);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var supplyContent = await _dataExecutor.FirstOrDefaultAsync(
                _dataBaseManager.SupplyContentRepository.Query
                .Include(s => s.Product)
                .Include(s => s.Supply),
                x => x.Id == id);

            if (supplyContent == null)
            {
                return HttpNotFound();
            }
            
            ViewBag.ProductId = new SelectList(_dataBaseManager.ProductRepository.Query, "Id", "FullName", supplyContent.ProductId);
            ViewBag.SupplyId = new SelectList(_dataBaseManager.SupplyRepository.Query, "Id", "Number", supplyContent.SupplyId);
            
            return View(supplyContent);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(SupplyContent supplyContent)
        {
            if (ModelState.IsValid)
            {
                var entity = await _dataExecutor.FirstOrDefaultAsync(
                _dataBaseManager.SupplyContentRepository.Query
                .Include(s => s.Product)
                .Include(s => s.Supply),
                x => x.Id == supplyContent.Id);

                entity.SupplyId = supplyContent.SupplyId;
                entity.ProductId = supplyContent.ProductId;
                entity.Count = supplyContent.Count;
                entity.SupplyPrice = supplyContent.SupplyPrice;

                await _dataBaseManager.SupplyContentRepository.CommitAsync();

                return RedirectToAction("Index");
            }
            
            ViewBag.ProductId = new SelectList(_dataBaseManager.ProductRepository.Query, "Id", "FullName", supplyContent.ProductId);
            ViewBag.SupplyId = new SelectList(_dataBaseManager.SupplyRepository.Query, "Id", "Number", supplyContent.SupplyId);
            
            return View(supplyContent);
        }

        [HttpPost]
        public async Task<bool> Delete(int id)
        {
            var supplyContent = await _dataExecutor.FirstOrDefaultAsync(
                _dataBaseManager.SupplyContentRepository.Query
                .Include(s => s.Product)
                .Include(s => s.Supply),
                x => x.Id == id);

            _dataBaseManager.SupplyContentRepository.Remove(supplyContent);

            await _dataBaseManager.SupplyContentRepository.CommitAsync();

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
