﻿using System.Net;
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

        public async Task<ActionResult> Index()
        {
            var productTypes = await _dataExecutor.ToListAsync(_dataBaseManager.ProductTypeRepository.Query);

            return View(productTypes);
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
