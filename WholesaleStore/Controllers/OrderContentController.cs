using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using WholesaleStore.Controllers.Base;
using WholesaleStore.Data.Interfaces;

namespace WholesaleStore.Controllers
{
    public class OrderContentController : BaseController
    {
        public OrderContentController(
            IDataExecutor dataExecutor,
            IGridManager gridManager,
            IDataBaseManager dataBaseManager) : base(gridManager, dataExecutor, dataBaseManager)
        {

        }

        public async Task<ActionResult> Index()
        {
            var orderContents = await _dataExecutor.ToListAsync(_dataBaseManager.OrderContentRepository.Query.Include(o => o.Order).Include(o => o.Product));

            return View(orderContents);
        }

        public ActionResult Create()
        {
            ViewBag.OrderId = new SelectList(_dataBaseManager.OrderRepository.Query, "Id", "Number");
            ViewBag.ProductId = new SelectList(_dataBaseManager.ProductRepository.Query, "Id", "FullName");
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(OrderContent orderContent)
        {
            if (ModelState.IsValid)
            {
                _dataBaseManager.OrderContentRepository.Create(orderContent);

                await _dataBaseManager.OrderContentRepository.CommitAsync();

                return RedirectToAction("Index");
            }

            ViewBag.OrderId = new SelectList(_dataBaseManager.OrderRepository.Query, "Id", "Number", orderContent.OrderId);
            ViewBag.ProductId = new SelectList(_dataBaseManager.ProductRepository.Query, "Id", "Name", orderContent.ProductId);
            
            return View(orderContent);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var orderContent = await _dataExecutor.FirstOrDefaultAsync(_dataBaseManager.OrderContentRepository.Query.Include(o => o.Order).Include(o => o.Product), x => x.Id == id);

            if (orderContent == null)
            {
                return HttpNotFound();
            }
            
            ViewBag.OrderId = new SelectList(_dataBaseManager.OrderRepository.Query, "Id", "Number", orderContent.OrderId);
            ViewBag.ProductId = new SelectList(_dataBaseManager.ProductRepository.Query, "Id", "Name", orderContent.ProductId);
            
            return View(orderContent);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(OrderContent orderContent)
        {
            if (ModelState.IsValid)
            {
                var entity = await _dataExecutor.FirstOrDefaultAsync(_dataBaseManager.OrderContentRepository.Query, x => x.Id == orderContent.Id);

                entity.OrderId = orderContent.OrderId;
                entity.ProductId = orderContent.ProductId;
                entity.Count = orderContent.Count;

                await _dataBaseManager.OrderContentRepository.CommitAsync();

                return RedirectToAction("Index");
            }
            
            ViewBag.OrderId = new SelectList(_dataBaseManager.OrderRepository.Query, "Id", "Number", orderContent.OrderId);
            ViewBag.ProductId = new SelectList(_dataBaseManager.ProductRepository.Query, "Id", "Name", orderContent.ProductId);
            
            return View(orderContent);
        }

        [HttpPost]
        public async Task<bool> Delete(int id)
        {
            var orderContent = await _dataExecutor.FirstOrDefaultAsync(_dataBaseManager.OrderContentRepository.Query.Include(o => o.Order).Include(o => o.Product), x => x.Id == id);

            _dataBaseManager.OrderContentRepository.Remove(orderContent);

            await _dataBaseManager.OrderContentRepository.CommitAsync();

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
