﻿using PagedList;
using System;
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
    public class OrderController : BaseController
    {
        public OrderController(
            IDataExecutor dataExecutor,
            IGridManager gridManager,
            IDataBaseManager dataBaseManager) : base(gridManager, dataExecutor, dataBaseManager)
        {

        }

        public async Task<ActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.PriceSortParm = sortOrder == "Price" ? "price_desc" : "Price";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            ViewBag.StatusSortParm = sortOrder == "Status" ? "status_desc" : "Status";
            ViewBag.EmployeeSortParm = sortOrder == "Employee" ? "employee_desc" : "Employee";
            ViewBag.ClientSortParm = sortOrder == "Client" ? "client_desc" : "Client";
            ViewBag.NumberSortParm = sortOrder == "Number" ? "number_desc" : "Number";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var ordersQuery = _dataBaseManager.OrderRepository.Query
                .Include(x => x.Address.City.Region.Country)
                .Include(o => o.Client)
                .Include(o => o.Employee);

            if (!String.IsNullOrEmpty(searchString))
            {
                ordersQuery = ordersQuery.Where(x => x.Client.LastName.Contains(searchString)
                    || x.Client.FirstName.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "price_desc":
                    ordersQuery = ordersQuery.OrderByDescending(x => x.TotalPrice);
                    break;
                case "Date":
                    ordersQuery = ordersQuery.OrderBy(s => s.Date);
                    break;
                case "date_desc":
                    ordersQuery = ordersQuery.OrderByDescending(x => x.Date);
                    break;
                case "Price":
                    ordersQuery = ordersQuery.OrderBy(x => x.TotalPrice);
                    break;
                case "Status":
                    ordersQuery = ordersQuery.OrderBy(x => x.Status);
                    break;
                case "status_desc":
                    ordersQuery = ordersQuery.OrderByDescending(x => x.Status);
                    break;
                case "Client":
                    ordersQuery = ordersQuery.OrderBy(x => x.Client.FirstName).ThenBy(x => x.Client.LastName);
                    break;
                case "client_desc":
                    ordersQuery = ordersQuery.OrderByDescending(x => x.Client.FirstName).ThenByDescending(x => x.Client.LastName);
                    break;
                case "Employee":
                    ordersQuery = ordersQuery.OrderBy(x => x.Employee.FirstName).ThenBy(x => x.Employee.LastName);
                    break;
                case "employee_desc":
                    ordersQuery = ordersQuery.OrderByDescending(x => x.Employee.FirstName).ThenByDescending(x => x.Employee.LastName);
                    break;
                case "Number":
                    ordersQuery = ordersQuery.OrderBy(x => x.Number);
                    break;
                case "number_desc":
                    ordersQuery = ordersQuery.OrderByDescending(x => x.Number);
                    break;
            }

            var orders = await _dataExecutor.ToListAsync(ordersQuery);

            int pageSize = 7;
            int pageNumber = (page ?? 1);

            return View(orders.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Create()
        {
            var order = new OrderDto();

            AddressHelper.ConfigureDto(_dataBaseManager, order);

            ViewBag.ClientId = new SelectList(_dataBaseManager.ClientRepository.Query, "Id", "FullName");
            ViewBag.EmployeeId = new SelectList(_dataBaseManager.EmployeeRepository.Query, "Id", "FullName");

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(OrderDto orderDto)
        {
            if (ModelState.IsValid)
            {
                var order = new Order
                {
                    Number = orderDto.Number,
                    EmployeeId = orderDto.EmployeeId,
                    ClientId = orderDto.ClientId,
                    Date = orderDto.Date,
                    TotalPrice = orderDto.TotalPrice,
                    Status = (int)orderDto.Status,
                    Address = new Address
                    {
                        Address1 = orderDto.Address,
                        CityId = orderDto.CityId.Value,
                        ZipCode = orderDto.ZipCode
                    }
                };

                _dataBaseManager.OrderRepository.Create(order);
                await _dataBaseManager.OrderRepository.CommitAsync();

                return RedirectToAction("Index");
            }

            AddressHelper.ConfigureDto(_dataBaseManager, orderDto);

            ViewBag.ClientId = new SelectList(_dataBaseManager.ClientRepository.Query, "Id", "FullName", orderDto.ClientId);
            ViewBag.EmployeeId = new SelectList(_dataBaseManager.EmployeeRepository.Query, "Id", "FullName", orderDto.EmployeeId);

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
                Status = (OrderStatus)order.Status,
            };

            AddressHelper.ConfigureDto(_dataBaseManager, orderDto);

            ViewBag.ClientId = new SelectList(_dataBaseManager.ClientRepository.Query, "Id", "FullName", orderDto.ClientId);
            ViewBag.EmployeeId = new SelectList(_dataBaseManager.EmployeeRepository.Query, "Id", "FullName", orderDto.EmployeeId);

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
                entity.Status = (int)order.Status;

                await _dataBaseManager.OrderRepository.CommitAsync();

                return RedirectToAction("Index");
            }

            AddressHelper.ConfigureDto(_dataBaseManager, order);

            ViewBag.ClientId = new SelectList(_dataBaseManager.ClientRepository.Query, "Id", "FullName", order.ClientId);
            ViewBag.EmployeeId = new SelectList(_dataBaseManager.EmployeeRepository.Query, "Id", "FullName", order.EmployeeId);

            return View(order);
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
