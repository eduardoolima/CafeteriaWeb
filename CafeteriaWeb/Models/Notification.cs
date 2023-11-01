using CafeteriaWeb.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace CafeteriaWeb.Models
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Notification Title cannot be null")]
        [Display(Name = "Notificação")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Notification text cannot be null")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Notificação")]
        public string Text { get; set; }
        [Required]
        public bool IsRead { get; set; }
        [Required]
        public string UserToNotifyId { get; set; }
        public User UserToNotify { get; set; }
        [Required]
        public NotificationType NotificationType { get; set; }
        [Required]
        public DateTime CreatedOn { get; set; }
        public DateTime? ReadOn { get; set; }
        public bool Enabled { get; set; }
    }
}
