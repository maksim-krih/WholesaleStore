using PagedList;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using WholesaleStore.Controllers.Base;
using WholesaleStore.Data.Interfaces;

namespace WholesaleStore.Controllers
{
    public class CountryController : BaseController
    {
        public CountryController(
            IDataExecutor dataExecutor,
            IGridManager gridManager,
            IDataBaseManager dataBaseManager) : base(gridManager, dataExecutor, dataBaseManager)
        {

        }

        public async Task<ActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = sortOrder == "Name" ? "name_desc" : "Name";
            
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var countriesQuery = _dataBaseManager.CountryRepository.Query;

            if (!String.IsNullOrEmpty(searchString))
            {
                countriesQuery = countriesQuery.Where(x => x.Name.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    countriesQuery = countriesQuery.OrderByDescending(x => x.Name);
                    break;
                case "Name":
                    countriesQuery = countriesQuery.OrderBy(s => s.Name);
                    break;
            }

            var countries = await _dataExecutor.ToListAsync(countriesQuery);

            int pageSize = 7;
            int pageNumber = (page ?? 1);

            return View(countries.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Country country)
        {
            if (ModelState.IsValid)
            {
                _dataBaseManager.CountryRepository.Create(country);
                await _dataBaseManager.CountryRepository.CommitAsync();

                return RedirectToAction("Index");
            }

            return View(country);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var country = await _dataExecutor.FirstOrDefaultAsync(_dataBaseManager.CountryRepository.Query, x => x.Id == id);

            if (country == null)
            {
                return HttpNotFound();
            }

            return View(country);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Country country)
        {
            if (ModelState.IsValid)
            {
                var entity = await _dataExecutor.FirstOrDefaultAsync(_dataBaseManager.CountryRepository.Query, x => x.Id == country.Id);

                entity.Name = country.Name;

                await _dataBaseManager.CountryRepository.CommitAsync();

                return RedirectToAction("Index");
            }

            return View(country);
        }

        [HttpPost]
        public async Task<bool> Delete(int id)
        {
            var country = await _dataExecutor.FirstOrDefaultAsync(_dataBaseManager.CountryRepository.Query, x => x.Id == id);

            _dataBaseManager.CountryRepository.Remove(country);

            await _dataBaseManager.CountryRepository.CommitAsync();

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
