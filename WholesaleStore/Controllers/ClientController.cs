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

        public async Task<ActionResult> Index()
        {
            var clients = await _dataExecutor.ToListAsync(_dataBaseManager.ClientRepository.Query);

            return View(clients);
        }

        public async Task<ActionResult> Details(int? id)
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

        public async Task<ActionResult> Delete(int? id)
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

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var client = await _dataExecutor.FirstOrDefaultAsync(_dataBaseManager.ClientRepository.Query, x => x.Id == id);

            _dataBaseManager.ClientRepository.Remove(client);

            await _dataBaseManager.ClientRepository.CommitAsync();

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
