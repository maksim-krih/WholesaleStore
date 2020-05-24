using System.Collections.Generic;
using System.Web.Mvc;

namespace WholesaleStore.Models.Dtos
{
    public class OrderDto : AddressDto
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public int EmployeeId { get; set; }
        public int ClientId { get; set; }
        public System.DateTime Date { get; set; }
        public int TotalPrice { get; set; }
        public int Status { get; set; }

        public Address AddressDto { get; set; }
        public Client Client { get; set; }
        public Employee Employee { get; set; }
        public List<OrderContentDto> OrderContents { get; set; }
        public List<OrderDelivery> OrderDeliveries { get; set; }

        public SelectList ProductList { get; set; }
        public SelectList EmployeeList { get; set; }
        public SelectList StorageList { get; set; }
    }
}