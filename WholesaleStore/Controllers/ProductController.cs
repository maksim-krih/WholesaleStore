using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using WholesaleStore.Controllers.Base;
using WholesaleStore.Data.Interfaces;

namespace WholesaleStore.Controllers
{
    public class ProductController : BaseController
    {
        public ProductController(
            IDataExecutor dataExecutor,
            IGridManager gridManager,
            IDataBaseManager dataBaseManager) : base(gridManager, dataExecutor, dataBaseManager)
        {

        }

        public async Task<ActionResult> Index()
        {
            var products = await _dataExecutor.ToListAsync(_dataBaseManager.ProductRepository.Query
                .Include(p => p.Brand)
                .Include(p => p.ProductType)
                );

            return View(products);
        }

        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var product = await _dataExecutor.FirstOrDefaultAsync(_dataBaseManager.ProductRepository.Query, x => x.Id == id);

            if (product == null)
            {
                return HttpNotFound();
            }

            return View(product);
        }

        public ActionResult Create()
        {
            ViewBag.BrandId = new SelectList(_dataBaseManager.BrandRepository.Query, "Id", "Name");
            ViewBag.ProductTypeId = new SelectList(_dataBaseManager.ProductTypeRepository.Query, "Id", "Name");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                _dataBaseManager.ProductRepository.Create(product);

                await _dataBaseManager.ProductRepository.CommitAsync();

                return RedirectToAction("Index");
            }

            ViewBag.BrandId = new SelectList(_dataBaseManager.BrandRepository.Query, "Id", "Name", product.BrandId);
            ViewBag.ProductTypeId = new SelectList(_dataBaseManager.ProductTypeRepository.Query, "Id", "Name", product.ProductTypeId);

            return View(product);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var product = await _dataExecutor.FirstOrDefaultAsync(_dataBaseManager.ProductRepository.Query, x => x.Id == id);

            if (product == null)
            {
                return HttpNotFound();
            }

            ViewBag.BrandId = new SelectList(_dataBaseManager.BrandRepository.Query, "Id", "Name", product.BrandId);
            ViewBag.ProductTypeId = new SelectList(_dataBaseManager.ProductTypeRepository.Query, "Id", "Name", product.ProductTypeId);

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                var entity = await _dataExecutor.FirstOrDefaultAsync(_dataBaseManager.ProductRepository.Query, x => x.Id == product.Id);

                entity.Code = product.Code;
                entity.ProductTypeId = product.ProductTypeId;
                entity.BrandId = product.BrandId;
                entity.Name = product.Name;
                entity.Description = product.Description;
                entity.AmountInPackage = product.AmountInPackage;
                entity.PackagePrice = product.PackagePrice;

                await _dataBaseManager.ProductRepository.CommitAsync();

                return RedirectToAction("Index");
            }

            ViewBag.BrandId = new SelectList(_dataBaseManager.BrandRepository.Query, "Id", "Name", product.BrandId);
            ViewBag.ProductTypeId = new SelectList(_dataBaseManager.ProductTypeRepository.Query, "Id", "Name", product.ProductTypeId);

            return View(product);
        }

        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var product = await _dataExecutor.FirstOrDefaultAsync(_dataBaseManager.ProductRepository.Query, x => x.Id == id);

            if (product == null)
            {
                return HttpNotFound();
            }

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var product = await _dataExecutor.FirstOrDefaultAsync(_dataBaseManager.ProductRepository.Query, x => x.Id == id);

            _dataBaseManager.ProductRepository.Remove(product);

            await _dataBaseManager.ProductRepository.CommitAsync();

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
