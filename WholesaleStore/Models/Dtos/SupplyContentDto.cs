using System.Collections.Generic;

namespace WholesaleStore.Models.Dtos
{
    public class SupplyContentDto
    {
        public int Id { get; set; }
        public int SupplyId { get; set; }
        public int ProductId { get; set; }
        public int Count { get; set; }
        public int SupplyPrice { get; set; }

        public Product Product { get; set; }
        public List<SupplyShipmentDto> SupplyShipments { get; set; }
    }
}