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
    public class RegionController : BaseController
    {
        private WholesaleStoreContext db = new WholesaleStoreContext();

        public RegionController(
            IDataExecutor dataExecutor,
            IGridManager gridManager,
            IDataBaseManager dataBaseManager) : base(gridManager, dataExecutor, dataBaseManager)
        {

        }

        public async Task<ActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = sortOrder == "Name" ? "name_desc" : "Name";
            ViewBag.CountrySortParm = sortOrder == "Country" ? "country_desc" : "Country";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var regionsQuery = _dataBaseManager.RegionRepository.Query.Include(b => b.Country);

            if (!String.IsNullOrEmpty(searchString))
            {
                regionsQuery = regionsQuery.Where(x => x.Name.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    regionsQuery = regionsQuery.OrderByDescending(x => x.Name);
                    break;
                case "Name":
                    regionsQuery = regionsQuery.OrderBy(s => s.Name);
                    break;
                case "country_desc":
                    regionsQuery = regionsQuery.OrderByDescending(x => x.Country.Name);
                    break;
                case "Country":
                    regionsQuery = regionsQuery.OrderBy(s => s.Country.Name);
                    break;
            }

            var regions = await _dataExecutor.ToListAsync(regionsQuery);

            int pageSize = 7;
            int pageNumber = (page ?? 1);

            return View(regions.ToPagedList(pageNumber, pageSize));
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
