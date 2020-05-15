﻿using System.Data.Entity;
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

        public async Task<ActionResult> Index()
        {
            var cities = await _dataExecutor.ToListAsync(_dataBaseManager.CityRepository.Query.Include(c => c.Region.Country));

            return View(cities);
        }
        
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var city = await _dataExecutor.FirstOrDefaultAsync(
                _dataBaseManager.CityRepository.Query
                .Include(x => x.Region.Country),
                x => x.Id == id);
            
            if (city == null)
            {
                return HttpNotFound();
            }
            
            return View(city);
        }

        public ActionResult Create()
        {
            var city = new CityDto();

            ConfigureDto(city);

            return View(city);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Name,RegionId")] CityDto cityDto)
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
        public async Task<ActionResult> Edit([Bind(Include = "Id,Name,RegionId")] CityDto city)
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

        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var city = await _dataExecutor.FirstOrDefaultAsync(_dataBaseManager.CityRepository.Query, x => x.Id == id);
            
            if (city == null)
            {
                return HttpNotFound();
            }
            
            return View(city);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var city = await _dataExecutor.FirstOrDefaultAsync(_dataBaseManager.CityRepository.Query, x => x.Id == id);

            _dataBaseManager.CityRepository.Remove(city);

            await _dataBaseManager.CityRepository.CommitAsync();

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