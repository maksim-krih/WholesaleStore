using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using WholesaleStore.Controllers.Base;
using WholesaleStore.Data.Interfaces;

namespace WholesaleStore.Controllers
{
    public class PositionController : BaseController
    {
        public PositionController(
            IDataExecutor dataExecutor,
            IGridManager gridManager,
            IDataBaseManager dataBaseManager) : base(gridManager, dataExecutor, dataBaseManager)
        {

        }

        public async Task<ActionResult> Index()
        {
            var positions = await _dataExecutor.ToListAsync(_dataBaseManager.PositionRepository.Query);

            return View(positions);
        }

        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var position = await _dataExecutor.FirstOrDefaultAsync(_dataBaseManager.PositionRepository.Query, x => x.Id == id);

            if (position == null)
            {
                return HttpNotFound();
            }

            return View(position);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Position position)
        {
            if (ModelState.IsValid)
            {
                _dataBaseManager.PositionRepository.Create(position);
                await _dataBaseManager.PositionRepository.CommitAsync();

                return RedirectToAction("Index");
            }

            return View(position);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var position = await _dataExecutor.FirstOrDefaultAsync(_dataBaseManager.PositionRepository.Query, x => x.Id == id);

            if (position == null)
            {
                return HttpNotFound();
            }

            return View(position);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Position position)
        {
            if (ModelState.IsValid)
            {
                var entity = await _dataExecutor.FirstOrDefaultAsync(_dataBaseManager.PositionRepository.Query, x => x.Id == position.Id);

                entity.Name = position.Name;

                await _dataBaseManager.PositionRepository.CommitAsync();

                return RedirectToAction("Index");
            }

            return View(position);
        }

        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var position = await _dataExecutor.FirstOrDefaultAsync(_dataBaseManager.PositionRepository.Query, x => x.Id == id);

            if (position == null)
            {
                return HttpNotFound();
            }

            return View(position);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var position = await _dataExecutor.FirstOrDefaultAsync(_dataBaseManager.PositionRepository.Query, x => x.Id == id);

            _dataBaseManager.PositionRepository.Remove(position);

            await _dataBaseManager.PositionRepository.CommitAsync();

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
