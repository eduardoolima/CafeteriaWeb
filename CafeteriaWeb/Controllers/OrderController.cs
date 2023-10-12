using CafeteriaWeb.Models;
using CafeteriaWeb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CafeteriaWeb.Controllers
{    
    public class OrderController : Controller
    {
        private readonly OrderService _orderService;
        private readonly ShoppingCart _shoppingCart;

        public OrderController(OrderService orderService, ShoppingCart shoppingCart)
        {
            _orderService = orderService;
            _shoppingCart = shoppingCart;
        }

        [Authorize]
        public IActionResult Checkout()
        {
            return View();
        }

        [Authorize]
        [HttpPost]        
        public IActionResult Checkout(Order order)
        {
            int totalItensOrder = 0;
            decimal totalPriceOrder = 0.0m;
            List<ShoppingCartItem> items = _shoppingCart.GetShoppingCartItems();
            _shoppingCart.ShoppingCartItems = items;

            if(_shoppingCart.ShoppingCartItems.Count == 0)
            {
                ModelState.AddModelError("", "Seu carrinho está vazio!");
            }
            foreach(var item in items)
            {
                totalItensOrder += item.Amount;
                totalPriceOrder += item.Product.Price * item.Amount;
            }

            order.TotalItensOrder = totalItensOrder;
            order.TotalOrder = totalPriceOrder;
            if (ModelState.IsValid)
            {
                _orderService.CreateOrder(order);
                ViewBag.CheckoutMessage = "Obrigado pelo seu pedido :)";
                ViewBag.TotalOrder = _shoppingCart.GetShoppingCartTotal();

                _shoppingCart.ClearShoppingCart();
                return View("~/Views/Order/CheckoutComplete.cshtml", order);
            }
            return View(order);
        }
    }
}
