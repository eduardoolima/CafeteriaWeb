using CafeteriaWeb.Data;
using CafeteriaWeb.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace CafeteriaWeb.Services
{
    public class CategorySupplierService
    {
        readonly ApplicationDbContext _context;
        public CategorySupplierService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CategorySupplier>> ListAllAsync()
        {
            List<CategorySupplier> categories = await _context.CategorySupplier.ToListAsync();
            return categories.Where(obj => obj.Enabled).ToList();
        }

        public List<CategorySupplier> ListAll()
        {
            List<CategorySupplier> categories = _context.CategorySupplier.ToList();
            return categories.Where(obj => obj.Enabled).ToList();
        }

        public async Task<CategorySupplier> FindByIdAsync(int id)
        {
            return await _context.CategorySupplier.FirstOrDefaultAsync(obj => obj.Id == id && obj.Enabled);
        }

        public CategorySupplier FindById(int id)
        {
            return _context.CategorySupplier.FirstOrDefault(obj => obj.Id == id && obj.Enabled);
        }

        public bool CategoryExists(int id)
        {
            return (_context.CategorySupplier?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        #region add
        public async Task InsertAsync(CategorySupplier obj)
        {
            obj.Enabled = true;
            obj.CreatedOn = DateTime.Now;
            _context.Add(obj);
            await _context.SaveChangesAsync();
        }

        public void Insert(CategorySupplier obj)
        {
            obj.Enabled = true;
            obj.CreatedOn = DateTime.Now;
            _context.Add(obj);
            _context.SaveChanges();
        }
        #endregion

        #region update
        public void Update(CategorySupplier obj)
        {
            if (!_context.CategorySupplier.Any(x => x.Id == obj.Id))
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

        public async Task UpdateAsync(CategorySupplier obj)
        {
            if (!await _context.CategorySupplier.AnyAsync(x => x.Id == obj.Id))
            {
                throw new Exception("Category not found");
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
            var obj = _context.CategorySupplier.Find(id);
            obj.Enabled = false;
            obj.ModifyedOn = DateTime.Now;
            _context.CategorySupplier.Update(obj);
            _context.SaveChanges();
        }

        public async Task RemoveAsync(int id)
        {
            var obj = await _context.CategorySupplier.FindAsync(id);
            obj.Enabled = false;
            obj.ModifyedOn = DateTime.Now;
            _context.CategorySupplier.Update(obj);
            await _context.SaveChangesAsync();
        }

        public void Delete(int id)
        {
            var obj = _context.CategorySupplier.Find(id);
            _context.CategorySupplier.Remove(obj);
            _context.SaveChanges();
        }

        public async Task DeleteAsync(int id)
        {
            var obj = await _context.CategorySupplier.FindAsync(id);
            _context.CategorySupplier.Remove(obj);
            await _context.SaveChangesAsync();
        }

        internal Task<CategorySupplier> FindByIdAsync(object value)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
