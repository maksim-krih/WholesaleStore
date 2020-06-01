using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using WholesaleStore.Common.Enums;
using WholesaleStore.Controllers.Base;
using WholesaleStore.Data.Interfaces;
using WholesaleStore.Models.Dtos;

namespace WholesaleStore.Controllers
{
    public class SupplyManagerController : BaseController
    {
        public SupplyManagerController(
            IDataExecutor dataExecutor,
            IGridManager gridManager,
            IDataBaseManager dataBaseManager) : base(gridManager, dataExecutor, dataBaseManager)
        {

        }

        public async Task<ActionResult> Supply(string sortOrder, string currentFilter, string searchString, int? page)
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
                .Include(s => s.Supplier)
                .Include($"{nameof(WholesaleStore.Supply.SupplyContents)}.{nameof(SupplyContent.Product)}");

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

        public async Task<ActionResult> Shipment(string sortOrder, string currentFilter, string searchString, int? page)
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
                .Include(s => s.Supplier)
                .Include($"{nameof(WholesaleStore.Supply.SupplyContents)}.{nameof(SupplyContent.Product)}")
                .Include($"{nameof(WholesaleStore.Supply.SupplyContents)}.{nameof(SupplyContent.SupplyShipments)}");

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

        public ActionResult CreateSupply()
        {
            var supply = new SupplyDto();

            supply.SupplyContents = new List<SupplyContentDto>
            {
                new SupplyContentDto { Count = 0, ProductId = 0 }
            };

            supply.ProductList = new SelectList(_dataBaseManager.ProductRepository.Query, "Id", "FullName");

            ViewBag.EmployeeId = new SelectList(_dataBaseManager.EmployeeRepository.Query, "Id", "FullName");
            ViewBag.SupplierId = new SelectList(_dataBaseManager.SupplierRepository.Query, "Id", "CompanyName");

            return View(supply);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSupplyContent(SupplyDto supplyDto)
        {
            supplyDto.SupplyContents.Add(new SupplyContentDto());
            supplyDto.ProductList = new SelectList(_dataBaseManager.ProductRepository.Query, "Id", "FullName");

            return PartialView("SupplyContent", supplyDto);
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
                    SupplierId = supplyDto.SupplierId,
                    Date = DateTime.Now,
                    Status = (int)SupplyStatus.WaitingForShipment,
                    SupplyContents = supplyDto.SupplyContents.Select(x => new SupplyContent
                    {
                        ProductId = x.ProductId,
                        Count = x.Count
                    }).ToList()
                };

                _dataBaseManager.SupplyRepository.Create(supply);

                await _dataBaseManager.SupplyRepository.CommitAsync();

                return RedirectToAction("Supply");
            }

            supplyDto.ProductList = new SelectList(_dataBaseManager.ProductRepository.Query, "Id", "FullName");

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
                SupplierId = supply.SupplierId
            };

            ViewBag.EmployeeId = new SelectList(_dataBaseManager.EmployeeRepository.Query, "Id", "FullName", supplyDto.EmployeeId);
            ViewBag.SupplierId = new SelectList(_dataBaseManager.SupplierRepository.Query, "Id", "CompanyName", supplyDto.SupplierId);

            return View(supplyDto);
        }

        public async Task<ActionResult> EditShipment(int? id)
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

            var supplyShipmentDto = new List<SupplyShipmentDto>();

            for (var i = 0; i < supply.SupplyContents.Count; i++)
            {
                supplyShipmentDto.Add(new SupplyShipmentDto
                {
                    ProductsInStorage = new ProductsInStorage()
                });
            }

            var supplyDto = new SupplyDto
            {
                Id = supply.Id,
                SupplyContents = supply.SupplyContents.Select(x => new SupplyContentDto
                {
                    Count = x.Count,
                    Product = x.Product,
                    SupplyShipments = supplyShipmentDto
                }).ToList()
            };

            supplyDto.ProductList = new SelectList(_dataBaseManager.ProductRepository.Query, "Id", "FullName");
            supplyDto.EmployeeList = new SelectList(_dataBaseManager.EmployeeRepository.Query, "Id", "FullName");
            supplyDto.StorageList = new SelectList(_dataBaseManager.StorageRepository.Query, "Id", "Number");

            return View(supplyDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditShipment(SupplyDto supply)
        {
            if (ModelState.IsValid)
            {
                var entity = await _dataExecutor.FirstOrDefaultAsync(
                    _dataBaseManager.SupplyRepository.Query
                    .Include(s => s.Employee)
                    .Include(s => s.Supplier),
                    x => x.Id == supply.Id);

                var supplyContents = entity.SupplyContents.ToList();

                for (var i = 0; i < entity.SupplyContents.Count; i++)
                {
                    var supplyShipmentDto = supply.SupplyContents[i].SupplyShipments[0];

                    if (supplyContents[i].SupplyShipments.Count == 0)
                    {
                        supplyContents[i].SupplyShipments.Add(new SupplyShipment());
                    }

                    var supplyShipment = supplyContents[i].SupplyShipments.FirstOrDefault();

                    supplyShipment.EmployeeId = supplyShipmentDto.EmployeeId;
                    supplyShipment.ProductsInStorage = new ProductsInStorage
                    {
                        StorageId = supplyShipmentDto.ProductsInStorage.StorageId,
                        ProductId = supplyContents[0].ProductId
                    };
                    supplyShipment.Count = supplyContents[i].Count;
                    supplyShipment.Date = DateTime.Now;
                }

                entity.Status = (int)SupplyStatus.Delivering;

                await _dataBaseManager.SupplyRepository.CommitAsync();

                return RedirectToAction("Shipment");
            }

            supply.ProductList = new SelectList(_dataBaseManager.ProductRepository.Query, "Id", "FullName");
            supply.EmployeeList = new SelectList(_dataBaseManager.EmployeeRepository.Query, "Id", "FullName");
            supply.StorageList = new SelectList(_dataBaseManager.StorageRepository.Query, "Id", "Number");

            return View(supply);
        }

        [HttpPost]
        public async Task<bool> CloseShipment(int? id)
        {
            var order = await _dataExecutor.FirstOrDefaultAsync(
                _dataBaseManager.SupplyRepository.Query
                .Include(s => s.Employee)
                .Include(s => s.Supplier),
                x => x.Id == id);

            order.Status = (int)SupplyStatus.Delivered;

            await _dataBaseManager.SupplyRepository.CommitAsync();

            return true;
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
