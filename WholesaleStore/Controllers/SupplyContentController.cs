using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using WholesaleStore.Controllers.Base;
using WholesaleStore.Data.Interfaces;

namespace WholesaleStore.Controllers
{
    public class SupplyContentController : BaseController
    {
        public SupplyContentController(
            IDataExecutor dataExecutor,
            IGridManager gridManager,
            IDataBaseManager dataBaseManager) : base(gridManager, dataExecutor, dataBaseManager)
        {

        }

        public async Task<ActionResult> Index()
        {
            var supplyContents = await _dataExecutor.ToListAsync(_dataBaseManager.SupplyContentRepository.Query.Include(s => s.Product).Include(s => s.Supply));

            return View(supplyContents);
        }

        public ActionResult Create()
        {
            ViewBag.ProductId = new SelectList(_dataBaseManager.ProductRepository.Query, "Id", "FullName");
            ViewBag.SupplyId = new SelectList(_dataBaseManager.SupplyRepository.Query, "Id", "Id");
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(SupplyContent supplyContent)
        {
            if (ModelState.IsValid)
            {
                _dataBaseManager.SupplyContentRepository.Create(supplyContent);

                await _dataBaseManager.SupplyContentRepository.CommitAsync();

                return RedirectToAction("Index");
            }

            ViewBag.ProductId = new SelectList(_dataBaseManager.ProductRepository.Query, "Id", "Name", supplyContent.ProductId);
            ViewBag.SupplyId = new SelectList(_dataBaseManager.SupplyRepository.Query, "Id", "Id", supplyContent.SupplyId);
            
            return View(supplyContent);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var supplyContent = await _dataExecutor.FirstOrDefaultAsync(
                _dataBaseManager.SupplyContentRepository.Query
                .Include(s => s.Product)
                .Include(s => s.Supply),
                x => x.Id == id);

            if (supplyContent == null)
            {
                return HttpNotFound();
            }
            
            ViewBag.ProductId = new SelectList(_dataBaseManager.ProductRepository.Query, "Id", "Name", supplyContent.ProductId);
            ViewBag.SupplyId = new SelectList(_dataBaseManager.SupplyRepository.Query, "Id", "Id", supplyContent.SupplyId);
            
            return View(supplyContent);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(SupplyContent supplyContent)
        {
            if (ModelState.IsValid)
            {
                var entity = await _dataExecutor.FirstOrDefaultAsync(
                _dataBaseManager.SupplyContentRepository.Query
                .Include(s => s.Product)
                .Include(s => s.Supply),
                x => x.Id == supplyContent.Id);

                entity.SupplyId = supplyContent.SupplyId;
                entity.ProductId = supplyContent.ProductId;
                entity.Count = supplyContent.Count;
                entity.SupplyPrice = supplyContent.SupplyPrice;

                await _dataBaseManager.SupplyContentRepository.CommitAsync();

                return RedirectToAction("Index");
            }
            
            ViewBag.ProductId = new SelectList(_dataBaseManager.ProductRepository.Query, "Id", "Name", supplyContent.ProductId);
            ViewBag.SupplyId = new SelectList(_dataBaseManager.SupplyRepository.Query, "Id", "Id", supplyContent.SupplyId);
            
            return View(supplyContent);
        }

        [HttpPost]
        public async Task<bool> Delete(int id)
        {
            var supplyContent = await _dataExecutor.FirstOrDefaultAsync(
                _dataBaseManager.SupplyContentRepository.Query
                .Include(s => s.Product)
                .Include(s => s.Supply),
                x => x.Id == id);

            _dataBaseManager.SupplyContentRepository.Remove(supplyContent);

            await _dataBaseManager.SupplyContentRepository.CommitAsync();

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
