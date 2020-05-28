using System.Data.Entity;
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

        public async Task<ActionResult> Index()
        {
            var supplyShipments = await _dataExecutor.ToListAsync(
                _dataBaseManager.SupplyShipmentRepository.Query
                .Include(s => s.Employee)
                .Include(s => s.ProductsInStorage)
                .Include(s => s.SupplyContent));

            return View(supplyShipments);
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

            ViewBag.EmployeeId = new SelectList(_dataBaseManager.EmployeeRepository.Query, "Id", "FirstName", supplyShipment.EmployeeId);
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
            
            ViewBag.EmployeeId = new SelectList(_dataBaseManager.EmployeeRepository.Query, "Id", "FirstName", supplyShipment.EmployeeId);
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
        
            ViewBag.EmployeeId = new SelectList(_dataBaseManager.EmployeeRepository.Query, "Id", "FirstName", supplyShipment.EmployeeId);
            ViewBag.ProductInStorageId = new SelectList(_dataBaseManager.ProductsInStorageRepository.Query, "Id", "Id", supplyShipment.ProductInStorageId);
            ViewBag.SupplyContentId = new SelectList(_dataBaseManager.SupplyContentRepository.Query, "Id", "Id", supplyShipment.SupplyContentId);
         
            return View(supplyShipment);
        }

        public async Task<ActionResult> Delete(int? id)
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
         
            return View(supplyShipment);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var supplyShipment = await _dataExecutor.FirstOrDefaultAsync(
                _dataBaseManager.SupplyShipmentRepository.Query
                .Include(s => s.Employee)
                .Include(s => s.ProductsInStorage)
                .Include(s => s.SupplyContent),
                x => x.Id == id);

            _dataBaseManager.SupplyShipmentRepository.Remove(supplyShipment);

            await _dataBaseManager.SupplyShipmentRepository.CommitAsync();

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
