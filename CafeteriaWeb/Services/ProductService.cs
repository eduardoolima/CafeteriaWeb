using CafeteriaWeb.Data;
using CafeteriaWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace CafeteriaWeb.Services
{
    public class ProductService
    {
        readonly ApplicationDbContext _context;
        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> ListAllAsync()
        {
            List<Product> products = await _context.Products.Include(obj => obj.Category).ToListAsync();
            return products.Where(obj => obj.Enabled).ToList();
        }

        public List<Product> ListAll()
        {
            List<Product> products = _context.Products.Include(obj => obj.Category).ToList();
            return products.Where(obj => obj.Enabled).ToList();
        }

        public async Task<List<Product>> ListByCategory(int categoryId)
        {
            List<Product> products = await _context.Products.Include(obj => obj.Category).ToListAsync();
            return products.Where(obj => obj.Enabled && obj.CategoryId == categoryId).ToList();
        }

        public async Task<Product> FindByIdAsync(int id)
        {
            return await _context.Products.FirstOrDefaultAsync(obj => obj.Id == id && obj.Enabled);
        }

        public Product FindById(int id)
        {
            return _context.Products.FirstOrDefault(obj => obj.Id == id && obj.Enabled);
        }

        #region add
        public async Task InsertAsync(Product obj)
        {
            try
            {
                _context.Add(obj);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public void Insert(Product obj)
        {
            _context.Add(obj);
            _context.SaveChanges();
        }
        #endregion

        #region update
        public void Update(Product obj)
        {
            if (!_context.Products.Any(x => x.Id == obj.Id))
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

        public async Task UpdateAsync(Product obj)
        {
            if (!await _context.Products.AnyAsync(x => x.Id == obj.Id && obj.Enabled))
            {
                throw new Exception("Product not found");
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
            var obj = _context.Products.Find(id);
            _context.Products.Remove(obj);
            _context.SaveChanges();
        }

        public async Task RemoveAsync(int id)
        {
            var obj = await _context.Products.FindAsync(id);
            _context.Products.Remove(obj);
            await _context.SaveChangesAsync();
        }
        #endregion
    }
}
