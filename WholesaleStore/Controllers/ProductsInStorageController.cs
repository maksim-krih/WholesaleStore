using System.Data.Entity;
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

        public async Task<ActionResult> Index()
        {
            var productsInStorages = await _dataExecutor.ToListAsync(_dataBaseManager.ProductsInStorageRepository.Query.Include(p => p.Product).Include(p => p.Storage));

            return View(productsInStorages);
        }

        public ActionResult Create()
        {
            ViewBag.ProductId = new SelectList(_dataBaseManager.ProductRepository.Query, "Id", "Name");
            ViewBag.StorageId = new SelectList(_dataBaseManager.StorageRepository.Query, "Id", "ContactPhone");

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

            ViewBag.ProductId = new SelectList(_dataBaseManager.ProductRepository.Query, "Id", "Name", productsInStorage.ProductId);
            ViewBag.StorageId = new SelectList(_dataBaseManager.StorageRepository.Query, "Id", "ContactPhone", productsInStorage.StorageId);

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

            ViewBag.ProductId = new SelectList(_dataBaseManager.ProductRepository.Query, "Id", "Name", productsInStorage.ProductId);
            ViewBag.StorageId = new SelectList(_dataBaseManager.StorageRepository.Query, "Id", "ContactPhone", productsInStorage.StorageId);

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

            ViewBag.ProductId = new SelectList(_dataBaseManager.ProductRepository.Query, "Id", "Name", productsInStorage.ProductId);
            ViewBag.StorageId = new SelectList(_dataBaseManager.StorageRepository.Query, "Id", "ContactPhone", productsInStorage.StorageId);

            return View(productsInStorage);
        }

        public async Task<ActionResult> Delete(int? id)
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

            return View(productsInStorage);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var productsInStorage = await _dataExecutor.FirstOrDefaultAsync(_dataBaseManager.ProductsInStorageRepository.Query.Include(p => p.Product).Include(p => p.Storage));

            _dataBaseManager.ProductsInStorageRepository.Remove(productsInStorage);

            await _dataBaseManager.ProductsInStorageRepository.CommitAsync();

            return RedirectToAction("Index");
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
