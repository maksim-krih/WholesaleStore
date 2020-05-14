using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using WholesaleStore.Controllers.Base;
using WholesaleStore.Data.Interfaces;

namespace WholesaleStore.Controllers
{
    public class BrandController : BaseController
    {
        public BrandController(
            IDataExecutor dataExecutor,
            IGridManager gridManager,
            IDataBaseManager dataBaseManager) : base(gridManager, dataExecutor, dataBaseManager)
        {

        }

        public async Task<ActionResult> Index()
        {
            var brands = await _dataExecutor.ToListAsync(_dataBaseManager.BrandRepository.Query.Include(b => b.Country));
            
            return View(brands);
        }

        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var brand = await _dataExecutor.FirstOrDefaultAsync(_dataBaseManager.BrandRepository.Query, x => x.Id == id);

            if (brand == null)
            {
                return HttpNotFound();
            }
            
            return View(brand);
        }

        public ActionResult Create()
        {
            ViewBag.CountryId = new SelectList(_dataBaseManager.CountryRepository.Query, "Id", "Name");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Name,Description,CountryId")] Brand brand)
        {
            if (ModelState.IsValid)
            {
                _dataBaseManager.BrandRepository.Create(brand);
                await _dataBaseManager.BrandRepository.CommitAsync();

                return RedirectToAction("Index");
            }

            ViewBag.CountryId = new SelectList(_dataBaseManager.CountryRepository.Query, "Id", "Name");

            return View(brand);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var brand = await _dataExecutor.FirstOrDefaultAsync(_dataBaseManager.BrandRepository.Query, x => x.Id == id);

            if (brand == null)
            {
                return HttpNotFound();
            }
            
            ViewBag.CountryId = new SelectList(_dataBaseManager.CountryRepository.Query, "Id", "Name", brand.CountryId);
            
            return View(brand);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,Description,CountryId")] Brand brand)
        {
            if (ModelState.IsValid)
            {
                var entity = await _dataExecutor.FirstOrDefaultAsync(_dataBaseManager.BrandRepository.Query, x => x.Id == brand.Id);

                entity.Name = brand.Name;
                entity.Description = brand.Description;
                entity.CountryId = brand.CountryId;

                await _dataBaseManager.BrandRepository.CommitAsync();

                return RedirectToAction("Index");
            }

            ViewBag.CountryId = new SelectList(_dataBaseManager.CountryRepository.Query, "Id", "Name", brand.CountryId);

            return View(brand);
        }
        
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            var brand = await _dataExecutor.FirstOrDefaultAsync(_dataBaseManager.BrandRepository.Query, x => x.Id == id);

            if (brand == null)
            {
                return HttpNotFound();
            }

            return View(brand);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var brand = await _dataExecutor.FirstOrDefaultAsync(_dataBaseManager.BrandRepository.Query, x => x.Id == id);

            _dataBaseManager.BrandRepository.Remove(brand);

            await _dataBaseManager.BrandRepository.CommitAsync();

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
