using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CafeteriaWeb.Models
{
    public class Promotion
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Informe os produtos que estarão em promoção")]
        [Display(Name = "Produtos")]
        public ICollection<Product>? Products { get; set; }
        [Required(ErrorMessage = "Informe o novo preço")]
        [Display(Name = "Preço")]
        [Column(TypeName = "decimal(10,2)")]
        [Range(0.50, 99999.99, ErrorMessage = "O Preço deve estar entre R$0.50 e R$9999.99")]
        [DataType(DataType.Text)]
        public decimal OnSalePrice { get; set; }
        [Required(ErrorMessage = "Informe a data de inicio da promoção")]
        [Display(Name = "Data de Início")]
        public DateTime SaleStart { get; set; }
        [Required(ErrorMessage = "Informe a data do fim do promoção")]
        [Display(Name = "Data do Fim")]
        public DateTime SaleEnd { get; set; }
        public bool Enabled { get; set; }
        [Required]
        [Display(Name = "Criado em")]
        public DateTime CreatedOn { get; set; }
        [Required]
        [Display(Name = "Ultima modificação")]
        public DateTime ModifyedOn { get; set; }
        [NotMapped]
        public bool IsActive { get; set; }
    }
}
