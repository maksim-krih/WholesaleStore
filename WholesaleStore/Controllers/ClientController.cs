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
    public class ClientController : BaseController
    {
        public ClientController(
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

            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var clientsQuery = _dataBaseManager.ClientRepository.Query;

            if (!String.IsNullOrEmpty(searchString))
            {
                clientsQuery = clientsQuery.Where(x => x.FirstName.Contains(searchString) ||
                    x.LastName.Contains(searchString) || x.Email.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "firstName_desc":
                    clientsQuery = clientsQuery.OrderByDescending(x => x.FirstName);
                    break;
                case "FirstName":
                    clientsQuery = clientsQuery.OrderBy(s => s.FirstName);
                    break;
                case "lastName_desc":
                    clientsQuery = clientsQuery.OrderByDescending(x => x.LastName);
                    break;
                case "LastName":
                    clientsQuery = clientsQuery.OrderBy(s => s.LastName);
                    break;
                case "email_desc":
                    clientsQuery = clientsQuery.OrderByDescending(x => x.Email);
                    break;
                case "Email":
                    clientsQuery = clientsQuery.OrderBy(s => s.Email);
                    break;
            }

            var clients = await _dataExecutor.ToListAsync(clientsQuery);

            int pageSize = 7;
            int pageNumber = (page ?? 1);

            return View(clients.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Client client)
        {
            if (ModelState.IsValid)
            {
                _dataBaseManager.ClientRepository.Create(client);
                await _dataBaseManager.ClientRepository.CommitAsync();

                return RedirectToAction("Index");
            }

            return View(client);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var client = await _dataExecutor.FirstOrDefaultAsync(_dataBaseManager.ClientRepository.Query, x => x.Id == id);

            if (client == null)
            {
                return HttpNotFound();
            }

            return View(client);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Client client)
        {
            if (ModelState.IsValid)
            {
                var entity = await _dataExecutor.FirstOrDefaultAsync(_dataBaseManager.ClientRepository.Query, x => x.Id == client.Id);

                entity.FirstName = client.FirstName;
                entity.LastName = client.LastName;
                entity.Email = client.Email;
                entity.Phone = client.Phone;

                await _dataBaseManager.ClientRepository.CommitAsync();

                return RedirectToAction("Index");
            }

            return View(client);
        }

        [HttpPost]
        public async Task<bool> Delete(int id)
        {
            var client = await _dataExecutor.FirstOrDefaultAsync(_dataBaseManager.ClientRepository.Query, x => x.Id == id);

            _dataBaseManager.ClientRepository.Remove(client);

            await _dataBaseManager.ClientRepository.CommitAsync();

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
