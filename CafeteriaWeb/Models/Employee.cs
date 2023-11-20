using System.ComponentModel.DataAnnotations;

namespace CafeteriaWeb.Models
{
    public class Employee
    {
        public int Id { get; set; }
        [StringLength(50, ErrorMessage = "O tamanho máximo é 50 caracteres")]
        [Required(ErrorMessage = "Informe o Nome do funcionario")]
        [Display(Name = "Nome")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Informe o saláraio do funcionario")]
        [Display(Name = "Salário")]
        public decimal Wage { get; set; }
        [Display(Name = "Ativo")]
        public bool Enabled { get; set; }

        [Display(Name = "Criado em")]
        public DateTime CreatedOn { get; set; }
        [Display(Name = "Ultima modificação")]
        public DateTime ModifiedOn { get; set; }
    }
}
