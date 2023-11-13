using CafeteriaWeb.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CafeteriaWeb.Models
{
    public class Order
    {
        public int Id { get; set; }
        [Display(Name = "Observação")]
        public string? Observation { get; set; }

        [ScaffoldColumn(false)]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Total do Pedido")]
        public decimal TotalOrder { get; set; }

        [ScaffoldColumn(false)]
        [Display(Name = "Itens do Pedido")]
        public int TotalItensOrder { get; set; }

        [Display(Name = "Data do Pedido")]
        [DataType(DataType.Text)]
        [DisplayFormat(DataFormatString = "{0: dd/MM/yyyy hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime OrderDispatched { get; set; }

        [Display(Name = "Data Envio Pedido")]
        [DataType(DataType.Text)]
        [DisplayFormat(DataFormatString = "{0: dd/MM/yyyy hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime? OrderDelivered { get; set; }
        [Display(Name = "Método de Pagamento")]
        [Required(ErrorMessage = "Selecione um método de pagamento")]
        public PaymentMethod PaymentMethod { get; set; }
        [Display(Name = "Pagamento")]
        public bool PaymentOnline { get; set; }
        public string? TransactionId { get; set; }
        [Display(Name = "Está Pago?")]
        public bool IsPaid { get; set; }
        [Display(Name = "Para entrega?")]
        public bool ForDelivery { get; set; }
        public bool Finished { get; set; }
        public List<OrderDetail>? OrderItens { get; set; }
        public string UserId { get; set; }
        [Display(Name = "Cliente")]
        public virtual User User { get; set; }
        public int AdressId { get; set; }
        [Display(Name = "Endereço")]
        public virtual Adress Adress { get; set; }
        public bool Enabled { get; set; }

        [NotMapped]
        public string? StatusMessage { get; set; }
    }
}
