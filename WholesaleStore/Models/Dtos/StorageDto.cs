namespace WholesaleStore.Models.Dtos
{
    public class StorageDto : AddressDto
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public string ContactPhone { get; set; }
    }
}