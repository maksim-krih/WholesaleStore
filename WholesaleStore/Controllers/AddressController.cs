using System.Linq;
using System.Web.Mvc;
using WholesaleStore.Controllers.Base;
using WholesaleStore.Data.Interfaces;

namespace WholesaleStore.Controllers
{
    public class AddressController : BaseController
    {
        public AddressController(
            IDataExecutor dataExecutor,
            IGridManager gridManager,
            IDataBaseManager dataBaseManager) : base(gridManager, dataExecutor, dataBaseManager)
        {

        }

        [HttpGet]
        public JsonResult GetRegions(int countryId)
        {
            var regions = _dataBaseManager.RegionRepository.Query.Where(x => x.CountryId == countryId).Select(x => new
            {
                Value = x.Id,
                Text = x.Name
            });

            return Json(regions, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetCities(int regionId)
        {
            var cities = _dataBaseManager.CityRepository.Query.Where(x => x.RegionId == regionId).Select(x => new
            {
                Value = x.Id,
                Text = x.Name
            });

            return Json(cities, JsonRequestBehavior.AllowGet);
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
