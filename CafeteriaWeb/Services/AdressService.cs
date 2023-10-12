using CafeteriaWeb.Data;
using CafeteriaWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace CafeteriaWeb.Services
{
    public class AdressService
    {
        readonly ApplicationDbContext _context;
        public AdressService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Adress> FindByIdAsync(int id)
        {
            return await _context.Adress.FirstOrDefaultAsync(obj => obj.Id == id);
        }

        public Adress FindById(int id)
        {
            return _context.Adress.FirstOrDefault(obj => obj.Id == id);
        }


        public List<Adress> ListByUserId(string userId)
        {
            return _context.Users.Where(u => u.Id == userId) .SelectMany(u => u.Adresses).ToList();
        }

        #region add
        public async Task InsertAsync(Adress obj)
        {
            _context.Add(obj);
            await _context.SaveChangesAsync();
        }

        public void Insert(Adress obj)
        {
            _context.Add(obj);
            _context.SaveChanges();
        }
        #endregion

        #region update
        public void Update(Adress obj)
        {
            if (!_context.Adress.Any(x => x.Id == obj.Id))
            {
                throw new Exception("Project not found");
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

        public async Task UpdateAsync(Adress obj)
        {
            if (!await _context.Adress.AnyAsync(x => x.Id == obj.Id))
            {
                throw new Exception("Project not found");
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
            var obj = _context.Adress.Find(id);
            _context.Adress.Remove(obj);
            _context.SaveChanges();
        }

        public async Task RemoveAsync(int id)
        {
            var obj = await _context.Adress.FindAsync(id);
            _context.Adress.Remove(obj);
            await _context.SaveChangesAsync();
        }
        #endregion
    }
}
