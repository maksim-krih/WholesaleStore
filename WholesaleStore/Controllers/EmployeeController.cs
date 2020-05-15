using System.Data.Entity;
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

        public async Task<ActionResult> Index()
        {
            var employees = await _dataExecutor.ToListAsync(
                _dataBaseManager.EmployeeRepository.Query
                .Include(x => x.Address.City.Region.Country)
                .Include(e => e.Position)
                );

            return View(employees);
        }

        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var employee = await _dataExecutor.FirstOrDefaultAsync(
                _dataBaseManager.EmployeeRepository.Query
                .Include(x => x.Address.City.Region.Country)
                .Include(e => e.Position)
                );

            if (employee == null)
            {
                return HttpNotFound();
            }

            return View(employee);
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

        public async Task<ActionResult> Delete(int? id)
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

            return View(employee);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var employee = await _dataExecutor.FirstOrDefaultAsync(_dataBaseManager.EmployeeRepository.Query, x => x.Id == id);

            _dataBaseManager.EmployeeRepository.Remove(employee);

            await _dataBaseManager.EmployeeRepository.CommitAsync();

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
