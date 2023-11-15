using CafeteriaWeb.Data;
using CafeteriaWeb.Models;
using CafeteriaWeb.ViewModel;
using Microsoft.EntityFrameworkCore;

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

        public async Task<List<Order>> ListAllAsync()
        {
            return await _context.Orders.Where(obj => obj.Enabled).ToListAsync();
        }

        public async Task<List<Order>> ListAllByDayAsync(DateTime date)
        {
            return await _context.Orders.Include(obj => obj.User).Include(obj => obj.Adress).Where(obj => obj.Enabled && obj.OrderDispatched.Date == date.Date).ToListAsync();
        }

        public async Task<List<Order>> ListAllByPeriodAsync(DateTime dateStart, DateTime dateEnd)
        {
            return await _context.Orders.Include(obj => obj.User).Include(obj => obj.Adress).Where(obj => obj.Enabled && obj.OrderDispatched.Date >= dateStart.Date && obj.OrderDispatched.Date <= dateEnd.Date)
            .ToListAsync();
        }

        public List<Order> ListAll()
        {            
            return _context.Orders.Include(obj => obj.User).Include(obj => obj.Adress).Where(obj => obj.Enabled).ToList();
        }

        public async Task<List<Order>> ListByUser(string userId)
        {
            return await _context.Orders.Include(a => a.Adress).Where(obj => obj.Enabled && obj.UserId == userId).ToListAsync();
        }

        public async Task<Order> FindByIdAsync(int id)
        {
            return await _context.Orders.Include(u => u.User).Include(a => a.Adress).Include(i => i.OrderItens).FirstOrDefaultAsync(obj => obj.Id == id && obj.Enabled);
        }

        public Order FindById(int id)
        {
            return _context.Orders.Include(obj => obj.User).Include(a => a.Adress).FirstOrDefault(obj => obj.Id == id && obj.Enabled);
        }

        public async Task<Order> FindByTransactionIdAsync(string transactionId)
        {
            return await _context.Orders.FirstOrDefaultAsync(obj => obj.TransactionId == transactionId && obj.Enabled);
        }

        public Order FindByTransactionId(string transactionId)
        {
            return _context.Orders.FirstOrDefault(obj => obj.TransactionId == transactionId && obj.Enabled);
        }

        public void CreateOrder(Order order)
        {
            order.Enabled = true;
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

        public void CreateOrderAdmin(Order order, List<Products> products)
        {
            order.Enabled = true;
            order.OrderDispatched = DateTime.Now;
            _context.Orders.Add(order);
            _context.SaveChanges();


            foreach (var item in products)
            {
                var orderDetail = new OrderDetail()
                {
                    Amount = item.Amount,
                    ProductId = item.ProductId,
                    OrderId = order.Id,
                    Price = item.Price
                };
                _context.OrderDetails.Add(orderDetail);
            }
            _context.SaveChanges();
        }

        #region update
        public void Update(Order obj)
        {
            if (!_context.Orders.Any(x => x.Id == obj.Id))
            {
                throw new Exception("Product not found");
            }
            try
            {
                _context.Update(obj);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new DbUpdateConcurrencyException(e.Message);
            }
        }

        public async Task UpdateAsync(Order obj)
        {
            if (!await _context.Orders.AnyAsync(x => x.Id == obj.Id && obj.Enabled))
            {
                throw new Exception("Order not found");
            }
            try
            {
                _context.Update(obj);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new DbUpdateConcurrencyException(e.Message);
            }
        }

        public async Task FinishOrderAsync(int id)
        {
            var obj = _context.Orders.Find(id) ?? throw new Exception("Order not found");
            try
            {
                if(obj.OrderDelivered == null)
                {
                    obj.OrderDelivered = DateTime.Now;
                }
                obj.Finished = true;
                obj.IsPaid = true;
                _context.Update(obj);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new DbUpdateConcurrencyException(e.Message);
            }
        }

        public async Task MarkOrderAsPayedAsync(int id)
        {
            var obj = _context.Orders.Find(id) ?? throw new Exception("Order not found");
            try
            {
                obj.IsPaid = true;
                _context.Update(obj);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new DbUpdateConcurrencyException(e.Message);
            }
        }

        public async Task OutForDeliveryAsync(int id)
        {
            var obj = _context.Orders.Find(id) ?? throw new Exception("Order not found");
            try
            {
                obj.OrderDelivered = DateTime.Now;
                _context.Update(obj);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                throw new DbUpdateConcurrencyException(e.Message);
            }
        }
        #endregion

        #region delete
        public void Remove(int id)
        {
            var obj = _context.Orders.Find(id);
            _context.Orders.Remove(obj);
            _context.SaveChanges();
        }

        public async Task RemoveAsync(int id)
        {
            var obj = await _context.Orders.FindAsync(id);
            obj.Enabled = false;
            _context.Orders.Update(obj);
            await _context.SaveChangesAsync();
        }
        #endregion
    }
}
