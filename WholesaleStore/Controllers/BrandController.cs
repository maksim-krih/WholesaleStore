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
    public class BrandController : BaseController
    {
        public BrandController(
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

            var brandsQuery = _dataBaseManager.BrandRepository.Query.Include(b => b.Country);

            if (!String.IsNullOrEmpty(searchString))
            {
                brandsQuery = brandsQuery.Where(x => x.Name.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    brandsQuery = brandsQuery.OrderByDescending(x => x.Name);
                    break;
                case "Name":
                    brandsQuery = brandsQuery.OrderBy(s => s.Name);
                    break;
                case "country_desc":
                    brandsQuery = brandsQuery.OrderByDescending(x => x.Country.Name);
                    break;
                case "Country":
                    brandsQuery = brandsQuery.OrderBy(s => s.Country.Name);
                    break;
            }

            var brands = await _dataExecutor.ToListAsync(brandsQuery);

            int pageSize = 7;
            int pageNumber = (page ?? 1);

            return View(brands.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Create()
        {
            ViewBag.CountryId = new SelectList(_dataBaseManager.CountryRepository.Query, "Id", "Name");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Brand brand)
        {
            if (ModelState.IsValid)
            {
                _dataBaseManager.BrandRepository.Create(brand);

                await _dataBaseManager.BrandRepository.CommitAsync();

                return RedirectToAction("Index");
            }

            ViewBag.CountryId = new SelectList(_dataBaseManager.CountryRepository.Query, "Id", "Name", brand.CountryId);

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
        public async Task<ActionResult> Edit(Brand brand)
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

        [HttpPost]
        public async Task<bool> Delete(int id)
        {
            var brand = await _dataExecutor.FirstOrDefaultAsync(_dataBaseManager.BrandRepository.Query, x => x.Id == id);

            _dataBaseManager.BrandRepository.Remove(brand);

            await _dataBaseManager.BrandRepository.CommitAsync();

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
