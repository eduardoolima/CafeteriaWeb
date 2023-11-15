using CafeteriaWeb.Models;
using System.ComponentModel.DataAnnotations;

namespace CafeteriaWeb.ViewModel
{
    public class AdminOrderViewModel
    {
        public required Order Order { get; set; }
        [Required(ErrorMessage = "Defina os produtos do pedido")]
        public List<Products> Products { get; set; }
        [Display(Name = "Cliente Externo?")]
        public bool ExternalClient { get; set; }
        [Display(Name = "Endereço")]
        public string? Adress { get; set; }
    }

    public class Products
    {
        public int ProductId { get; set; }
        public int Amount { get; set; }
        public decimal Price { get; set; }
    }
}
