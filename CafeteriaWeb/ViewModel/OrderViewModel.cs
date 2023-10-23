using CafeteriaWeb.Models;

namespace CafeteriaWeb.ViewModel
{
    public class OrderViewModel
    {
        public required Order Order { get; set; }
        public string? PixCopyPaste { get; set; }
        public string? PixQrCode { get; set; }
    }
}
