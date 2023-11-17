using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CafeteriaWeb.Models
{
    public class Product
    {
        public int Id { get; set; }
        [StringLength(80, MinimumLength = 2, ErrorMessage = "O {0} deve ter no mínimo {1} e no máximo {2} caracteres")]
        [Required(ErrorMessage = "Informe o Nome do produto")]
        [Display(Name = "Nome")]
        public string Name { get; set; }
        
        [Required(ErrorMessage = "Informe o preço")]
        [Display(Name = "Preço")]
        [Column(TypeName = "decimal(10,2)")]
        [Range(0.50, 99999.99, ErrorMessage = "O Preço deve estar entre R$0.50 e R$9999.99")]
        [DataType(DataType.Text)]
        public decimal Price { get; set; }
        [StringLength(80, MinimumLength = 2, ErrorMessage = "O campo {0} deve ter no mínimo {1} e no máximo {2} caracteres")]
        [Required(ErrorMessage = "Informe uma descrição curta do produto")]
        [Display(Name = "Descrição Rápida")]
        public string SmallDescription { get; set; }

        [StringLength(280,  ErrorMessage = "O campo {0} deve ter no máximo {2} caracteres")]
        [Display(Name = "Descrição Completa")]
        public string? Description { get; set; }
        //public int Avaible { get; set; } // Quantidade em estoque
        [Display(Name = "Imagem padrão")]
        public string? ImgPath { get; set; }

        [Display(Name = "Imagem miniatura")]
        public string? ImgThumbnailPath { get; set; }

        [Display(Name = "Favorito?")]
        public bool IsFavorite { get; set; }

        [Display(Name = "Disponível?")]
        public bool IsAvaible { get; set; }
        [Required]
        [Display(Name = "Criado em")]
        public DateTime CreatedOn { get; set; }
        [Required]
        [Display(Name = "Ultima modificação")]
        public DateTime ModifyedOn { get; set; }

        public int CategoryId { get; set; }
        [Display(Name = "Categoria")]
        public virtual Category? Category { get; set; }
        public bool Enabled { get; set; }
        public bool IsOnPromotion { get; set; }
        public int? PromotionId { get; set; }
        public virtual Promotion? Promotion { get; set; }

        [Display(Name = "Preço Especial")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal NormalPrice { get; set; }
        [NotMapped]
        public string? StatusMessage { get; set; }
        //[NotMapped]
        //public string? ImgPathOld { get; set; }
        //[NotMapped]
        //public string? ImgThumbnailPathOld { get; set; }
    }
}
