using WholesaleStore.Common.Enums;

namespace WholesaleStore.Utils
{
    public static class SupplyStatusDescription
    {
        public static string GetDescription(this SupplyStatus status)
        {
            switch (status)
            {
                case SupplyStatus.WaitingForShipment:
                    return "Waiting For Shipment";
                case SupplyStatus.Delivering:
                    return "Delivering";
                case SupplyStatus.Delivered:
                    return "Delivered";
                default:
                    return "";
            }
        }
    }
}