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
    public class ProductTypeController : BaseController
    {
        public ProductTypeController(
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

            var productTypesQuery = _dataBaseManager.ProductTypeRepository.Query;

            if (!String.IsNullOrEmpty(searchString))
            {
                productTypesQuery = productTypesQuery.Where(x => x.Name.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    productTypesQuery = productTypesQuery.OrderByDescending(x => x.Name);
                    break;
                case "Name":
                    productTypesQuery = productTypesQuery.OrderBy(s => s.Name);
                    break;
            }

            var productTypes = await _dataExecutor.ToListAsync(productTypesQuery);

            int pageSize = 7;
            int pageNumber = (page ?? 1);

            return View(productTypes.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(ProductType productType)
        {
            if (ModelState.IsValid)
            {
                _dataBaseManager.ProductTypeRepository.Create(productType);
                await _dataBaseManager.ProductTypeRepository.CommitAsync();

                return RedirectToAction("Index");
            }

            return View(productType);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var productType = await _dataExecutor.FirstOrDefaultAsync(_dataBaseManager.ProductTypeRepository.Query, x => x.Id == id);

            if (productType == null)
            {
                return HttpNotFound();
            }

            return View(productType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(ProductType productType)
        {
            if (ModelState.IsValid)
            {
                var entity = await _dataExecutor.FirstOrDefaultAsync(_dataBaseManager.ProductTypeRepository.Query, x => x.Id == productType.Id);

                entity.Name = productType.Name;

                await _dataBaseManager.ProductTypeRepository.CommitAsync();

                return RedirectToAction("Index");
            }

            return View(productType);
        }

        [HttpPost]
        public async Task<bool> Delete(int id)
        {
            var productType = await _dataExecutor.FirstOrDefaultAsync(_dataBaseManager.ProductTypeRepository.Query, x => x.Id == id);

            _dataBaseManager.ProductTypeRepository.Remove(productType);

            await _dataBaseManager.ProductTypeRepository.CommitAsync();

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
