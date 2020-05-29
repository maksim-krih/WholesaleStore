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
                );

            return View(orders);
        }

        public async Task<ActionResult> Delivery()
        {
            var orders = await _dataExecutor.ToListAsync(
                _dataBaseManager.OrderRepository.Query
                .Include(x => x.Address.City.Region.Country)
                .Include(o => o.Client)
                .Include(o => o.Employee)
                .Include($"{nameof(WholesaleStore.Order.OrderContents)}.{nameof(OrderContent.Product)}")
                );

            var orderDtos = orders.Select(x => new OrderDto
            {
                AddressDto = x.Address,
                Client = x.Client,
                Date = x.Date,
                Employee = x.Employee,
                Number = x.Number,
                OrderContents = x.OrderContents.Select(oc => new OrderContentDto
                {
                    Count = oc.Count,
                    Product = oc.Product,
                    OrderShipments = oc.OrderShipments.Select(os => new OrderShipmentDto
                    {
                        Count = os.Count,
                        Date = os.Date,
                        OrderContent = os.OrderContent,
                        ProductsInStorage = os.ProductsInStorage
                    }).ToList()
                }).ToList(),
                OrderDeliveries = x.OrderDeliveries.ToList(),
                Status = x.Status,
                TotalPrice = x.TotalPrice,
                Id = x.Id
            });

            return View(orderDtos);
        }

        public ActionResult CreateOrder()
        {
            var order = new OrderDto();

            order.OrderContents = new List<OrderContentDto>
            {
                new OrderContentDto { Count = 0, ProductId = 0 }
            };

            order.ProductList = new SelectList(_dataBaseManager.ProductRepository.Query, "Id", "FullName");

            AddressHelper.ConfigureDto(_dataBaseManager, order);

            ViewBag.ClientId = new SelectList(_dataBaseManager.ClientRepository.Query, "Id", "FullName");
            ViewBag.EmployeeId = new SelectList(_dataBaseManager.EmployeeRepository.Query, "Id", "FullName");

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddOrderContent(OrderDto orderDto)
        {
            orderDto.OrderContents.Add(new OrderContentDto());
            orderDto.ProductList = new SelectList(_dataBaseManager.ProductRepository.Query, "Id", "FullName");

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
                    OrderContents = orderDto.OrderContents.Select(x => new OrderContent
                    {
                        Count = x.Count,
                        ProductId = x.ProductId
                    }).ToList()

                };

                _dataBaseManager.OrderRepository.Create(order);
                await _dataBaseManager.OrderRepository.CommitAsync();

                return RedirectToAction("Order");
            }

            orderDto.ProductList = new SelectList(_dataBaseManager.ProductRepository.Query, "Id", "FullName");

            AddressHelper.ConfigureDto(_dataBaseManager, orderDto);

            ViewBag.ClientId = new SelectList(_dataBaseManager.ClientRepository.Query, "Id", "FirstName", orderDto.ClientId);
            ViewBag.EmployeeId = new SelectList(_dataBaseManager.EmployeeRepository.Query, "Id", "FirstName", orderDto.EmployeeId);

            return View(orderDto);
        }

        public async Task<ActionResult> EditDelivery(int? id)
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

            var orderShipmentDto = new List<OrderShipmentDto>();

            for (var i = 0; i < order.OrderContents.Count; i++)
            {
                orderShipmentDto.Add(new OrderShipmentDto
                {
                    ProductsInStorage = new ProductsInStorage()
                });
            }

            var orderDto = new OrderDto
            {
                Id = order.Id,
                OrderContents = order.OrderContents.Select(x => new OrderContentDto
                {
                    Count = x.Count,
                    Product = x.Product,
                    OrderShipments = orderShipmentDto,

                }).ToList(),
                OrderDeliveries = new List<OrderDelivery>
                {
                    new OrderDelivery()
                }
            };

            orderDto.ProductList = new SelectList(_dataBaseManager.ProductRepository.Query, "Id", "FullName");
            orderDto.EmployeeList = new SelectList(_dataBaseManager.EmployeeRepository.Query, "Id", "FullName");
            orderDto.StorageList = new SelectList(_dataBaseManager.StorageRepository.Query, "Id", "Number");

            return View(orderDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditDelivery(OrderDto order)
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

                var orderContents = entity.OrderContents.ToList();

                for (var i = 0; i < entity.OrderContents.Count; i++)
                {
                    var orderShipmentDto = order.OrderContents[i].OrderShipments[0];

                    if (orderContents[i].OrderShipments.Count == 0)
                    {
                        orderContents[i].OrderShipments.Add(new OrderShipment());
                    }

                    var supplyShipment = orderContents[i].OrderShipments.FirstOrDefault();

                    supplyShipment.ProductsInStorage = new ProductsInStorage
                    {
                        StorageId = orderShipmentDto.ProductsInStorage.StorageId,
                        ProductId = orderContents[0].ProductId
                    };
                    supplyShipment.Date = DateTime.Now;
                }

                if (entity.OrderDeliveries.Count == 0)
                {
                    entity.OrderDeliveries.Add(new OrderDelivery());
                }

                var orderDeliveries = entity.OrderDeliveries.FirstOrDefault();

                orderDeliveries.DeliveryDate = DateTime.Now;
                orderDeliveries.ReceiveDate = DateTime.Now;
                orderDeliveries.EmployeeId = order.OrderDeliveries.FirstOrDefault().EmployeeId;

                entity.Status = (int)OrderStatus.Delivering;

                await _dataBaseManager.OrderRepository.CommitAsync();

                return RedirectToAction("Delivery");
            }

            order.ProductList = new SelectList(_dataBaseManager.ProductRepository.Query, "Id", "FullName");
            order.EmployeeList = new SelectList(_dataBaseManager.EmployeeRepository.Query, "Id", "FullName");
            order.StorageList = new SelectList(_dataBaseManager.StorageRepository.Query, "Id", "Number");

            return View(order);
        }

        [HttpPost]
        public async Task<bool> CloseDelivery(int? id)
        {
            var order = await _dataExecutor.FirstOrDefaultAsync(
                _dataBaseManager.OrderRepository.Query
                .Include(x => x.Address.City.Region.Country)
                .Include(o => o.Client)
                .Include(o => o.Employee),
                x => x.Id == id
            );

            var orderDeliveries = order.OrderDeliveries.FirstOrDefault();

            orderDeliveries.ReceiveDate = DateTime.Now;

            order.Status = (int)OrderStatus.Delivered;

            await _dataBaseManager.OrderRepository.CommitAsync();

            return true;
        }

        [HttpPost]
        public async Task<bool> Delete(int id)
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
