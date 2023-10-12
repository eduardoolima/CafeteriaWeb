using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CafeteriaWeb.Models
{
    [Serializable]
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

        public List<Product>? Products { get; set; }
        public bool Enabled { get; set; }

        [Display(Name = "Criado em")]
        public DateTime CreatedOn { get; set; }
        [Display(Name = "Ultima modificação")]
        public DateTime ModifyedOn { get; set; }
        [NotMapped]
        public string? StatusMessage { get; set; }

    }
}
