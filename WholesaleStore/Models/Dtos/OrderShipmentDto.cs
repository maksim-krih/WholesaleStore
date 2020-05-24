namespace WholesaleStore.Models.Dtos
{
    public class OrderShipmentDto
    {
        public int Id { get; set; }
        public int OrderContentId { get; set; }
        public int ProductInStrorageId { get; set; }
        public int Count { get; set; }
        public System.DateTime Date { get; set; }

        public OrderContent OrderContent { get; set; }
        public ProductsInStorage ProductsInStorage { get; set; }
    }
}