using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using WholesaleStore.Controllers.Base;
using WholesaleStore.Data.Interfaces;

namespace WholesaleStore.Controllers
{
    public class CountryController : BaseController
    {
        public CountryController(
            IDataExecutor dataExecutor,
            IGridManager gridManager,
            IDataBaseManager dataBaseManager) : base(gridManager, dataExecutor, dataBaseManager)
        {

        }

        public async Task<ActionResult> Index()
        {
            var countries = await _dataExecutor.ToListAsync(_dataBaseManager.CountryRepository.Query);

            return View(countries);
        }

        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var country = await _dataExecutor.FirstOrDefaultAsync(_dataBaseManager.CountryRepository.Query, x => x.Id == id);

            if (country == null)
            {
                return HttpNotFound();
            }

            return View(country);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Country country)
        {
            if (ModelState.IsValid)
            {
                _dataBaseManager.CountryRepository.Create(country);
                await _dataBaseManager.CountryRepository.CommitAsync();

                return RedirectToAction("Index");
            }

            return View(country);
        }

        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var country = await _dataExecutor.FirstOrDefaultAsync(_dataBaseManager.CountryRepository.Query, x => x.Id == id);

            if (country == null)
            {
                return HttpNotFound();
            }

            return View(country);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(Country country)
        {
            if (ModelState.IsValid)
            {
                var entity = await _dataExecutor.FirstOrDefaultAsync(_dataBaseManager.CountryRepository.Query, x => x.Id == country.Id);

                entity.Name = country.Name;

                await _dataBaseManager.CountryRepository.CommitAsync();

                return RedirectToAction("Index");
            }

            return View(country);
        }

        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var country = await _dataExecutor.FirstOrDefaultAsync(_dataBaseManager.CountryRepository.Query, x => x.Id == id);

            if (country == null)
            {
                return HttpNotFound();
            }

            return View(country);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            var country = await _dataExecutor.FirstOrDefaultAsync(_dataBaseManager.CountryRepository.Query, x => x.Id == id);

            _dataBaseManager.CountryRepository.Remove(country);

            await _dataBaseManager.CountryRepository.CommitAsync();

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
