using CafeteriaWeb.Models;

namespace CafeteriaWeb.ViewModel
{
    public class PromotionViewModel
    {
        public int? PromotionId { get; set; }
        public List<int>? Products { get; set; }
        public decimal OnSalePrice { get; set; }
        public DateTime SaleStart { get; set; }
        public DateTime SaleEnd { get; set; }
    }
}
