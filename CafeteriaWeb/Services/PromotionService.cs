using CafeteriaWeb.Data;
using CafeteriaWeb.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace CafeteriaWeb.Services
{
    public class PromotionService
    {
        readonly ApplicationDbContext _context;
        public PromotionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Promotion>> ListAllAsync()
        {
            List<Promotion> promotions = await _context.Promotions.Include(obj => obj.Products).ToListAsync();
            return promotions.Where(obj => obj.Enabled).ToList();
        }

        public List<Promotion> ListAll()
        {
            List<Promotion> promotions = _context.Promotions.Include(obj => obj.Products).ToList();
            return promotions.Where(obj => obj.Enabled).ToList();
        }

        public async Task<Promotion> FindByIdAsync(int id)
        {            
            return await _context.Promotions.Include(p => p.Products).FirstOrDefaultAsync(obj => obj.Id == id && obj.Enabled);
        }

        public Promotion FindById(int id)
        {
            return _context.Promotions.Include(p => p.Products).FirstOrDefault(obj => obj.Id == id && obj.Enabled);
        }

        //public bool ProductHasPromotion(int id) { }

        public List<Promotion> ListByProductId(int id)
        {
            List<Promotion> promotions = _context.Promotions.Include(s => s.Products).Where(s => s.Products.Any(p => p.Id == id)).ToList();            
            return promotions;
        }

        #region add
        public async Task InsertAsync(Promotion obj)
        {
            try
            {
                obj.Enabled = true;
                obj.CreatedOn = DateTime.Now;
                obj.ModifyedOn = DateTime.Now;
                _context.Add(obj);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public void Insert(Promotion obj)
        {
            obj.Enabled = true;
            obj.CreatedOn = DateTime.Now;
            obj.ModifyedOn = DateTime.Now;
            _context.Add(obj);
            _context.SaveChanges();
        }
        #endregion

        #region update
        public void Update(Promotion obj)
        {
            if (!_context.Promotions.Any(x => x.Id == obj.Id && obj.Enabled))
            {
                throw new Exception("Promotions not found");
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

        public async Task UpdateAsync(Promotion obj)
        {
            if (!await _context.Promotions.AnyAsync(x => x.Id == obj.Id && obj.Enabled))
            {
                throw new Exception("Promotions not found");
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
            var obj = _context.Promotions.Find(id);
            obj.Enabled = false;
            _context.Update(obj);
            _context.SaveChanges();
        }

        public async Task RemoveAsync(int id)
        {
            var obj = await _context.Promotions.FindAsync(id);

            if (obj != null)
            {
                obj.Enabled = false;
                var productsToUpdate = await _context.Products
                .Where(p => p.PromotionId == id && p.Enabled)
                .ToListAsync();

                foreach (var product in productsToUpdate)
                {
                    product.PromotionId = null;
                    _context.Update(product);
                }
                _context.Update(obj);
                await _context.SaveChangesAsync();
            }             
           
        }

        public void Delete(int id)
        {
            var obj = _context.Promotions.Find(id);
            _context.Promotions.Remove(obj);
            _context.SaveChanges();
        }

        public async Task DeleteAsync(int id)
        {
            var obj = await _context.Promotions.FindAsync(id);
            _context.Promotions.Remove(obj);
            await _context.SaveChangesAsync();
        }
        #endregion
    }
}
