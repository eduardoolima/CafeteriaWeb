using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CafeteriaWeb.Models
{
    public class User : IdentityUser
    {
        public string? Cpf { get; set; }
        [Required(ErrorMessage = "Informe o seu Nome")]
        [Display(Name = "Primeiro Nome")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Informe o seu Sobrenome")]
        [Display(Name = "Sobrenome")]
        public string LastName { get; set; }
        public List<Adress> Adresses { get; set; }
        public List<Order> Orders { get; set; }
        public string? PathPhoto { get; set; }
        [Required]
        public DateTime CreatedOn { get; set; }
        public DateTime ModifyedOn { get; set; }
        public ICollection<Notification>? Notifications { get; set; }
        public bool Enabled { get; set; }
        [NotMapped]
        public string? CompleteName { get; set; }

        public static implicit operator User(Task<User?> v)
        {
            throw new NotImplementedException();
        }
    }
}
