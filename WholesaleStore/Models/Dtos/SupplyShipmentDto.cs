namespace WholesaleStore.Models.Dtos
{
    public class SupplyShipmentDto
    {
        public int Id { get; set; }
        public int SupplyContentId { get; set; }
        public int ProductInStorageId { get; set; }
        public System.DateTime Date { get; set; }
        public int Count { get; set; }
        public int EmployeeId { get; set; }
        public virtual ProductsInStorage ProductsInStorage { get; set; }
    }
}