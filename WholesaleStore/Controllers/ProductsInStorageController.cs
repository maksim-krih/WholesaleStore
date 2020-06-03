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
    public class ProductsInStorageController : BaseController
    {
        public ProductsInStorageController(
            IDataExecutor dataExecutor,
            IGridManager gridManager,
            IDataBaseManager dataBaseManager) : base(gridManager, dataExecutor, dataBaseManager)
        {

        }

        public async Task<ActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.AmountSortParm = sortOrder == "Amount" ? "amount_desc" : "Amount";
            ViewBag.ProductSortParm = sortOrder == "Product" ? "product_desc" : "Product";
            ViewBag.StorageSortParm = sortOrder == "Storage" ? "storage_desc" : "Storage";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var productsInStoragesQuery = _dataBaseManager.ProductsInStorageRepository.Query
                .Include(p => p.Product).Include(p => p.Storage);

            if (!String.IsNullOrEmpty(searchString))
            {
                productsInStoragesQuery = productsInStoragesQuery.Where(x => x.Amount.ToString().Contains(searchString) ||
                    x.Product.Code.ToString().Contains(searchString) || x.Storage.Number.ToString().Contains(searchString));
            }

            switch (sortOrder)
            {
                case "amount_desc":
                    productsInStoragesQuery = productsInStoragesQuery.OrderByDescending(x => x.Amount);
                    break;
                case "Amount":
                    productsInStoragesQuery = productsInStoragesQuery.OrderBy(s => s.Amount);
                    break;
                case "product_desc":
                    productsInStoragesQuery = productsInStoragesQuery.OrderByDescending(x => x.Product.Code);
                    break;
                case "Product":
                    productsInStoragesQuery = productsInStoragesQuery.OrderBy(s => s.Product.Code);
                    break;
                case "storage_desc":
                    productsInStoragesQuery = productsInStoragesQuery.OrderByDescending(x => x.Storage.Number);
                    break;
                case "Storage":
                    productsInStoragesQuery = productsInStoragesQuery.OrderBy(s => s.Storage.Number);
                    break;
            }

            var productsInStorages = await _dataExecutor.ToListAsync(productsInStoragesQuery);

            int pageSize = 7;
            int pageNumber = (page ?? 1);

            return View(productsInStorages.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Create()
        {
            ViewBag.ProductId = new SelectList(_dataBaseManager.ProductRepository.Query, "Id", "FullName");
            ViewBag.StorageId = new SelectList(_dataBaseManager.StorageRepository.Query, "Id", "Number");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ProductsInStorage productsInStorage)
        {
            if (ModelState.IsValid)
            {
                _dataBaseManager.ProductsInStorageRepository.Create(productsInStorage);
                await _dataBaseManager.ProductsInStorageRepository.CommitAsync();

                return RedirectToAction("Index");
            }

            ViewBag.ProductId = new SelectList(_dataBaseManager.ProductRepository.Query, "Id", "FullName", productsInStorage.ProductId);
            ViewBag.StorageId = new SelectList(_dataBaseManager.StorageRepository.Query, "Id", "Number", productsInStorage.StorageId);

            return View(productsInStorage);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var productsInStorage = await _dataExecutor.FirstOrDefaultAsync(_dataBaseManager.ProductsInStorageRepository.Query.Include(p => p.Product).Include(p => p.Storage));

            if (productsInStorage == null)
            {
                return HttpNotFound();
            }

            ViewBag.ProductId = new SelectList(_dataBaseManager.ProductRepository.Query, "Id", "FullName", productsInStorage.ProductId);
            ViewBag.StorageId = new SelectList(_dataBaseManager.StorageRepository.Query, "Id", "Number", productsInStorage.StorageId);

            return View(productsInStorage);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ProductsInStorage productsInStorage)
        {
            if (ModelState.IsValid)
            {
                var entity = await _dataExecutor.FirstOrDefaultAsync(_dataBaseManager.ProductsInStorageRepository.Query.Include(p => p.Product).Include(p => p.Storage));

                entity.Amount = productsInStorage.Amount;
                entity.ProductId = productsInStorage.ProductId;
                entity.StorageId = productsInStorage.StorageId;

                await _dataBaseManager.ProductsInStorageRepository.CommitAsync();

                return RedirectToAction("Index");
            }

            ViewBag.ProductId = new SelectList(_dataBaseManager.ProductRepository.Query, "Id", "FullName", productsInStorage.ProductId);
            ViewBag.StorageId = new SelectList(_dataBaseManager.StorageRepository.Query, "Id", "Number", productsInStorage.StorageId);

            return View(productsInStorage);
        }

        [HttpPost]
        public async Task<bool> Delete(int id)
        {
            var productsInStorage = await _dataExecutor.FirstOrDefaultAsync(_dataBaseManager.ProductsInStorageRepository.Query.Include(p => p.Product).Include(p => p.Storage));

            _dataBaseManager.ProductsInStorageRepository.Remove(productsInStorage);

            await _dataBaseManager.ProductsInStorageRepository.CommitAsync();

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
