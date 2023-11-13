using MercadoPago.Config;

namespace CafeteriaWeb.Services
{
    public class NotificationConfiguration
    {
        public string? NotificationApprovedTitle { get; set; }
        public string? NotificationApprovedText { get; set; }
        public string? NotificationRepprovedTitle { get; set; }
        public string? NotificationRepprovedText { get; set; }
        public string? NotificationNewOrderTitle { get; set; }
        public string? NotificationNewOrderText { get; set; }
        public string? NotificationOutForDeliveryTitle { get; set; }
        public string? NotificationOutForDeliveryText { get; set; }
    }
}
