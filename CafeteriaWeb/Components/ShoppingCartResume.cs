using CafeteriaWeb.Models;
using CafeteriaWeb.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace CafeteriaWeb.Components
{
    public class ShoppingCartResume : ViewComponent
    {
        private readonly ShoppingCart _shoppingCart;

        public ShoppingCartResume(ShoppingCart shoppingCart)
        {
            _shoppingCart = shoppingCart;
        }

        public IViewComponentResult Invoke()
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
    }
}
