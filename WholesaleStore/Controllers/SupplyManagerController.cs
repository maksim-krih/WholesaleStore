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

        public async Task<ActionResult> Supply()
        {
            var supplies = await _dataExecutor.ToListAsync(
                _dataBaseManager.SupplyRepository.Query
                .Include(s => s.Employee)
                .Include(s => s.Supplier)
                .Include($"{nameof(WholesaleStore.Supply.SupplyContents)}.{nameof(SupplyContent.Product)}")
                );

            return View(supplies);
        }

        public async Task<ActionResult> Shipment()
        {
            var supplies = await _dataExecutor.ToListAsync(
                _dataBaseManager.SupplyRepository.Query
                .Include(s => s.Employee)
                .Include(s => s.Supplier)
                .Include($"{nameof(WholesaleStore.Supply.SupplyContents)}.{nameof(SupplyContent.Product)}")
                .Include($"{nameof(WholesaleStore.Supply.SupplyContents)}.{nameof(SupplyContent.SupplyShipments)}")
                );

            return View(supplies);
        }

        public async Task<ActionResult> Details(int? id)
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

            return View(supply);
        }

        public ActionResult CreateSupply()
        {
            var supply = new SupplyDto();

            supply.SupplyContents = new List<SupplyContentDto>
            {
                new SupplyContentDto { Count = 0, ProductId = 0 }
            };

            supply.ProductList = new SelectList(_dataBaseManager.ProductRepository.Query, "Id", "Name");

            ViewBag.EmployeeId = new SelectList(_dataBaseManager.EmployeeRepository.Query, "Id", "FirstName");
            ViewBag.SupplierId = new SelectList(_dataBaseManager.SupplierRepository.Query, "Id", "CompanyName");

            return View(supply);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddSupplyContent(SupplyDto supplyDto)
        {
            supplyDto.SupplyContents.Add(new SupplyContentDto());
            supplyDto.ProductList = new SelectList(_dataBaseManager.ProductRepository.Query, "Id", "Name");

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

            supplyDto.ProductList = new SelectList(_dataBaseManager.ProductRepository.Query, "Id", "Name");

            ViewBag.EmployeeId = new SelectList(_dataBaseManager.EmployeeRepository.Query, "Id", "FirstName", supplyDto.EmployeeId);
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

            supplyDto.ProductList = new SelectList(_dataBaseManager.ProductRepository.Query, "Id", "Name");
            supplyDto.EmployeeList = new SelectList(_dataBaseManager.EmployeeRepository.Query, "Id", "FirstName");
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

                await _dataBaseManager.SupplyRepository.CommitAsync();

                return RedirectToAction("Shipment");
            }

            supply.ProductList = new SelectList(_dataBaseManager.ProductRepository.Query, "Id", "Name");
            supply.EmployeeList = new SelectList(_dataBaseManager.EmployeeRepository.Query, "Id", "FirstName");
            supply.StorageList = new SelectList(_dataBaseManager.StorageRepository.Query, "Id", "Number");

            return View(supply);
        }

        public async Task<ActionResult> Delete(int? id)
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

            return View(supply);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var supply = await _dataExecutor.FirstOrDefaultAsync(
                 _dataBaseManager.SupplyRepository.Query
                 .Include(s => s.Employee)
                 .Include(s => s.Supplier),
                 x => x.Id == id);

            _dataBaseManager.SupplyRepository.Remove(supply);

            await _dataBaseManager.SupplyRepository.CommitAsync();

            return RedirectToAction("Supply");
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
