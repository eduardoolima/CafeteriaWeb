using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CafeteriaWeb.Models
{
    public class Supplier
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Nome Obrigatório")]
        [Display(Name = "Nome")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Endereço Obrigatório")]
        [Display(Name = "Endereço")]
        public string Adress { get; set; }
        [Required(ErrorMessage = "Telefone Obrigatório")]
        [Display(Name = "Telefone")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Email Obrigatório")]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Descrição Obrigatória")]
        [StringLength(200, ErrorMessage = "O tamanho máximo é 200 caracteres")]
        [Display(Name = "Descrição")]
        public string Description { get; set; }
        public int CategorySupplierId { get; set; }
        [Display(Name = "Categoria")]
        public virtual CategorySupplier? CategorySupplier { get; set; }
        public bool Enabled { get; set; }
        [Display(Name = "Criado em")]
        public DateTime CreatedOn { get; set; }
        [Display(Name = "Ultima modificação")]
        public DateTime ModifyedOn { get; set; }
    }
}
