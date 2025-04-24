using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgriConnect.Models
{
    public class Product{
        [Key]
        public int ProductId { get; set; }
        [ForeignKey("UserId")]
        public int UserId { get; set; }
        public string? ProductName { get; set; }
        public string? ProductDescription { get; set; }
        public decimal ProductPrice { get; set; }
        public int ProductQuantity { get; set; }
        public string? ProductImage { get; set; }
        public string? ProductCategory { get; set; }
    }
}
