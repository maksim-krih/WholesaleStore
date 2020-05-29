using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using WholesaleStore.Controllers.Base;
using WholesaleStore.Data.Interfaces;

namespace WholesaleStore.Controllers
{
    public class OrderDeliveryController : BaseController
    {
        public OrderDeliveryController(
            IDataExecutor dataExecutor,
            IGridManager gridManager,
            IDataBaseManager dataBaseManager) : base(gridManager, dataExecutor, dataBaseManager)
        {

        }

        public async Task<ActionResult> Index()
        {
            var orderDeliveries = await _dataExecutor.ToListAsync(_dataBaseManager.OrderDeliveryRepository.Query.Include(o => o.Employee).Include(o => o.Order));

            return View(orderDeliveries);
        }

        public ActionResult Create()
        {
            ViewBag.EmployeeId = new SelectList(_dataBaseManager.EmployeeRepository.Query, "Id", "FullName");
            ViewBag.OrderId = new SelectList(_dataBaseManager.OrderRepository.Query, "Id", "Id");
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(OrderDelivery orderDelivery)
        {
            if (ModelState.IsValid)
            {
                _dataBaseManager.OrderDeliveryRepository.Create(orderDelivery);

                await _dataBaseManager.OrderDeliveryRepository.CommitAsync();

                return RedirectToAction("Index");
            }

            ViewBag.EmployeeId = new SelectList(_dataBaseManager.EmployeeRepository.Query, "Id", "FirstName", orderDelivery.EmployeeId);
            ViewBag.OrderId = new SelectList(_dataBaseManager.OrderRepository.Query, "Id", "Id", orderDelivery.OrderId);
            
            return View(orderDelivery);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var orderDelivery = await _dataExecutor.FirstOrDefaultAsync(
                _dataBaseManager.OrderDeliveryRepository.Query
                .Include(o => o.Employee)
                .Include(o => o.Order),
                x => x.Id == id);

            if (orderDelivery == null)
            {
                return HttpNotFound();
            }
            
            ViewBag.EmployeeId = new SelectList(_dataBaseManager.EmployeeRepository.Query, "Id", "FirstName", orderDelivery.EmployeeId);
            ViewBag.OrderId = new SelectList(_dataBaseManager.OrderRepository.Query, "Id", "Id", orderDelivery.OrderId);
            
            return View(orderDelivery);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(OrderDelivery orderDelivery)
        {
            if (ModelState.IsValid)
            {
                var entity = await _dataExecutor.FirstOrDefaultAsync(
                                _dataBaseManager.OrderDeliveryRepository.Query
                                .Include(o => o.Employee)
                                .Include(o => o.Order),
                                x => x.Id == orderDelivery.Id);

                entity.OrderId = orderDelivery.OrderId;
                entity.EmployeeId = orderDelivery.EmployeeId;
                entity.ReceiveDate = orderDelivery.ReceiveDate;
                entity.DeliveryDate = orderDelivery.DeliveryDate;

                await _dataBaseManager.OrderDeliveryRepository.CommitAsync();

                return RedirectToAction("Index");
            }
         
            ViewBag.EmployeeId = new SelectList(_dataBaseManager.EmployeeRepository.Query, "Id", "FirstName", orderDelivery.EmployeeId);
            ViewBag.OrderId = new SelectList(_dataBaseManager.OrderRepository.Query, "Id", "Id", orderDelivery.OrderId);
        
            return View(orderDelivery);
        }

        [HttpPost]
        public async Task<bool> Delete(int id)
        {
            var orderDelivery = await _dataExecutor.FirstOrDefaultAsync(
                _dataBaseManager.OrderDeliveryRepository.Query
                .Include(o => o.Employee)
                .Include(o => o.Order),
                x => x.Id == id);

            _dataBaseManager.OrderDeliveryRepository.Remove(orderDelivery);

            await _dataBaseManager.OrderDeliveryRepository.CommitAsync();

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
