using CafeteriaWeb.Data;
using Microsoft.EntityFrameworkCore;
using System;

namespace CafeteriaWeb.Models
{
    public class ShoppingCart
    {
        private readonly ApplicationDbContext _context;

        public ShoppingCart(ApplicationDbContext context)
        {
            _context = context;
        }

        public string Id { get; set; }
        public List<ShoppingCartItem> ShoppingCartItems { get; set; }
        public static ShoppingCart GetShoppingCart(IServiceProvider services)
        {
            //defines a session
            ISession session = services.GetRequiredService<IHttpContextAccessor>()?.HttpContext.Session;

            //get a service of the type of the context
            var context = services.GetService<ApplicationDbContext>();

            //get or set the ID of the ShoppingCart
            string shoppingCartId = session.GetString("ShoppingCartId") ?? Guid.NewGuid().ToString();

            //assign the id of the shopping cart in the session
            session.SetString("ShoppingCartId", shoppingCartId);

            //return the shopingcart whith the context and the Id assigned or obtained
            return new ShoppingCart(context)
            {
                Id = shoppingCartId
            };

        }

        public void AddToShoppingCart(Product product)
        {
            var shoppingCartItem = _context.ShoppingCartItems.SingleOrDefault(
                s => s.Product.Id == product.Id && s.ShoppingCartId == Id);

            if (shoppingCartItem == null)
            {
                shoppingCartItem = new ShoppingCartItem
                {
                    ShoppingCartId = Id,
                    Product = product,
                    Amount = 1
                };
                _context.ShoppingCartItems.Add(shoppingCartItem);
            }
            else
            {
                shoppingCartItem.Amount++;
            }
            _context.SaveChanges();
        }

        public int RemoveFromShoppingCart(Product product)
        {
            var shoppingCartItem = _context.ShoppingCartItems.SingleOrDefault(
                s => s.Product.Id == product.Id && s.ShoppingCartId == Id);
            var localAmount = 0;

            if (shoppingCartItem != null)
            {
                if (shoppingCartItem.Amount > 1)
                {
                    shoppingCartItem.Amount--;
                    localAmount = shoppingCartItem.Amount;
                }
                else
                {
                    _context.ShoppingCartItems.Remove(shoppingCartItem);
                }
            }
            _context.SaveChanges();
            return localAmount;
        }

        public int RemoveAllProductsFromShoppingCart(Product product)
        {
            var shoppingCartItem = _context.ShoppingCartItems.SingleOrDefault(
                s => s.Product.Id == product.Id && s.ShoppingCartId == Id);
            var localAmount = 0;

            if (shoppingCartItem != null)
            {
                _context.ShoppingCartItems.Remove(shoppingCartItem);
            }
            _context.SaveChanges();
            return localAmount;
        }

        public List<ShoppingCartItem> GetShoppingCartItems()
        {
            return ShoppingCartItems ??
                (ShoppingCartItems = _context.ShoppingCartItems.Where(c => c.ShoppingCartId == Id)
                .Include(p => p.Product).ToList());
        }

        public void ClearShoppingCart()
        {
            var items = _context.ShoppingCartItems.Where(c => c.ShoppingCartId == Id);
            _context.ShoppingCartItems.RemoveRange(items);
            _context.SaveChanges();
        }

        public decimal GetShoppingCartTotal()
        {
            var total = _context.ShoppingCartItems.Where(c => c.ShoppingCartId == Id).Select(c => c.Product.Price * c.Amount).Sum();
            return total;
        }
    }
}
