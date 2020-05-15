using System.Web.Mvc;

namespace WholesaleStore.Models.Dtos
{
    public class CityDto
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int? RegionId { get; set; }
        public int? CountryId { get; set; }

        public SelectList RegionList { get; set; }
        public SelectList CountryList { get; set; }
    }
}