using MercadoPago.Config;

namespace CafeteriaWeb.Services
{
    public class PaymentConfiguration
    {
        public string? MercadoPagoAccessToken { get; set; }
        public string? PixDescription { get; set; }
        public string? PaymentNotificationUrl { get; set; }
    }
}
