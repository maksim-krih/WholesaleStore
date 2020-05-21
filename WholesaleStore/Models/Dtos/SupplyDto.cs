using System.Collections.Generic;
using System.Web.Mvc;

namespace WholesaleStore.Models.Dtos
{
    public class SupplyDto
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public int EmployeeId { get; set; }
        public int SupplierId { get; set; }
        public System.DateTime Date { get; set; }
        public int Status { get; set; }
        public List<SupplyContentDto> SupplyContents { get; set; }
        public SelectList ProductList { get; set; }
        public SelectList EmployeeList { get; set; }
        public SelectList StorageList { get; set; }
    }
}