using System.Data.Entity;
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

        public async Task<ActionResult> Index()
        {
            var suppliers = await _dataExecutor.ToListAsync(_dataBaseManager.SupplierRepository.Query.Include(x => x.Address.City.Region.Country));

            return View(suppliers);
        }

        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var supplier = await _dataExecutor.FirstOrDefaultAsync(
                _dataBaseManager.SupplierRepository.Query
                .Include(x => x.Address.City.Region.Country),
                x => x.Id == id);

            if (supplier == null)
            {
                return HttpNotFound();
            }
            return View(supplier);
        }

        public ActionResult Create()
        {
            var supplier = new SupplierDto();

            AddressHelper.ConfigureDto(_dataBaseManager, supplier);

            return View(supplier);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "CompanyName,Address,ContactPhone,CityId,ZipCode")] SupplierDto supplierDto)
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
                await _dataBaseManager.BrandRepository.CommitAsync();

                return RedirectToAction("Index");
            }

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
        public async Task<ActionResult> Edit([Bind(Include = "Id,CompanyName,Address,ContactPhone,CityId,ZipCode")] SupplierDto supplier)
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

        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            var supplier = await _dataExecutor.FirstOrDefaultAsync(_dataBaseManager.SupplierRepository.Query, x => x.Id == id);

            if (supplier == null)
            {
                return HttpNotFound();
            }
            
            return View(supplier);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var supplier = await _dataExecutor.FirstOrDefaultAsync(_dataBaseManager.SupplierRepository.Query, x => x.Id == id);

            _dataBaseManager.SupplierRepository.Remove(supplier);

            await _dataBaseManager.SupplierRepository.CommitAsync();

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
