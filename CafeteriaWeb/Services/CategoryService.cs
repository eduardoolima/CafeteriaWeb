using CafeteriaWeb.Data;
using CafeteriaWeb.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace CafeteriaWeb.Services
{
    public class CategoryService
    {
        readonly ApplicationDbContext _context;
        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> ListAllAsync()
        {
            List<Category> categories = await _context.Categories.ToListAsync();
            return categories.Where(obj => obj.Enabled).ToList();
        }

        public List<Category> ListAll()
        {
            List<Category> categories = _context.Categories.ToList();
            return categories.Where(obj => obj.Enabled).ToList();
        }

        public async Task<Category> FindByIdAsync(int id)
        {
            return await _context.Categories.FirstOrDefaultAsync(obj => obj.Id == id && obj.Enabled);
        }

        public Category FindById(int id)
        {
            return _context.Categories.FirstOrDefault(obj => obj.Id == id && obj.Enabled);
        }

        #region add
        public async Task InsertAsync(Category obj)
        {
            _context.Add(obj);
            await _context.SaveChangesAsync();
        }

        public void Insert(Category obj)
        {
            _context.Add(obj);
            _context.SaveChanges();
        }
        #endregion

        #region update
        public void Update(Category obj)
        {
            if (!_context.Categories.Any(x => x.Id == obj.Id))
            {
                throw new Exception("Category not found");
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

        public async Task UpdateAsync(Category obj)
        {
            if (!await _context.Categories.AnyAsync(x => x.Id == obj.Id && obj.Enabled))
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
            var obj = _context.Categories.Find(id);
            _context.Categories.Remove(obj);
            _context.SaveChanges();
        }

        public async Task RemoveAsync(int id)
        {
            var obj = await _context.Categories.FindAsync(id);
            _context.Categories.Remove(obj);
            await _context.SaveChangesAsync();
        }
        #endregion
    }
}
