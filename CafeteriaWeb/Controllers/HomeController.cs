using CafeteriaWeb.Models;
using CafeteriaWeb.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CafeteriaWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ProductService _productsService;

        public HomeController(ILogger<HomeController> logger,
            ProductService productService)
        {
            _logger = logger;
            _productsService = productService;
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

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}