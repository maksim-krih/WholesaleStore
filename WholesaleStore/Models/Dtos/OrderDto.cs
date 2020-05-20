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
        public List<OrderContent> OrderContents { get; set; }
        public SelectList ProductList { get; set; }
    }
}