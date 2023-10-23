using CafeteriaWeb.Models;
using CafeteriaWeb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CafeteriaWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ProductService _productsService;
        private readonly ShoppingCart _shoppingCart;
        private readonly IViewComponentHelper _viewComponentHelper;

        public HomeController(ILogger<HomeController> logger,
            ProductService productService,
            ShoppingCart shoppingCart,
            IViewComponentHelper viewComponentHelper)
        {
            _logger = logger;
            _productsService = productService;
            _shoppingCart = shoppingCart;
            _viewComponentHelper = viewComponentHelper;
        }

        public IActionResult Index()
        {
            var products = _productsService.ListAll();
            return View(products);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult AddItemToShoppingCart(int id)
        {
            var selectedProduct = _productsService.FindById(id);

            if (selectedProduct != null)
            {
                _shoppingCart.AddToShoppingCart(selectedProduct);
            }
            var summary = ViewComponent("ShoppingCartResume");
            return Content(summary.ToString(), "text/html");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}