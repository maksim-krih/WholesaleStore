using PagedList;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using WholesaleStore.Controllers.Base;
using WholesaleStore.Data.Interfaces;

namespace WholesaleStore.Controllers
{
    public class ProductController : BaseController
    {
        public ProductController(
            IDataExecutor dataExecutor,
            IGridManager gridManager,
            IDataBaseManager dataBaseManager) : base(gridManager, dataExecutor, dataBaseManager)
        {

        }

        public async Task<ActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.CodeSortParm = sortOrder == "Code" ? "code_desc" : "Code";
            ViewBag.NameSortParm = sortOrder == "Name" ? "name_desc" : "Name";
            ViewBag.AmountSortParm = sortOrder == "Amount" ? "amount_desc" : "Amount";
            ViewBag.PriceSortParm = sortOrder == "Price" ? "price_desc" : "Price";
            ViewBag.BrandSortParm = sortOrder == "Brand" ? "brand_desc" : "Brand";
            ViewBag.TypeSortParm = sortOrder == "Type" ? "type_desc" : "Type";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var productsQuery = _dataBaseManager.ProductRepository.Query
                .Include(p => p.Brand)
                .Include(p => p.ProductType);

            if (!String.IsNullOrEmpty(searchString))
            {
                productsQuery = productsQuery.Where(x => x.Name.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "code_desc":
                    productsQuery = productsQuery.OrderByDescending(x => x.Code);
                    break;
                case "Code":
                    productsQuery = productsQuery.OrderBy(s => s.Code);
                    break;
                case "name_desc":
                    productsQuery = productsQuery.OrderByDescending(x => x.Name);
                    break;
                case "Name":
                    productsQuery = productsQuery.OrderBy(s => s.Name);
                    break;
                case "amount_desc":
                    productsQuery = productsQuery.OrderByDescending(x => x.AmountInPackage);
                    break;
                case "Amount":
                    productsQuery = productsQuery.OrderBy(s => s.AmountInPackage);
                    break;
                case "price_desc":
                    productsQuery = productsQuery.OrderByDescending(x => x.PackagePrice);
                    break;
                case "Price":
                    productsQuery = productsQuery.OrderBy(s => s.PackagePrice);
                    break;
                case "brand_desc":
                    productsQuery = productsQuery.OrderByDescending(x => x.Brand.Name);
                    break;
                case "Brand":
                    productsQuery = productsQuery.OrderBy(s => s.Brand.Name);
                    break;
                case "type_desc":
                    productsQuery = productsQuery.OrderByDescending(x => x.ProductType.Name);
                    break;
                case "Type":
                    productsQuery = productsQuery.OrderBy(s => s.ProductType.Name);
                    break;
            }

            var products = await _dataExecutor.ToListAsync(productsQuery);

            int pageSize = 7;
            int pageNumber = (page ?? 1);

            return View(products.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Create()
        {
            ViewBag.BrandId = new SelectList(_dataBaseManager.BrandRepository.Query, "Id", "Name");
            ViewBag.ProductTypeId = new SelectList(_dataBaseManager.ProductTypeRepository.Query, "Id", "Name");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Product product)
        {
            if (ModelState.IsValid)
            {
                _dataBaseManager.ProductRepository.Create(product);

                await _dataBaseManager.ProductRepository.CommitAsync();

                return RedirectToAction("Index");
            }

            ViewBag.BrandId = new SelectList(_dataBaseManager.BrandRepository.Query, "Id", "Name", product.BrandId);
            ViewBag.ProductTypeId = new SelectList(_dataBaseManager.ProductTypeRepository.Query, "Id", "Name", product.ProductTypeId);

            return View(product);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var product = await _dataExecutor.FirstOrDefaultAsync(_dataBaseManager.ProductRepository.Query, x => x.Id == id);

            if (product == null)
            {
                return HttpNotFound();
            }

            ViewBag.BrandId = new SelectList(_dataBaseManager.BrandRepository.Query, "Id", "Name", product.BrandId);
            ViewBag.ProductTypeId = new SelectList(_dataBaseManager.ProductTypeRepository.Query, "Id", "Name", product.ProductTypeId);

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                var entity = await _dataExecutor.FirstOrDefaultAsync(_dataBaseManager.ProductRepository.Query, x => x.Id == product.Id);

                if (entity.SupplyContents.Any() && entity.SupplyContents.Average(x => x.SupplyPrice) * 2 < product.PackagePrice)
                {
                    ModelState.AddModelError("", "Price cant be larger than 2 times as supply price");

                    ViewBag.BrandId = new SelectList(_dataBaseManager.BrandRepository.Query, "Id", "Name", product.BrandId);
                    ViewBag.ProductTypeId = new SelectList(_dataBaseManager.ProductTypeRepository.Query, "Id", "Name", product.ProductTypeId);

                    return View(product);
                }

                entity.Code = product.Code;
                entity.ProductTypeId = product.ProductTypeId;
                entity.BrandId = product.BrandId;
                entity.Name = product.Name;
                entity.Description = product.Description;
                entity.AmountInPackage = product.AmountInPackage;
                entity.PackagePrice = product.PackagePrice;

                await _dataBaseManager.ProductRepository.CommitAsync();

                return RedirectToAction("Index");
            }

            ViewBag.BrandId = new SelectList(_dataBaseManager.BrandRepository.Query, "Id", "Name", product.BrandId);
            ViewBag.ProductTypeId = new SelectList(_dataBaseManager.ProductTypeRepository.Query, "Id", "Name", product.ProductTypeId);

            return View(product);
        }

        [HttpPost]
        public async Task<bool> Delete(int id)
        {
            var product = await _dataExecutor.FirstOrDefaultAsync(_dataBaseManager.ProductRepository.Query, x => x.Id == id);

            _dataBaseManager.ProductRepository.Remove(product);

            await _dataBaseManager.ProductRepository.CommitAsync();

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
