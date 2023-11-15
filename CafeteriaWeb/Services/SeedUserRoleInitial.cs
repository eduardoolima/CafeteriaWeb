using CafeteriaWeb.Models;
using Microsoft.AspNetCore.Identity;

namespace CafeteriaWeb.Services
{
    public class SeedUserRoleInitial
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public SeedUserRoleInitial(UserManager<User> userManager, RoleManager<IdentityRole> manager)
        {
            _userManager = userManager;
            _roleManager = manager;
        }

        public void SeedRoles()
        {
            if (!_roleManager.RoleExistsAsync("Client").Result)
            {
                IdentityRole role = new();
                role.Name = "Client";
                role.NormalizedName = "CLIENT";
                IdentityResult roleResult = _roleManager.CreateAsync(role).Result;
            }
            if (!_roleManager.RoleExistsAsync("Admin").Result)
            {
                IdentityRole role = new();
                role.Name = "Admin";
                role.NormalizedName = "ADMIN";
                IdentityResult roleResult = _roleManager.CreateAsync(role).Result;
            }
        }

        public void SeedUsers()
        {            
            if (_userManager.FindByEmailAsync("suporte@dev").Result == null)
            {
                User user = new()
                {
                    FirstName = "Suporte",
                    LastName = "Dev",
                    UserName = "suporte@dev",
                    PathPhoto = "~/Img/UserProfilePictures/desenvolvedor-100.png",
                    Email = "suporte@dev",
                    NormalizedUserName = "SUPORTE@DEV",
                    NormalizedEmail = "SUPORTE@DEV",
                    EmailConfirmed = true,
                    LockoutEnabled = false,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    CreatedOn = DateTime.Now
                };

                var result = _userManager.CreateAsync(user, "@1q2w3e4r!Q@W#E$R").Result;
                if (result.Succeeded)
                {
                    _userManager.AddToRoleAsync(user, "Admin").Wait();
                }
            }
            if (_userManager.FindByEmailAsync("externalClient@cafeteriaweb").Result == null)
            {
                User user = new()
                {
                    FirstName = "Cliente",
                    LastName = "Externo",
                    UserName = "ClienteExterno",
                    PathPhoto = "",
                    Email = "externalClient@cafeteriaweb",
                    NormalizedUserName = "EXTERNALCLIENT@CAFETERIAWEB",
                    NormalizedEmail = "EXTERNALCLIENT@CAFETERIAWEB",
                    EmailConfirmed = true,
                    LockoutEnabled = false,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    CreatedOn = DateTime.Now
                };

                var result = _userManager.CreateAsync(user, "@1q2w3e4r!Q@W#E$R").Result;
                if (result.Succeeded)
                {
                    _userManager.AddToRoleAsync(user, "Admin").Wait();
                }
            }
        }
    }
}
