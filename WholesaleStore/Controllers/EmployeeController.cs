using PagedList;
using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using WholesaleStore.Controllers.Base;
using WholesaleStore.Data.Interfaces;
using WholesaleStore.Models.Dtos;
using WholesaleStore.Utils;

namespace WholesaleStore.Controllers
{
    public class EmployeeController : BaseController
    {
        private WholesaleStoreContext db = new WholesaleStoreContext();

        public EmployeeController(
            IDataExecutor dataExecutor,
            IGridManager gridManager,
            IDataBaseManager dataBaseManager) : base(gridManager, dataExecutor, dataBaseManager)
        {

        }

        public async Task<ActionResult> Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.FirstNameSortParm = sortOrder == "FirstName" ? "firstName_desc" : "FirstName";
            ViewBag.LastNameSortParm = sortOrder == "LastName" ? "lastName_desc" : "LastName";
            ViewBag.EmailSortParm = sortOrder == "Email" ? "email_desc" : "Email";
            ViewBag.LoginSortParm = sortOrder == "Login" ? "login_desc" : "Login";
            ViewBag.PasswordSortParm = sortOrder == "Password" ? "password_desc" : "Password";
            ViewBag.PositionSortParm = sortOrder == "Position" ? "position_desc" : "Position";

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var employeesQuery = _dataBaseManager.EmployeeRepository.Query
                .Include(x => x.Address.City.Region.Country)
                .Include(e => e.Position);

            if (!String.IsNullOrEmpty(searchString))
            {
                employeesQuery = employeesQuery.Where(x => x.FirstName.Contains(searchString) ||
                    x.LastName.Contains(searchString) || x.Email.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "firstName_desc":
                    employeesQuery = employeesQuery.OrderByDescending(x => x.FirstName);
                    break;
                case "FirstName":
                    employeesQuery = employeesQuery.OrderBy(s => s.FirstName);
                    break;
                case "lastName_desc":
                    employeesQuery = employeesQuery.OrderByDescending(x => x.LastName);
                    break;
                case "LastName":
                    employeesQuery = employeesQuery.OrderBy(s => s.LastName);
                    break;
                case "email_desc":
                    employeesQuery = employeesQuery.OrderByDescending(x => x.Email);
                    break;
                case "Email":
                    employeesQuery = employeesQuery.OrderBy(s => s.Email);
                    break;
                case "login_desc":
                    employeesQuery = employeesQuery.OrderByDescending(x => x.Login);
                    break;
                case "Login":
                    employeesQuery = employeesQuery.OrderBy(s => s.Login);
                    break;
                case "password_desc":
                    employeesQuery = employeesQuery.OrderByDescending(x => x.Password);
                    break;
                case "Password":
                    employeesQuery = employeesQuery.OrderBy(s => s.Password);
                    break;
                case "position_desc":
                    employeesQuery = employeesQuery.OrderByDescending(x => x.Position.Name);
                    break;
                case "Position":
                    employeesQuery = employeesQuery.OrderBy(s => s.Position.Name);
                    break;
            }

            var employees = await _dataExecutor.ToListAsync(employeesQuery);

            int pageSize = 7;
            int pageNumber = (page ?? 1);

            return View(employees.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Create()
        {
            var employee = new EmployeeDto();

            AddressHelper.ConfigureDto(_dataBaseManager, employee);

            ViewBag.PositionId = new SelectList(db.Positions, "Id", "Name");

            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(EmployeeDto employeeDto)
        {
            if (ModelState.IsValid)
            {
                var employee = new Employee
                {
                    PositionId = employeeDto.PositionId,
                    FirstName = employeeDto.FirstName,
                    LastName = employeeDto.LastName,
                    Email = employeeDto.Email,
                    Phone = employeeDto.Phone,
                    Login = employeeDto.Login,
                    Password = employeeDto.Password,
                    Address = new Address
                    {
                        Address1 = employeeDto.Address,
                        CityId = employeeDto.CityId.Value,
                        ZipCode = employeeDto.ZipCode
                    }
                };

                _dataBaseManager.EmployeeRepository.Create(employee);
                await _dataBaseManager.EmployeeRepository.CommitAsync();

                return RedirectToAction("Index");
            }

            AddressHelper.ConfigureDto(_dataBaseManager, employeeDto);
            ViewBag.PositionId = new SelectList(db.Positions, "Id", "Name", employeeDto.PositionId);

            return View(employeeDto);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var employee = await _dataExecutor.FirstOrDefaultAsync(
                _dataBaseManager.EmployeeRepository.Query
                .Include(x => x.Address.City.Region)
                .Include(e => e.Position)
                );

            if (employee == null)
            {
                return HttpNotFound();
            }

            var employeeDto = new EmployeeDto
            {
                Address = employee.Address.Address1,
                ZipCode = employee.Address.ZipCode,
                CityId = employee.Address.CityId,
                CountryId = employee.Address.City.Region.CountryId,
                Id = employee.Id,
                RegionId = employee.Address.City.RegionId,
                Email = employee.Email,
                Password = employee.Password,
                Login = employee.Login,
                Phone = employee.Phone,
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                PositionId = employee.PositionId
            };

            AddressHelper.ConfigureDto(_dataBaseManager, employeeDto);
            ViewBag.PositionId = new SelectList(db.Positions, "Id", "Name", employeeDto.PositionId);

            return View(employeeDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(EmployeeDto employee)
        {
            if (ModelState.IsValid)
            {
                var entity = await _dataExecutor.FirstOrDefaultAsync(_dataBaseManager.EmployeeRepository.Query.Include(x => x.Address), x => x.Id == employee.Id);

                entity.Address.Address1 = employee.Address;
                entity.Address.ZipCode = employee.ZipCode;
                entity.Address.CityId = employee.CityId.Value;
                entity.PositionId = employee.PositionId;
                entity.FirstName = employee.FirstName;
                entity.LastName = employee.LastName;
                entity.Email = employee.Email;
                entity.Phone = employee.Phone;
                entity.Login = employee.Login;
                entity.Password = employee.Password;

                await _dataBaseManager.EmployeeRepository.CommitAsync();

                return RedirectToAction("Index");
            }

            AddressHelper.ConfigureDto(_dataBaseManager, employee);
            ViewBag.PositionId = new SelectList(db.Positions, "Id", "Name", employee.PositionId);

            return View(employee);
        }

        [HttpPost]
        public async Task<bool> Delete(int id)
        {
            var employee = await _dataExecutor.FirstOrDefaultAsync(_dataBaseManager.EmployeeRepository.Query, x => x.Id == id);

            _dataBaseManager.EmployeeRepository.Remove(employee);

            await _dataBaseManager.EmployeeRepository.CommitAsync();

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
