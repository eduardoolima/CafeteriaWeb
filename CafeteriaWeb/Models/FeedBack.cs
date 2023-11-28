using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CafeteriaWeb.Models
{
    public class Feedback
    {
        public int Id { get; set; }
        [Display(Name = "Nome")]
        //public int OrderId { get; set; }
        //[Display(Name = "Pedido")]
        //public virtual Order Order { get; set; }
        public string UserId { get; set; }
        [Display(Name = "Cliente")]
        public virtual User User { get; set; }
        [Required(ErrorMessage = "Campo obrigatório")]
        [Display(Name = "Como foi sua experiência?")]
        [StringLength(300, MinimumLength = 2, ErrorMessage = "O {0} deve ter no mínimo {1} e no máximo {2} caracteres")]
        public string Text { get; set; }
        public bool Enabled { get; set; }

        [Display(Name = "Data do feedback")]
        public DateTime CreatedOn { get; set; }
        //[Display(Name = "Ultima modificação")]
        //public DateTime ModifiedOn { get; set; }
        [NotMapped]
        public string StatusMessage { get; set; }
    }
}
