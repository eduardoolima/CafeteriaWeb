using CafeteriaWeb.Data;
using CafeteriaWeb.Models;

namespace CafeteriaWeb.Services
{
    public class OrderService
    {
        readonly ApplicationDbContext _context;
        private readonly ShoppingCart _shoppingCart;
        public OrderService(ApplicationDbContext context, ShoppingCart shoppingCart)
        {
            _context = context;
            _shoppingCart = shoppingCart;
        }

        public void CreateOrder(Order order)
        {
            order.OrderDispatched = DateTime.Now;
            _context.Orders.Add(order);
            _context.SaveChanges();

            var shoppingCartItems = _shoppingCart.ShoppingCartItems;

            foreach (var cartItem in shoppingCartItems)
            {
                var orderDetail = new OrderDetail()
                {
                    Amount = cartItem.Amount,
                    ProductId = cartItem.Product.Id,
                    OrderId = order.Id,
                    Price = cartItem.Product.Price
                };
                _context.OrderDetails.Add(orderDetail);
            }
            _context.SaveChanges();
        }
    }
}
