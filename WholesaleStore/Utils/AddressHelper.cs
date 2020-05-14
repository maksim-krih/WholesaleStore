using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WholesaleStore.Data.Interfaces;
using WholesaleStore.Models.Dtos;

namespace WholesaleStore.Utils
{
    public static class AddressHelper
    {
        public static void ConfigureDto(IDataBaseManager dataBaseManager, AddressDto model)
        {
            model.CountryList = new SelectList(dataBaseManager.CountryRepository.Query, "Id", "Name");

            if (model.CountryId.HasValue)
            {
                model.RegionList = new SelectList(dataBaseManager.RegionRepository.Query.Where(x => x.CountryId == model.CountryId.Value), "Id", "Name");
            }
            else
            {
                model.RegionList = new SelectList(Enumerable.Empty<SelectListItem>());
            }

            if (model.RegionId.HasValue)
            {
                model.CityList = new SelectList(dataBaseManager.CityRepository.Query.Where(x => x.RegionId == model.RegionId.Value), "Id", "Name");
            }
            else
            {
                model.CityList = new SelectList(Enumerable.Empty<SelectListItem>());
            }
        }
    }
}