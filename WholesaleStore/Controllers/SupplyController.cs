using System.Data.Entity;
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

        public async Task<ActionResult> Index()
        {
            var supplies = await _dataExecutor.ToListAsync(
                _dataBaseManager.SupplyRepository.Query
                .Include(s => s.Employee)
                .Include(s => s.Supplier));

            return View(supplies);
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
