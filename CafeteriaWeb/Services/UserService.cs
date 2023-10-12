using CafeteriaWeb.Data;
using CafeteriaWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace CafeteriaWeb.Services
{
    public class UserService
    {
        readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        #region Find

        #region get
        public async Task<User> FindByIdAsync(string id)
        {
            return await _context.Users.FirstOrDefaultAsync(obj => obj.Id == id);
        }

        public User FindById(string id)
        {
            return _context.Users.Include(obj => obj.Notifications).FirstOrDefault(obj => obj.Id == id);
        }

        public User FindByUserName(string userName)
        {
            return _context.Users.FirstOrDefault(obj => obj.UserName == userName);
        }

        public async Task<User> FindByUserNameAsync(string userName)
        {
            return await _context.Users.FirstOrDefaultAsync(obj => obj.UserName == userName);
        }

        #endregion

        #region list
        public async Task<List<User>> FindAllAsync()
        {
            try
            {
                return await _context.Users.ToListAsync();
            }
            catch
            {
                return null;
            }

        }

        #endregion

        #endregion

        #region Update
        public async Task UpdateAsync(User obj)
        {
            bool hasAny = await _context.Users.AnyAsync(x => x.Id == obj.Id);
            if (!hasAny)
            {
                throw new Exception("User not found");
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
    }
}
