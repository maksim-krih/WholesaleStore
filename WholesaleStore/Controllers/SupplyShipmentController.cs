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
    public class SupplyShipmentController : BaseController
    {
        public SupplyShipmentController(
            IDataExecutor dataExecutor,
            IGridManager gridManager,
            IDataBaseManager dataBaseManager) : base(gridManager, dataExecutor, dataBaseManager)
        {

        }

        public async Task<ActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.EmployeeSortParm = sortOrder == "Employee" ? "employee_desc" : "Employee";
            ViewBag.CountSortParm = sortOrder == "Count" ? "count_desc" : "Count";
            ViewBag.ProductSortParm = sortOrder == "Product" ? "product_desc" : "Product";
            ViewBag.SupplySortParm = sortOrder == "Supply" ? "supply_desc" : "Supply";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var supplyShipmentsQuery = _dataBaseManager.SupplyShipmentRepository.Query
                .Include(s => s.Employee)
                .Include(s => s.ProductsInStorage)
                .Include(s => s.SupplyContent);


            if (!String.IsNullOrEmpty(searchString))
            {
                supplyShipmentsQuery = supplyShipmentsQuery.Where(x =>
                x.Employee.FirstName.Contains(searchString) ||
                x.Employee.LastName.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "employee_desc":
                    supplyShipmentsQuery = supplyShipmentsQuery.OrderByDescending(x => x.Employee.FirstName).ThenByDescending(x => x.Employee.LastName);
                    break;
                case "Employee":
                    supplyShipmentsQuery = supplyShipmentsQuery.OrderBy(s => s.Employee.FirstName).ThenBy(x => x.Employee.LastName);
                    break;
                case "count_desc":
                    supplyShipmentsQuery = supplyShipmentsQuery.OrderByDescending(x => x.Count);
                    break;
                case "Count":
                    supplyShipmentsQuery = supplyShipmentsQuery.OrderBy(s => s.Count);
                    break;
                case "product_desc":
                    supplyShipmentsQuery = supplyShipmentsQuery.OrderByDescending(x => x.ProductInStorageId);
                    break;
                case "Product":
                    supplyShipmentsQuery = supplyShipmentsQuery.OrderBy(s => s.ProductInStorageId);
                    break;
                case "supply_desc":
                    supplyShipmentsQuery = supplyShipmentsQuery.OrderByDescending(x => x.SupplyContentId);
                    break;
                case "Supply":
                    supplyShipmentsQuery = supplyShipmentsQuery.OrderBy(s => s.SupplyContentId);
                    break;
            }
                    
            var supplyShipments = await _dataExecutor.ToListAsync(supplyShipmentsQuery);

            int pageSize = 7;
            int pageNumber = (page ?? 1);

            return View(supplyShipments.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Create()
        {
            ViewBag.EmployeeId = new SelectList(_dataBaseManager.EmployeeRepository.Query, "Id", "FullName");
            ViewBag.ProductInStorageId = new SelectList(_dataBaseManager.ProductsInStorageRepository.Query, "Id", "Id");
            ViewBag.SupplyContentId = new SelectList(_dataBaseManager.SupplyContentRepository.Query, "Id", "Id");
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(SupplyShipment supplyShipment)
        {
            if (ModelState.IsValid)
            {
                _dataBaseManager.SupplyShipmentRepository.Create(supplyShipment);

                await _dataBaseManager.SupplyShipmentRepository.CommitAsync();

                return RedirectToAction("Index");
            }

            ViewBag.EmployeeId = new SelectList(_dataBaseManager.EmployeeRepository.Query, "Id", "FullName", supplyShipment.EmployeeId);
            ViewBag.ProductInStorageId = new SelectList(_dataBaseManager.ProductsInStorageRepository.Query, "Id", "Id", supplyShipment.ProductInStorageId);
            ViewBag.SupplyContentId = new SelectList(_dataBaseManager.SupplyContentRepository.Query, "Id", "Id", supplyShipment.SupplyContentId);
            
            return View(supplyShipment);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var supplyShipment = await _dataExecutor.FirstOrDefaultAsync(
                _dataBaseManager.SupplyShipmentRepository.Query
                .Include(s => s.Employee)
                .Include(s => s.ProductsInStorage)
                .Include(s => s.SupplyContent),
                x => x.Id == id);

            if (supplyShipment == null)
            {
                return HttpNotFound();
            }
            
            ViewBag.EmployeeId = new SelectList(_dataBaseManager.EmployeeRepository.Query, "Id", "FullName", supplyShipment.EmployeeId);
            ViewBag.ProductInStorageId = new SelectList(_dataBaseManager.ProductsInStorageRepository.Query, "Id", "Id", supplyShipment.ProductInStorageId);
            ViewBag.SupplyContentId = new SelectList(_dataBaseManager.SupplyContentRepository.Query, "Id", "Id", supplyShipment.SupplyContentId);
            
            return View(supplyShipment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(SupplyShipment supplyShipment)
        {
            if (ModelState.IsValid)
            {
                var entity = await _dataExecutor.FirstOrDefaultAsync(
                _dataBaseManager.SupplyShipmentRepository.Query
                .Include(s => s.Employee)
                .Include(s => s.ProductsInStorage)
                .Include(s => s.SupplyContent),
                x => x.Id == supplyShipment.Id);

                entity.SupplyContentId = supplyShipment.SupplyContentId;
                entity.ProductInStorageId = supplyShipment.ProductInStorageId;
                entity.Date = supplyShipment.Date;
                entity.Count = supplyShipment.Count;
                entity.EmployeeId = supplyShipment.EmployeeId;

                await _dataBaseManager.BrandRepository.CommitAsync();

                return RedirectToAction("Index");
            }
        
            ViewBag.EmployeeId = new SelectList(_dataBaseManager.EmployeeRepository.Query, "Id", "FullName", supplyShipment.EmployeeId);
            ViewBag.ProductInStorageId = new SelectList(_dataBaseManager.ProductsInStorageRepository.Query, "Id", "Id", supplyShipment.ProductInStorageId);
            ViewBag.SupplyContentId = new SelectList(_dataBaseManager.SupplyContentRepository.Query, "Id", "Id", supplyShipment.SupplyContentId);
         
            return View(supplyShipment);
        }

        [HttpPost]
        public async Task<bool> Delete(int id)
        {
            var supplyShipment = await _dataExecutor.FirstOrDefaultAsync(
                _dataBaseManager.SupplyShipmentRepository.Query
                .Include(s => s.Employee)
                .Include(s => s.ProductsInStorage)
                .Include(s => s.SupplyContent),
                x => x.Id == id);

            _dataBaseManager.SupplyShipmentRepository.Remove(supplyShipment);

            await _dataBaseManager.SupplyShipmentRepository.CommitAsync();

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
