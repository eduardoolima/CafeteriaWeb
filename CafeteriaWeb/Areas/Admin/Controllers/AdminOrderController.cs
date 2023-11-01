using CafeteriaWeb.Models;
using CafeteriaWeb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CafeteriaWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize("Admin")]
    public class AdminOrderController : Controller
    {
        private readonly OrderService _orderService;
        private readonly ShoppingCart _shoppingCart;
        private readonly AdressService _adressService;
        private readonly UserManager<User> _userManager;
        private PaymentConfiguration _paymentConfiguration;
        public AdminOrderController(OrderService orderService,
            ShoppingCart shoppingCart,
            AdressService adressService,
            UserManager<User> userManager,
            IOptions<PaymentConfiguration> paymentConfiguration)
        {
            _orderService = orderService;
            _shoppingCart = shoppingCart;
            _adressService = adressService;
            _userManager = userManager;
            _paymentConfiguration = paymentConfiguration.Value;
        }

        public IActionResult Index()
        {
            List<Order> orders = _orderService.ListAll();
            return View(orders);
        }
    }
}
