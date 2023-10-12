using System.ComponentModel.DataAnnotations;

namespace Cafeteria.Models
{
    public class Category
    {
        public int Id { get; set; }
        [StringLength(50, ErrorMessage = "O tamanho máximo é 50 caracteres")]
        [Required(ErrorMessage = "Informe o Nome da categoria")]
        [Display(Name = "Nome")]
        public string Name { get; set; }
        [StringLength(200, ErrorMessage = "O tamanho máximo é 200 caracteres")]
        [Required(ErrorMessage = "Informe a descrição da categoria")]
        [Display(Name = "Descrição")]
        public string Description { get; set; }

        public List<Product> Products { get; set; }
    }
}
