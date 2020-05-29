using System.ComponentModel.DataAnnotations;

namespace WholesaleStore.Common.Enums
{
    public enum SupplyStatus
    {
        [Display(Name = "Waiting For Shipment")]
        WaitingForShipment,
        Delivering,
        Delivered
    }
}