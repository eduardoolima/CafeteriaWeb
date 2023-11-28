using CafeteriaWeb.Data;
using CafeteriaWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace CafeteriaWeb.Services
{
    public class FeedbackService
    {
        readonly ApplicationDbContext _context;
        public FeedbackService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Feedback>> ListAllAsync()
        {
            List<Feedback> feedbacks = await _context.Feedbacks.Include(o => o.User).ToListAsync();
            if(feedbacks != null)
            {
                return feedbacks.Where(obj => obj.Enabled).ToList();
            }
            else
            {
                return new List<Feedback>();
            }
        }

        public List<Feedback> ListAll()
        {
            List<Feedback> feedbacks = _context.Feedbacks.Include(o => o.User).ToList();
            if (feedbacks != null)
            {
                return feedbacks.Where(obj => obj.Enabled).ToList();
            }
            else
            {
                return null;
            }
        }

        public async Task<Feedback> FindByIdAsync(int id)
        {
            return await _context.Feedbacks.Include(obj => obj.User).FirstOrDefaultAsync(obj => obj.Id == id && obj.Enabled);            
        }

        public Feedback FindById(int id)
        {
            return _context.Feedbacks.Include(obj => obj.User).FirstOrDefault(obj => obj.Id == id && obj.Enabled);
        }

        #region add
        public async Task InsertAsync(Feedback obj)
        {
            try
            {
                obj.Enabled = true;
                obj.CreatedOn = DateTime.Now;
                _context.Add(obj);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void Insert(Feedback obj)
        {
            obj.Enabled = true;
            obj.CreatedOn = DateTime.Now;
            _context.Add(obj);
            _context.SaveChanges();
        }
        #endregion

        #region update
        public void Update(Feedback obj)
        {
            if (!_context.Feedbacks.Any(x => x.Id == obj.Id))
            {
                throw new Exception("Feedback not found");
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

        public async Task UpdateAsync(Feedback obj)
        {
            if (!await _context.Feedbacks.AnyAsync(x => x.Id == obj.Id && obj.Enabled))
            {
                throw new Exception("Feedback not found");
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
            var obj = _context.Feedbacks.Find(id);
            obj.Enabled = false;
            _context.Update(obj);
            _context.SaveChanges();
        }

        public async Task RemoveAsync(int id)
        {
            var obj = await _context.Feedbacks.FindAsync(id);
            obj.Enabled = false;
            _context.Update(obj);
            await _context.SaveChangesAsync();
        }

        public void Delete(int id)
        {
            var obj = _context.Feedbacks.Find(id);
            _context.Feedbacks.Remove(obj);
            _context.SaveChanges();
        }

        public async Task DeleteAsync(int id)
        {
            var obj = await _context.Feedbacks.FindAsync(id);
            _context.Feedbacks.Remove(obj);
            await _context.SaveChangesAsync();
        }
        #endregion

    }
}
