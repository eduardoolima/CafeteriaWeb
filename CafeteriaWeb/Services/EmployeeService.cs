using CafeteriaWeb.Data;
using CafeteriaWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace CafeteriaWeb.Services
{
    public class EmployeeService
    {
        readonly ApplicationDbContext _context;

        public EmployeeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Employee>> ListAllEnabledAsync()
        {
            return await _context.Employees.Where(obj => obj.Enabled).ToListAsync();
        }

        public List<Employee> ListEnabledAll()
        {
            return _context.Employees.Where(obj => obj.Enabled).ToList();
        }

        public async Task<List<Employee>> ListAllAsync()
        {
            return await _context.Employees.ToListAsync();
        }

        public List<Employee> ListAll()
        {
            return _context.Employees.ToList();
        }

        public async Task<Employee> FindByIdAsync(int id)
        {
            return await _context.Employees.FirstOrDefaultAsync(obj => obj.Id == id);
        }

        public Employee FindById(int id)
        {
            return _context.Employees.FirstOrDefault(obj => obj.Id == id);
        }

        #region add
        public async Task InsertAsync(Employee obj)
        {
            try
            {
                obj.Enabled = true;
                obj.CreatedOn = DateTime.Now;
                obj.ModifiedOn = DateTime.Now;
                _context.Add(obj);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void Insert(Employee obj)
        {
            obj.Enabled = true;
            obj.CreatedOn = DateTime.Now;
            _context.Add(obj);
            _context.SaveChanges();
        }
        #endregion

        #region update
        public void Update(Employee obj)
        {
            if (!_context.Employees.Any(x => x.Id == obj.Id))
            {
                throw new Exception("Employee not found");
            }
            try
            {
                obj.ModifiedOn = DateTime.Now;
                _context.Update(obj);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                throw new DbUpdateConcurrencyException(e.Message);
            }
        }

        public async Task UpdateAsync(Employee obj)
        {
            if (!await _context.Employees.AnyAsync(x => x.Id == obj.Id && obj.Enabled))
            {
                throw new Exception("Employee not found");
            }
            try
            {
                obj.ModifiedOn = DateTime.Now;
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
            var obj = _context.Employees.Find(id);
            obj.Enabled = false;
            obj.ModifiedOn = DateTime.Now;
            _context.Update(obj);
            _context.SaveChanges();
        }

        public async Task RemoveAsync(int id)
        {
            var obj = await _context.Employees.FindAsync(id);
            obj.Enabled = false;
            obj.ModifiedOn = DateTime.Now;
            _context.Update(obj);
            await _context.SaveChangesAsync();
        }

        public void Delete(int id)
        {
            var obj = _context.Employees.Find(id);
            _context.Employees.Remove(obj);
            _context.SaveChanges();
        }

        public async Task DeleteAsync(int id)
        {
            var obj = await _context.Employees.FindAsync(id);
            _context.Employees.Remove(obj);
            await _context.SaveChangesAsync();
        }
        #endregion
    }
}
