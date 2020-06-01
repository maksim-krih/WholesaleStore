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
    public class SupplierController : BaseController
    {
        public SupplierController(
            IDataExecutor dataExecutor,
            IGridManager gridManager,
            IDataBaseManager dataBaseManager) : base(gridManager, dataExecutor, dataBaseManager)
        {

        }

        public async Task<ActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.CompanyNameSortParm = sortOrder == "CompanyName" ? "companyName_desc" : "CompanyName";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var suppliersQuery = _dataBaseManager.SupplierRepository.Query
                .Include(x => x.Address.City.Region.Country);

            if (!String.IsNullOrEmpty(searchString))
            {
                suppliersQuery = suppliersQuery.Where(x => x.CompanyName.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "companyName_desc":
                    suppliersQuery = suppliersQuery.OrderByDescending(x => x.CompanyName);
                    break;
                case "CompanyName":
                    suppliersQuery = suppliersQuery.OrderBy(s => s.CompanyName);
                    break;
            }

            var suppliers = await _dataExecutor.ToListAsync(suppliersQuery);

            int pageSize = 7;
            int pageNumber = (page ?? 1);

            return View(suppliers.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Create()
        {
            var supplier = new SupplierDto();

            AddressHelper.ConfigureDto(_dataBaseManager, supplier);

            return View(supplier);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(SupplierDto supplierDto)
        {
            if (ModelState.IsValid)
            {
                var supplier = new Supplier
                {
                    CompanyName = supplierDto.CompanyName,
                    ContactPhone = supplierDto.ContactPhone,
                    Address = new Address
                    {
                        Address1 = supplierDto.Address,
                        CityId = supplierDto.CityId.Value,
                        ZipCode = supplierDto.ZipCode
                    }
                };

                _dataBaseManager.SupplierRepository.Create(supplier);
                await _dataBaseManager.SupplierRepository.CommitAsync();

                return RedirectToAction("Index");
            }

            AddressHelper.ConfigureDto(_dataBaseManager, supplierDto);

            return View(supplierDto);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var supplier = await _dataExecutor.FirstOrDefaultAsync(_dataBaseManager.SupplierRepository.Query.Include(x => x.Address.City.Region), x => x.Id == id);

            if (supplier == null)
            {
                return HttpNotFound();
            }

            var supplierDto = new SupplierDto
            {
                Address = supplier.Address.Address1,
                ZipCode = supplier.Address.ZipCode,
                CityId = supplier.Address.CityId,
                CompanyName = supplier.CompanyName,
                ContactPhone = supplier.ContactPhone,
                CountryId = supplier.Address.City.Region.CountryId,
                Id = supplier.Id,
                RegionId = supplier.Address.City.RegionId
            };

            AddressHelper.ConfigureDto(_dataBaseManager, supplierDto);

            return View(supplierDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(SupplierDto supplier)
        {
            if (ModelState.IsValid)
            {
                var entity = await _dataExecutor.FirstOrDefaultAsync(_dataBaseManager.SupplierRepository.Query.Include(x => x.Address), x => x.Id == supplier.Id);

                entity.Address.Address1 = supplier.Address;
                entity.Address.ZipCode = supplier.ZipCode;
                entity.Address.CityId = supplier.CityId.Value;
                entity.CompanyName = supplier.CompanyName;
                entity.ContactPhone = supplier.ContactPhone;

                await _dataBaseManager.BrandRepository.CommitAsync();

                return RedirectToAction("Index");
            }

            AddressHelper.ConfigureDto(_dataBaseManager, supplier);

            return View(supplier);
        }

        [HttpPost]
        public async Task<bool> Delete(int id)
        {
            var supplier = await _dataExecutor.FirstOrDefaultAsync(_dataBaseManager.SupplierRepository.Query, x => x.Id == id);

            _dataBaseManager.SupplierRepository.Remove(supplier);

            await _dataBaseManager.SupplierRepository.CommitAsync();

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
