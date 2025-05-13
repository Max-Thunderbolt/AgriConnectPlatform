using System.ComponentModel.DataAnnotations;

namespace AgriConnectPlatform.Models
{
    public enum ProductCategory
    {
        Vegetables,
        Fruits,
        Grains,
        Livestock,
        Dairy,
        Poultry,
        Seafood,
        Herbs,
        Flowers,
        Other
    }

    public class Product
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="Name is Required")]
        public string productName { get; set; }
        public string Description { get; set; }
        public ProductCategory? Category { get; set; }
        public DateTime? DateCreated { get; set; }
        public string? CreatedByUserId { get; set; }
    }
}
