using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CafeteriaWeb.Models
{
    [Serializable]
    public class CategorySupplier
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Nome Obrigatório")]
        [Display(Name = "Nome")]
        public string Name { get; set; }
        [StringLength(200, ErrorMessage = "O tamanho máximo é 200 caracteres")]
        [Required(ErrorMessage = "Informe a descrição da categoria")]
        [Display(Name = "Descrição")]
        public string Description { get; set; }
        public List<Supplier>? Suppliers { get; set; }
        public bool Enabled { get; set; }

        [Display(Name = "Criado em")]
        public DateTime CreatedOn { get; set; }
        [Display(Name = "Ultima modificação")]
        public DateTime ModifyedOn { get; set; }
        [NotMapped]
        public string? StatusMessage { get; set; }

    }
}
