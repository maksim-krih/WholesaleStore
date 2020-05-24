using System.Collections.Generic;

namespace WholesaleStore.Models.Dtos
{
    public class OrderContentDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Count { get; set; }

        public Order Order { get; set; }
        public Product Product { get; set; }
        public List<OrderShipmentDto> OrderShipments { get; set; }
    }
}