//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WholesaleStore
{
    using System;
    using System.Collections.Generic;
    
    public partial class OrderDelivery
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int EmployeeId { get; set; }
        public System.DateTime ReceiveDate { get; set; }
        public System.DateTime DeliveryDate { get; set; }
    
        public virtual Employee Employee { get; set; }
        public virtual Order Order { get; set; }
    }
}
