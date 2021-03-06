﻿using PagedList;
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
    public class OrderShipmentController : BaseController
    {
        public OrderShipmentController(
            IDataExecutor dataExecutor,
            IGridManager gridManager,
            IDataBaseManager dataBaseManager) : base(gridManager, dataExecutor, dataBaseManager)
        {

        }

        public async Task<ActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.CountSortParm = sortOrder == "Count" ? "count_desc" : "Count";
            ViewBag.OrderContentSortParm = sortOrder == "OrderContent" ? "orderContent_desc" : "OrderContent";
            ViewBag.ProductSortParm = sortOrder == "Product" ? "product_desc" : "Product";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var orderShipmentsQuery = _dataBaseManager.OrderShipmentRepository.Query
                .Include(o => o.OrderContent).Include(o => o.ProductsInStorage);

            if (!String.IsNullOrEmpty(searchString))
            {
                orderShipmentsQuery = orderShipmentsQuery.Where(x => 
                    x.OrderContentId.ToString().Contains(searchString) ||
                    x.ProductInStrorageId.ToString().Contains(searchString));
            }

            switch (sortOrder)
            {
                case "count_desc":
                    orderShipmentsQuery = orderShipmentsQuery.OrderByDescending(x => x.Count);
                    break;
                case "Count":
                    orderShipmentsQuery = orderShipmentsQuery.OrderBy(s => s.Count);
                    break;
                case "orderContent_desc":
                    orderShipmentsQuery = orderShipmentsQuery.OrderByDescending(x => x.OrderContentId);
                    break;
                case "OrderContent":
                    orderShipmentsQuery = orderShipmentsQuery.OrderBy(s => s.OrderContentId);
                    break;
                case "product_desc":
                    orderShipmentsQuery = orderShipmentsQuery.OrderByDescending(x => x.ProductInStrorageId);
                    break;
                case "Product":
                    orderShipmentsQuery = orderShipmentsQuery.OrderBy(s => s.ProductInStrorageId);
                    break;
            }

            var orderShipments = await _dataExecutor.ToListAsync(orderShipmentsQuery);

            int pageSize = 7;
            int pageNumber = (page ?? 1);

            return View(orderShipments.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Create()
        {
            ViewBag.OrderContentId = new SelectList(_dataBaseManager.OrderContentRepository.Query, "Id", "Id");
            ViewBag.ProductInStrorageId = new SelectList(_dataBaseManager.ProductsInStorageRepository.Query, "Id", "Id");
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(OrderShipment orderShipment)
        {
            if (ModelState.IsValid)
            {
                _dataBaseManager.OrderShipmentRepository.Create(orderShipment);

                await _dataBaseManager.OrderShipmentRepository.CommitAsync();
                
                return RedirectToAction("Index");
            }

            ViewBag.OrderContentId = new SelectList(_dataBaseManager.OrderContentRepository.Query, "Id", "Id", orderShipment.OrderContentId);
            ViewBag.ProductInStrorageId = new SelectList(_dataBaseManager.ProductsInStorageRepository.Query, "Id", "Id", orderShipment.ProductInStrorageId);
            
            return View(orderShipment);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var orderShipment = await _dataExecutor.FirstOrDefaultAsync(
                _dataBaseManager.OrderShipmentRepository.Query
                .Include(o => o.OrderContent)
                .Include(o => o.ProductsInStorage),
                x => x.Id == id);

            if (orderShipment == null)
            {
                return HttpNotFound();
            }
            
            ViewBag.OrderContentId = new SelectList(_dataBaseManager.OrderContentRepository.Query, "Id", "Id", orderShipment.OrderContentId);
            ViewBag.ProductInStrorageId = new SelectList(_dataBaseManager.ProductsInStorageRepository.Query, "Id", "Id", orderShipment.ProductInStrorageId);
            
            return View(orderShipment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(OrderShipment orderShipment)
        {
            if (ModelState.IsValid)
            {
                var entity = await _dataExecutor.FirstOrDefaultAsync(
                _dataBaseManager.OrderShipmentRepository.Query, x => x.Id == orderShipment.Id);

                entity.OrderContentId = orderShipment.OrderContentId;
                entity.ProductInStrorageId = orderShipment.ProductInStrorageId;
                entity.Count = orderShipment.Count;
                entity.Date = orderShipment.Date;

                await _dataBaseManager.OrderShipmentRepository.CommitAsync();

                return RedirectToAction("Index");
            }
            
            ViewBag.OrderContentId = new SelectList(_dataBaseManager.OrderContentRepository.Query, "Id", "Id", orderShipment.OrderContentId);
            ViewBag.ProductInStrorageId = new SelectList(_dataBaseManager.ProductsInStorageRepository.Query, "Id", "Id", orderShipment.ProductInStrorageId);
            
            return View(orderShipment);
        }

        [HttpPost]
        public async Task<bool> Delete(int id)
        {
            var orderShipment = await _dataExecutor.FirstOrDefaultAsync(
                _dataBaseManager.OrderShipmentRepository.Query
                .Include(o => o.OrderContent)
                .Include(o => o.ProductsInStorage),
                x => x.Id == id);

            _dataBaseManager.OrderShipmentRepository.Remove(orderShipment);

            await _dataBaseManager.OrderShipmentRepository.CommitAsync();

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
