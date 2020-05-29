using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using WholesaleStore.Controllers.Base;
using WholesaleStore.Data.Interfaces;

namespace WholesaleStore.Controllers
{
    public class RegionController : BaseController
    {
        private WholesaleStoreContext db = new WholesaleStoreContext();

        public RegionController(
            IDataExecutor dataExecutor,
            IGridManager gridManager,
            IDataBaseManager dataBaseManager) : base(gridManager, dataExecutor, dataBaseManager)
        {

        }

        public async Task<ActionResult> Index()
        {
            var regions = await _dataExecutor.ToListAsync(_dataBaseManager.RegionRepository.Query.Include(b => b.Country));

            return View(regions);
        }

        public ActionResult Create()
        {
            ViewBag.CountryId = new SelectList(_dataBaseManager.CountryRepository.Query, "Id", "Name");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Region region)
        {
            if (ModelState.IsValid)
            {
                _dataBaseManager.RegionRepository.Create(region);
                await _dataBaseManager.RegionRepository.CommitAsync();

                return RedirectToAction("Index");
            }

            ViewBag.CountryId = new SelectList(_dataBaseManager.CountryRepository.Query, "Id", "Name", region.CountryId);

            return View(region);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var region = await _dataExecutor.FirstOrDefaultAsync(_dataBaseManager.RegionRepository.Query, x => x.Id == id);

            if (region == null)
            {
                return HttpNotFound();
            }

            ViewBag.CountryId = new SelectList(_dataBaseManager.CountryRepository.Query, "Id", "Name", region.CountryId);

            return View(region);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Region region)
        {
            if (ModelState.IsValid)
            {
                var entity = await _dataExecutor.FirstOrDefaultAsync(_dataBaseManager.RegionRepository.Query, x => x.Id == region.Id);

                entity.Name = region.Name;
                entity.CountryId = region.CountryId;

                await _dataBaseManager.RegionRepository.CommitAsync();

                return RedirectToAction("Index");
            }

            ViewBag.CountryId = new SelectList(_dataBaseManager.CountryRepository.Query, "Id", "Name", region.CountryId);

            return View(region);
        }

        [HttpPost]
        public async Task<bool> Delete(int id)
        {
            var region = await _dataExecutor.FirstOrDefaultAsync(_dataBaseManager.RegionRepository.Query, x => x.Id == id);

            _dataBaseManager.RegionRepository.Remove(region);

            await _dataBaseManager.RegionRepository.CommitAsync();

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
