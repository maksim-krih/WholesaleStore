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
    
    public partial class Order
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Order()
        {
            this.OrderContents = new HashSet<OrderContent>();
            this.OrderDeliveries = new HashSet<OrderDelivery>();
        }
    
        public int Id { get; set; }
        public int Number { get; set; }
        public int EmployeeId { get; set; }
        public int ClientId { get; set; }
        public int AddressId { get; set; }
        public System.DateTime Date { get; set; }
        public int TotalPrice { get; set; }
        public int Status { get; set; }
    
        public virtual Address Address { get; set; }
        public virtual Client Client { get; set; }
        public virtual Employee Employee { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderContent> OrderContents { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderDelivery> OrderDeliveries { get; set; }
    }
}
