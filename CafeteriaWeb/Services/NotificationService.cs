using CafeteriaWeb.Data;
using CafeteriaWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace CafeteriaWeb.Services
{
    public class NotificationService
    {
        readonly ApplicationDbContext _context;

        public NotificationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Notification>> ListAllAsync()
        {
            return await _context.Notification.Where(obj => obj.Enabled).ToListAsync();
        }

        public List<Notification> ListAll()
        {
            return _context.Notification.Where(obj => obj.Enabled).ToList();
        }

        public List<Notification> ListByUser(string userId)
        {
            return _context.Notification.Where(obj => obj.Enabled && obj.UserToNotifyId == userId).ToList();
        }

        public async Task<List<Notification>> ListByUserAsync(string userId)
        {
            return await _context.Notification.Where(obj => obj.Enabled && obj.UserToNotifyId == userId).ToListAsync();
        }

        public async Task<Notification> FindByIdAsync(int id)
        {
            return await _context.Notification.FirstOrDefaultAsync(obj => obj.Id == id && obj.Enabled);
        }

        public Notification FindById(int id)
        {
            return _context.Notification.FirstOrDefault(obj => obj.Id == id && obj.Enabled);
        }

        public void CreateNotification(Notification notification)
        {
            try
            {
                notification.CreatedOn = DateTime.Now;
                notification.IsRead = false;
                notification.Enabled = true;
                _context.Add(notification);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {

                throw;
            }       
        }


        #region Read Notification
        public void ReadNotification(int id)
        {
            var obj = _context.Notification.Find(id);
            if (!_context.Orders.Any(x => x.Id == obj.Id))
            {
                throw new Exception("Product not found");
            }
            try
            {
                obj.ReadOn = DateTime.Now;
                obj.IsRead = true;
                _context.Update(obj);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new DbUpdateConcurrencyException(e.Message);
            }
        }

        public async Task ReadNotificationAsync(int id)
        {
            var obj = _context.Notification.Find(id);
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
        #endregion

        #region delete
        public void Remove(int id)
        {
            var obj = _context.Notification.Find(id);
            _context.Notification.Remove(obj);
            _context.SaveChanges();
        }

        public async Task RemoveAsync(int id)
        {
            var obj = await _context.Notification.FindAsync(id);
            _context.Notification.Remove(obj);
            await _context.SaveChangesAsync();
        }
        #endregion
    }
}
