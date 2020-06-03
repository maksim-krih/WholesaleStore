using PagedList;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using WholesaleStore.Common.Enums;
using WholesaleStore.Controllers.Base;
using WholesaleStore.Data.Interfaces;
using WholesaleStore.Models.Dtos;

namespace WholesaleStore.Controllers
{
    public class SupplyController : BaseController
    {
        public SupplyController(
            IDataExecutor dataExecutor,
            IGridManager gridManager,
            IDataBaseManager dataBaseManager) : base(gridManager, dataExecutor, dataBaseManager)
        {

        }

        public async Task<ActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            ViewBag.StatusSortParm = sortOrder == "Status" ? "status_desc" : "Status";
            ViewBag.EmployeeSortParm = sortOrder == "Employee" ? "employee_desc" : "Employee";
            ViewBag.SupplierSortParm = sortOrder == "Supplier" ? "supplier_desc" : "Supplier";
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

            var suppliesQuery = _dataBaseManager.SupplyRepository.Query
                .Include(s => s.Employee)
                .Include(s => s.Supplier);

            if (!String.IsNullOrEmpty(searchString))
            {
                suppliesQuery = suppliesQuery.Where(x => x.Supplier.CompanyName.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "Date":
                    suppliesQuery = suppliesQuery.OrderBy(s => s.Date);
                    break;
                case "date_desc":
                    suppliesQuery = suppliesQuery.OrderByDescending(x => x.Date);
                    break;
                case "Status":
                    suppliesQuery = suppliesQuery.OrderBy(x => x.Status);
                    break;
                case "status_desc":
                    suppliesQuery = suppliesQuery.OrderByDescending(x => x.Status);
                    break;
                case "Supplier":
                    suppliesQuery = suppliesQuery.OrderBy(x => x.Supplier.CompanyName);
                    break;
                case "supplier_desc":
                    suppliesQuery = suppliesQuery.OrderByDescending(x => x.Supplier.CompanyName);
                    break;
                case "Employee":
                    suppliesQuery = suppliesQuery.OrderBy(x => x.Employee.FirstName).ThenBy(x => x.Employee.LastName);
                    break;
                case "employee_desc":
                    suppliesQuery = suppliesQuery.OrderByDescending(x => x.Employee.FirstName).ThenByDescending(x => x.Employee.LastName);
                    break;
                case "Number":
                    suppliesQuery = suppliesQuery.OrderBy(x => x.Number);
                    break;
                case "number_desc":
                    suppliesQuery = suppliesQuery.OrderByDescending(x => x.Number);
                    break;
            }

            var supplies = await _dataExecutor.ToListAsync(suppliesQuery);

            int pageSize = 7;
            int pageNumber = (page ?? 1);

            return View(supplies.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Create()
        {
            var supply = new SupplyDto();

            ViewBag.EmployeeId = new SelectList(_dataBaseManager.EmployeeRepository.Query, "Id", "FullName");
            ViewBag.SupplierId = new SelectList(_dataBaseManager.SupplierRepository.Query, "Id", "CompanyName");

            return View(supply);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(SupplyDto supplyDto)
        {
            if (ModelState.IsValid)
            {
                var supply = new Supply
                {
                    Number = supplyDto.Number,
                    EmployeeId = supplyDto.EmployeeId,
                    Date = supplyDto.Date,
                    Status = (int)supplyDto.Status,
                    SupplierId = supplyDto.SupplierId
                };

                _dataBaseManager.SupplyRepository.Create(supply);

                await _dataBaseManager.SupplyRepository.CommitAsync();

                return RedirectToAction("Index");
            }

            ViewBag.EmployeeId = new SelectList(_dataBaseManager.EmployeeRepository.Query, "Id", "FullName", supplyDto.EmployeeId);
            ViewBag.SupplierId = new SelectList(_dataBaseManager.SupplierRepository.Query, "Id", "CompanyName", supplyDto.SupplierId);

            return View(supplyDto);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var supply = await _dataExecutor.FirstOrDefaultAsync(
                _dataBaseManager.SupplyRepository.Query
                .Include(s => s.Employee)
                .Include(s => s.Supplier),
                x => x.Id == id);

            if (supply == null)
            {
                return HttpNotFound();
            }

            var supplyDto = new SupplyDto
            {
                Id = supply.Id,
                Number = supply.Number,
                EmployeeId = supply.EmployeeId,
                SupplierId = supply.SupplierId,
                Date = supply.Date,
                Status = (SupplyStatus)supply.Status
            };

            ViewBag.EmployeeId = new SelectList(_dataBaseManager.EmployeeRepository.Query, "Id", "FullName", supplyDto.EmployeeId);
            ViewBag.SupplierId = new SelectList(_dataBaseManager.SupplierRepository.Query, "Id", "CompanyName", supplyDto.SupplierId);

            return View(supplyDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(SupplyDto supply)
        {
            if (ModelState.IsValid)
            {
                var entity = await _dataExecutor.FirstOrDefaultAsync(
                    _dataBaseManager.SupplyRepository.Query
                    .Include(s => s.Employee)
                    .Include(s => s.Supplier),
                    x => x.Id == supply.Id);

                entity.Number = supply.Number;
                entity.Date = supply.Date;
                entity.EmployeeId = supply.EmployeeId;
                entity.SupplierId = supply.SupplierId;
                entity.Status = (int)supply.Status;

                await _dataBaseManager.SupplyRepository.CommitAsync();

                return RedirectToAction("Index");
            }

            ViewBag.EmployeeId = new SelectList(_dataBaseManager.EmployeeRepository.Query, "Id", "FullName", supply.EmployeeId);
            ViewBag.SupplierId = new SelectList(_dataBaseManager.SupplierRepository.Query, "Id", "CompanyName", supply.SupplierId);

            return View(supply);
        }

        [HttpPost]
        public async Task<bool> Delete(int id)
        {
            var supply = await _dataExecutor.FirstOrDefaultAsync(
                 _dataBaseManager.SupplyRepository.Query
                 .Include(s => s.Employee)
                 .Include(s => s.Supplier),
                 x => x.Id == id);

            _dataBaseManager.SupplyRepository.Remove(supply);

            await _dataBaseManager.SupplyRepository.CommitAsync();

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
