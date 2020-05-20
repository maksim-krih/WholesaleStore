using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using WholesaleStore.Common.Enums;
using WholesaleStore.Controllers.Base;
using WholesaleStore.Data.Interfaces;
using WholesaleStore.Models.Dtos;
using WholesaleStore.Utils;

namespace WholesaleStore.Controllers
{
    public class OrderManagerController : BaseController
    {
        public OrderManagerController(
            IDataExecutor dataExecutor,
            IGridManager gridManager,
            IDataBaseManager dataBaseManager) : base(gridManager, dataExecutor, dataBaseManager)
        {

        }

        public async Task<ActionResult> Order()
        {
            var orders = await _dataExecutor.ToListAsync(
                _dataBaseManager.OrderRepository.Query
                .Include(x => x.Address.City.Region.Country)
                .Include(o => o.Client)
                .Include(o => o.Employee)
                .Include($"{nameof(WholesaleStore.Order.OrderContents)}.{nameof(OrderContent.Product)}")
                .Include($"{nameof(WholesaleStore.Order.OrderContents)}.{nameof(OrderContent.OrderShipments)}")
                );

            return View(orders);
        }

        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var order = await _dataExecutor.FirstOrDefaultAsync(
                _dataBaseManager.OrderRepository.Query
                .Include(x => x.Address.City.Region.Country)
                .Include(o => o.Client)
                .Include(o => o.Employee),
                x => x.Id == id
                );

            if (order == null)
            {
                return HttpNotFound();
            }

            return View(order);
        }

        public ActionResult CreateOrder()
        {
            var order = new OrderDto();

            order.OrderContents = new List<OrderContent> 
            {
                new OrderContent { Count = 0, ProductId = 0 }
            };

            order.ProductList = new SelectList(_dataBaseManager.ProductRepository.Query, "Id", "Name");

            AddressHelper.ConfigureDto(_dataBaseManager, order);

            ViewBag.ClientId = new SelectList(_dataBaseManager.ClientRepository.Query, "Id", "FirstName");
            ViewBag.EmployeeId = new SelectList(_dataBaseManager.EmployeeRepository.Query, "Id", "FirstName");

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddOrderContent(OrderDto orderDto)
        {
            orderDto.OrderContents.Add(new OrderContent());
            orderDto.ProductList = new SelectList(_dataBaseManager.ProductRepository.Query, "Id", "Name");

            return PartialView("OrderContent", orderDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(OrderDto orderDto)
        {
            if (ModelState.IsValid)
            {
                var products = await _dataExecutor.ToListAsync(_dataBaseManager.ProductRepository.Query);
                
                var order = new Order
                {
                    Number = orderDto.Number,
                    EmployeeId = orderDto.EmployeeId,
                    ClientId = orderDto.ClientId,
                    Date = DateTime.Now,
                    TotalPrice = orderDto.OrderContents.Sum(x => x.Count * products.Find(p => p.Id == x.ProductId).PackagePrice),
                    Status = (int)OrderStatus.Preparing,
                    Address = new Address
                    {
                        Address1 = orderDto.Address,
                        CityId = orderDto.CityId.Value,
                        ZipCode = orderDto.ZipCode
                    },
                    OrderContents = orderDto.OrderContents

                };

                _dataBaseManager.OrderRepository.Create(order);
                await _dataBaseManager.OrderRepository.CommitAsync();

                return RedirectToAction("Order");
            }

            orderDto.ProductList = new SelectList(_dataBaseManager.ProductRepository.Query, "Id", "Name");

            AddressHelper.ConfigureDto(_dataBaseManager, orderDto);

            ViewBag.ClientId = new SelectList(_dataBaseManager.ClientRepository.Query, "Id", "FirstName", orderDto.ClientId);
            ViewBag.EmployeeId = new SelectList(_dataBaseManager.EmployeeRepository.Query, "Id", "FirstName", orderDto.EmployeeId);

            return View(orderDto);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var order = await _dataExecutor.FirstOrDefaultAsync(
                _dataBaseManager.OrderRepository.Query
                .Include(x => x.Address.City.Region.Country)
                .Include(o => o.Client)
                .Include(o => o.Employee),
                x => x.Id == id
                );

            if (order == null)
            {
                return HttpNotFound();
            }

            var orderDto = new OrderDto
            {
                Address = order.Address.Address1,
                ZipCode = order.Address.ZipCode,
                CityId = order.Address.CityId,
                CountryId = order.Address.City.Region.CountryId,
                Id = order.Id,
                RegionId = order.Address.City.RegionId,
                Number = order.Number,
                EmployeeId = order.EmployeeId,
                ClientId = order.ClientId,
                Date = order.Date,
                TotalPrice = order.TotalPrice,
                Status = order.Status,
            };

            AddressHelper.ConfigureDto(_dataBaseManager, orderDto);

            ViewBag.ClientId = new SelectList(_dataBaseManager.ClientRepository.Query, "Id", "FirstName", orderDto.ClientId);
            ViewBag.EmployeeId = new SelectList(_dataBaseManager.EmployeeRepository.Query, "Id", "FirstName", orderDto.EmployeeId);

            return View(orderDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(OrderDto order)
        {
            if (ModelState.IsValid)
            {
                var entity = await _dataExecutor.FirstOrDefaultAsync(
                    _dataBaseManager.OrderRepository.Query
                    .Include(x => x.Address.City.Region.Country)
                    .Include(o => o.Client)
                    .Include(o => o.Employee),
                    x => x.Id == order.Id
                );

                entity.Address.Address1 = order.Address;
                entity.Address.ZipCode = order.ZipCode;
                entity.Address.CityId = order.CityId.Value;
                entity.Number = order.Number;
                entity.EmployeeId = order.EmployeeId;
                entity.ClientId = order.ClientId;
                entity.Date = order.Date;
                entity.TotalPrice = order.TotalPrice;
                entity.Status = order.Status;

                await _dataBaseManager.OrderRepository.CommitAsync();

                return RedirectToAction("Order");
            }

            AddressHelper.ConfigureDto(_dataBaseManager, order);

            ViewBag.ClientId = new SelectList(_dataBaseManager.ClientRepository.Query, "Id", "FirstName", order.ClientId);
            ViewBag.EmployeeId = new SelectList(_dataBaseManager.EmployeeRepository.Query, "Id", "FirstName", order.EmployeeId);

            return View(order);
        }

        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var order = await _dataExecutor.FirstOrDefaultAsync(
                    _dataBaseManager.OrderRepository.Query
                    .Include(x => x.Address.City.Region.Country)
                    .Include(o => o.Client)
                    .Include(o => o.Employee),
                    x => x.Id == id
                );

            if (order == null)
            {
                return HttpNotFound();
            }

            return View(order);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var order = await _dataExecutor.FirstOrDefaultAsync(
                    _dataBaseManager.OrderRepository.Query
                    .Include(x => x.Address.City.Region.Country)
                    .Include(o => o.Client)
                    .Include(o => o.Employee),
                    x => x.Id == id
                );

            _dataBaseManager.OrderRepository.Remove(order);

            await _dataBaseManager.OrderRepository.CommitAsync();

            return RedirectToAction("Order");
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
