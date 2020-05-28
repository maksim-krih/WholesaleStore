using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using WholesaleStore.Controllers.Base;
using WholesaleStore.Data.Interfaces;

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
            ViewBag.EmployeeId = new SelectList(_dataBaseManager.EmployeeRepository.Query, "Id", "FullName");
            ViewBag.SupplierId = new SelectList(_dataBaseManager.SupplierRepository.Query, "Id", "CompanyName");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Supply supply)
        {
            if (ModelState.IsValid)
            {
                _dataBaseManager.SupplyRepository.Create(supply);

                await _dataBaseManager.SupplyRepository.CommitAsync();

                return RedirectToAction("Index");
            }

            ViewBag.EmployeeId = new SelectList(_dataBaseManager.EmployeeRepository.Query, "Id", "FirstName", supply.EmployeeId);
            ViewBag.SupplierId = new SelectList(_dataBaseManager.SupplierRepository.Query, "Id", "CompanyName", supply.SupplierId);

            return View(supply);
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

            ViewBag.EmployeeId = new SelectList(_dataBaseManager.EmployeeRepository.Query, "Id", "FirstName", supply.EmployeeId);
            ViewBag.SupplierId = new SelectList(_dataBaseManager.SupplierRepository.Query, "Id", "CompanyName", supply.SupplierId);

            return View(supply);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Supply supply)
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
                entity.Status = supply.Status;

                await _dataBaseManager.SupplyRepository.CommitAsync();

                return RedirectToAction("Index");
            }

            ViewBag.EmployeeId = new SelectList(_dataBaseManager.EmployeeRepository.Query, "Id", "FirstName", supply.EmployeeId);
            ViewBag.SupplierId = new SelectList(_dataBaseManager.SupplierRepository.Query, "Id", "CompanyName", supply.SupplierId);

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
