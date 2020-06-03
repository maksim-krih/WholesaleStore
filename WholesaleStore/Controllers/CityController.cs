using PagedList;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using WholesaleStore.Controllers.Base;
using WholesaleStore.Data.Interfaces;
using WholesaleStore.Models.Dtos;

namespace WholesaleStore.Controllers
{
    public class CityController : BaseController
    {
        public CityController(
            IDataExecutor dataExecutor,
            IGridManager gridManager,
            IDataBaseManager dataBaseManager) : base(gridManager, dataExecutor, dataBaseManager)
        {

        }

        public async Task<ActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = sortOrder == "Name" ? "name_desc" : "Name";
            ViewBag.RegionSortParm = sortOrder == "Region" ? "region_desc" : "Region";
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

            var citiesQuery = _dataBaseManager.CityRepository.Query
                .Include(c => c.Region.Country);

            if (!String.IsNullOrEmpty(searchString))
            {
                citiesQuery = citiesQuery.Where(x => x.Name.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    citiesQuery = citiesQuery.OrderByDescending(x => x.Name);
                    break;
                case "Name":
                    citiesQuery = citiesQuery.OrderBy(s => s.Name);
                    break;
                case "region_desc":
                    citiesQuery = citiesQuery.OrderByDescending(x => x.Region.Name);
                    break;
                case "Region":
                    citiesQuery = citiesQuery.OrderBy(s => s.Region.Name);
                    break;
                case "country_desc":
                    citiesQuery = citiesQuery.OrderByDescending(x => x.Region.Country.Name);
                    break;
                case "Country":
                    citiesQuery = citiesQuery.OrderBy(s => s.Region.Country.Name);
                    break;
            }

            var cities = await _dataExecutor.ToListAsync(citiesQuery);

            int pageSize = 7;
            int pageNumber = (page ?? 1);

            return View(cities.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Create()
        {
            var city = new CityDto();

            ConfigureDto(city);

            return View(city);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CityDto cityDto)
        {
            if (ModelState.IsValid)
            {
                var city = new City
                {
                    Name = cityDto.Name,
                    RegionId = cityDto.RegionId.Value
                };

                _dataBaseManager.CityRepository.Create(city);
                await _dataBaseManager.CityRepository.CommitAsync();

                return RedirectToAction("Index");
            }

            ConfigureDto(cityDto);

            return View(cityDto);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var city = await _dataExecutor.FirstOrDefaultAsync(_dataBaseManager.CityRepository.Query.Include(x => x.Region), x => x.Id == id);

            if (city == null)
            {
                return HttpNotFound();
            }

            var cityDto = new CityDto
            {
                Name = city.Name,
                CountryId = city.Region.CountryId,
                Id = city.Id,
                RegionId = city.RegionId
            };

            ConfigureDto(cityDto);

            return View(cityDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(CityDto city)
        {
            if (ModelState.IsValid)
            {
                var entity = await _dataExecutor.FirstOrDefaultAsync(_dataBaseManager.CityRepository.Query, x => x.Id == city.Id);

                entity.Name = city.Name;
                entity.RegionId = city.RegionId.Value;

                await _dataBaseManager.BrandRepository.CommitAsync();

                return RedirectToAction("Index");
            }

            ConfigureDto(city);

            return View(city);
        }

        [HttpPost]
        public async Task<bool> Delete(int id)
        {
            var city = await _dataExecutor.FirstOrDefaultAsync(_dataBaseManager.CityRepository.Query, x => x.Id == id);

            _dataBaseManager.CityRepository.Remove(city);

            await _dataBaseManager.CityRepository.CommitAsync();

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

        private void ConfigureDto(CityDto model)
        {
            model.CountryList = new SelectList(_dataBaseManager.CountryRepository.Query, "Id", "Name");

            if (model.CountryId.HasValue)
            {
                model.RegionList = new SelectList(_dataBaseManager.RegionRepository.Query.Where(x => x.CountryId == model.CountryId.Value), "Id", "Name");
            }
            else
            {
                model.RegionList = new SelectList(Enumerable.Empty<SelectListItem>());
            }
        }
    }
}
