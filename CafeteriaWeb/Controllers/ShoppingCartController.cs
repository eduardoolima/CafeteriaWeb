using CafeteriaWeb.Models;
using CafeteriaWeb.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CafeteriaWeb.Services;

namespace CafeteriaWeb.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly ProductService _productsService;
        private readonly ShoppingCart _shoppingCart;

        public ShoppingCartController(ProductService productsService, ShoppingCart shoppingCart)
        {
            _productsService = productsService;
            _shoppingCart = shoppingCart;
        }

        public IActionResult Index()
        {
            var itens = _shoppingCart.GetShoppingCartItems();
            _shoppingCart.ShoppingCartItems = itens;

            var shoppingCartVM = new ShoppingCartViewModel()
            {
                ShoppingCart = _shoppingCart,
                ShoppingCartTotal = _shoppingCart.GetShoppingCartTotal()
            };
            return View(shoppingCartVM);
        }

        [Authorize]
        public IActionResult AddItemToShoppingCart(int id)
        {
            var selectedProduct = _productsService.FindById(id);

            if(selectedProduct != null)
            {
                _shoppingCart.AddToShoppingCart(selectedProduct);
            }
            return RedirectToAction("Index");
        }

        [Authorize]
        public IActionResult RemoveItemFromShoppingCart(int id)
        {
            var selectedProduct = _productsService.FindById(id);

            if (selectedProduct != null)
            {
                _shoppingCart.RemoveFromShoppingCart(selectedProduct);
            }
            return RedirectToAction("Index");
        }

        public IActionResult RemoveAllProductFromShoppingCart(int id)
        {
            var selectedProduct = _productsService.FindById(id);

            if (selectedProduct != null)
            {
                _shoppingCart.RemoveAllProductsFromShoppingCart(selectedProduct);
            }
            
            return RedirectToAction("Index");
        }

        public IActionResult ClearFromShoppingCart()
        {
            _shoppingCart.ClearShoppingCart();            
            return RedirectToAction("Index");
        }
    }
}
