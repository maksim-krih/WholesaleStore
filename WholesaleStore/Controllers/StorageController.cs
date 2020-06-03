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
using WholesaleStore.Utils;

namespace WholesaleStore.Controllers
{
    public class StorageController : BaseController
    {
        public StorageController(
            IDataExecutor dataExecutor,
            IGridManager gridManager,
            IDataBaseManager dataBaseManager) : base(gridManager, dataExecutor, dataBaseManager)
        {

        }

        public async Task<ActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NumberSortParm = sortOrder == "Number" ? "number_desc" : "Number";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var storagesQuery = _dataBaseManager.StorageRepository.Query
                .Include(x => x.Address.City.Region.Country);

            if (!String.IsNullOrEmpty(searchString))
            {
                storagesQuery = storagesQuery.Where(x => 
                x.Number.ToString().Contains(searchString));
            }

            switch (sortOrder)
            {
                case "number_desc":
                    storagesQuery = storagesQuery.OrderByDescending(x => x.Number);
                    break;
                case "Number":
                    storagesQuery = storagesQuery.OrderBy(s => s.Number);
                    break;
            }

            var storages = await _dataExecutor.ToListAsync(storagesQuery);

            int pageSize = 7;
            int pageNumber = (page ?? 1);

            return View(storages.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Create()
        {
            var supplier = new StorageDto();

            AddressHelper.ConfigureDto(_dataBaseManager, supplier);

            return View(supplier);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(StorageDto storageDto)
        {
            if (ModelState.IsValid)
            {
                var storage = new Storage
                {
                    Number = storageDto.Number,
                    ContactPhone = storageDto.ContactPhone,
                    Address = new Address
                    {
                        Address1 = storageDto.Address,
                        CityId = storageDto.CityId.Value,
                        ZipCode = storageDto.ZipCode
                    }
                };

                _dataBaseManager.StorageRepository.Create(storage);
                await _dataBaseManager.StorageRepository.CommitAsync();

                return RedirectToAction("Index");
            }

            AddressHelper.ConfigureDto(_dataBaseManager, storageDto);

            return View(storageDto);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var storage = await _dataExecutor.FirstOrDefaultAsync(_dataBaseManager.StorageRepository.Query.Include(x => x.Address.City.Region), x => x.Id == id);

            if (storage == null)
            {
                return HttpNotFound();
            }

            var storageDto = new StorageDto
            {
                Address = storage.Address.Address1,
                ZipCode = storage.Address.ZipCode,
                CityId = storage.Address.CityId,
                Number = storage.Number,
                ContactPhone = storage.ContactPhone,
                CountryId = storage.Address.City.Region.CountryId,
                Id = storage.Id,
                RegionId = storage.Address.City.RegionId
            };

            AddressHelper.ConfigureDto(_dataBaseManager, storageDto);

            return View(storageDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(StorageDto storage)
        {
            if (ModelState.IsValid)
            {
                var entity = await _dataExecutor.FirstOrDefaultAsync(_dataBaseManager.StorageRepository.Query.Include(x => x.Address), x => x.Id == storage.Id);

                entity.Address.Address1 = storage.Address;
                entity.Address.ZipCode = storage.ZipCode;
                entity.Address.CityId = storage.CityId.Value;
                entity.Number = storage.Number;
                entity.ContactPhone = storage.ContactPhone;

                await _dataBaseManager.StorageRepository.CommitAsync();

                return RedirectToAction("Index");
            }

            AddressHelper.ConfigureDto(_dataBaseManager, storage);

            return View(storage);
        }

        [HttpPost]
        public async Task<bool> Delete(int id)
        {
            var storage = await _dataExecutor.FirstOrDefaultAsync(_dataBaseManager.StorageRepository.Query, x => x.Id == id);

            _dataBaseManager.StorageRepository.Remove(storage);

            await _dataBaseManager.StorageRepository.CommitAsync();

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
