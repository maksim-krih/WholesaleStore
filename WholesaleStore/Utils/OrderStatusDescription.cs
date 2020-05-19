using WholesaleStore.Common.Enums;

namespace WholesaleStore.Utils
{
    public static class OrderStatusDescription
    {
        public static string GetDescription(this OrderStatus status)
        {
            switch (status)
            {
                case OrderStatus.Preparing:
                    return "Preparing";
                case OrderStatus.ReadyToDelivery:
                    return "Ready To Delivery";
                case OrderStatus.Delivered:
                    return "Delivered";
                default:
                    return "";
            }
        }
    }
}