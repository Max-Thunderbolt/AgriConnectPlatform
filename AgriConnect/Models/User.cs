using System.ComponentModel.DataAnnotations;
using Supabase.Postgrest.Models;

namespace AgriConnect.Models
{
    public class User : BaseModel
    {
        [Key]
        public Guid Id { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
