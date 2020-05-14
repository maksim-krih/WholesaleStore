using System.Web.Mvc;

namespace WholesaleStore.Models.Dtos
{
    public class AddressDto
    {
        public string ZipCode { get; set; }
        public string Address { get; set; }
        public int? CityId { get; set; }
        public int? RegionId { get; set; }
        public int? CountryId { get; set; }

        public SelectList CityList { get; set; }
        public SelectList RegionList { get; set; }
        public SelectList CountryList { get; set; }
    }
}