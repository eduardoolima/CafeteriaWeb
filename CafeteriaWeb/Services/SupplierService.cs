using CafeteriaWeb.Data;
using CafeteriaWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace CafeteriaWeb.Services
{
    public class SupplierService
    {
        readonly ApplicationDbContext _context;
        public SupplierService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Supplier>> ListAllAsync()
        {
            List<Supplier> categories = await _context.Suppliers.Include(obj => obj.CategorySupplier).ToListAsync();
            return categories.Where(obj => obj.Enabled).ToList();
        }

        public List<Supplier> ListAll()
        {
            List<Supplier> categories = _context.Suppliers.Include(obj => obj.CategorySupplier).ToList();
            return categories.Where(obj => obj.Enabled).ToList();
        }

        public async Task<Supplier> FindByIdAsync(int id)
        {
            return await _context.Suppliers.Include(obj => obj.CategorySupplier).FirstOrDefaultAsync(obj => obj.Id == id && obj.Enabled);
        }

        public Supplier FindById(int id)
        {
            return _context.Suppliers.Include(obj => obj.CategorySupplier).FirstOrDefault(obj => obj.Id == id && obj.Enabled);
        }

        public bool SupplierExists(int id)
        {
            return (_context.Suppliers?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        #region add
        public async Task InsertAsync(Supplier obj)
        {
            try
            {
                obj.CreatedOn = DateTime.Now;
                obj.ModifyedOn = DateTime.Now;
                obj.Enabled = true;
                _context.Add(obj);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void Insert(Supplier obj)
        {
            obj.CreatedOn = DateTime.Now;
            obj.ModifyedOn = DateTime.Now;
            obj.Enabled = true;
            _context.Add(obj);
            _context.SaveChanges();
        }
        #endregion

        #region update
        public void Update(Supplier obj)
        {
            if (!_context.Suppliers.Any(x => x.Id == obj.Id))
            {
                throw new Exception("Category not found");
            }
            try
            {
                obj.ModifyedOn = DateTime.Now;
                _context.Update(obj);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new DbUpdateConcurrencyException(e.Message);
            }
        }

        public async Task UpdateAsync(Supplier obj)
        {
            if (!await _context.Suppliers.AnyAsync(x => x.Id == obj.Id && obj.Enabled))
            {
                throw new Exception("Supplier not found");
            }
            try
            {
                obj.ModifyedOn = DateTime.Now;
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
            var obj = _context.Suppliers.Find(id);
            obj.Enabled = false;
            obj.ModifyedOn = DateTime.Now;
            _context.Suppliers.Update(obj);
            _context.SaveChanges();
        }

        public async Task RemoveAsync(int id)
        {
            var obj = await _context.Suppliers.FindAsync(id);
            obj.Enabled = false;
            obj.ModifyedOn = DateTime.Now;
            _context.Suppliers.Update(obj);
            await _context.SaveChangesAsync();
        }

        public void Delete(int id)
        {
            var obj = _context.Suppliers.Find(id);
            _context.Suppliers.Remove(obj);
            _context.SaveChanges();
        }

        public async Task DeleteAsync(int id)
        {
            var obj = await _context.Suppliers.FindAsync(id);
            _context.Suppliers.Remove(obj);
            await _context.SaveChangesAsync();
        }
        #endregion
    }
}
