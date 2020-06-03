using PagedList;
using System;
using System.Linq;
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

        public async Task<ActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = sortOrder == "Name" ? "name_desc" : "Name";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var positionsQuery = _dataBaseManager.PositionRepository.Query;

            if (!String.IsNullOrEmpty(searchString))
            {
                positionsQuery = positionsQuery.Where(x => x.Name.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    positionsQuery = positionsQuery.OrderByDescending(x => x.Name);
                    break;
                case "Name":
                    positionsQuery = positionsQuery.OrderBy(s => s.Name);
                    break;
            }

            var positions = await _dataExecutor.ToListAsync(positionsQuery);

            int pageSize = 7;
            int pageNumber = (page ?? 1);

            return View(positions.ToPagedList(pageNumber, pageSize));
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

        [HttpPost]
        public async Task<bool> Delete(int id)
        {
            var position = await _dataExecutor.FirstOrDefaultAsync(_dataBaseManager.PositionRepository.Query, x => x.Id == id);

            _dataBaseManager.PositionRepository.Remove(position);

            await _dataBaseManager.PositionRepository.CommitAsync();

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
