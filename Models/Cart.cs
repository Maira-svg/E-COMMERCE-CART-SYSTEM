using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DarazUltimateMVC.Models
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string SessionId { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public int Quantity { get; set; } = 1;
        public DateTime AddedAt { get; set; } = DateTime.Now;

        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }
    }
}