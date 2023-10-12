using Microsoft.AspNetCore.Identity;

namespace Cafeteria.Models
{
    public class User : IdentityUser
    {
        public List<Adress> Adresses { get; set; }
        public List<Order> Orders { get; set; }
    }
}
