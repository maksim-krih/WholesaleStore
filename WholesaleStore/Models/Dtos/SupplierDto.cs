namespace WholesaleStore.Models.Dtos
{
    public class SupplierDto : AddressDto
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string ContactPhone { get; set; }
    }
}