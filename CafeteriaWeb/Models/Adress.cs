using System.ComponentModel.DataAnnotations;

namespace CafeteriaWeb.Models
{
    public class Adress
    {
        public int Id { get; set; }
        [Display(Name = "Nome do endereço")]
        [StringLength(50)]
        public string? Name { get; set; }        
        [Required(ErrorMessage = "Informe a Rua")]
        [Display(Name = "Rua")]
        [StringLength(50)]
        public string Street { get; set; }
        [Required(ErrorMessage = "Informe o Bairro")]
        [Display(Name = "Bairro")]
        [StringLength(50)]
        public string Neighborhood { get; set; }
        [Required(ErrorMessage = "Informe o Numero")]
        [Display(Name = "Número")]
        [StringLength(50)]
        public string Number { get; set; }
        [Required(ErrorMessage = "Informe o CEP")]
        [StringLength(10, MinimumLength = 9, ErrorMessage = "CEP deve ter 9 caracteres")]
        [Display(Name = "CEP")]
        public string Cep { get; set; }
        [Required(ErrorMessage = "Informe a Cidade")]
        [Display(Name = "Cidade")]
        [StringLength(50)]
        public string Town { get; set; }
        [Display(Name = "Complemento")]
        [StringLength(100)]
        public string? Complement { get; set; }
        public bool Enabled { get; set; }
    }
}
