using System.ComponentModel.DataAnnotations;

namespace Cafeteria.Models
{
    public class ShoppingCartItem
    {
        public int Id { get; set; }
        public Product Product { get; set; }
        public int Amount { get; set; }
        [StringLength(200)]
        public string ShoppingCartId { get; set; }
    }
}
