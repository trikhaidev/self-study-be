using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RedisCachingDemo.Database.Entities
{
    [Table("City")]
    public class City
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int CountryId { get; set; }

        [Required]
        [StringLength(150)]
        [Column(TypeName = "NVARCHAR")]
        public string Name { get; set; } = null!;

        [ForeignKey("CountryId")]
        [Required]
        public Country Country { get; set; } = null!;
    }
}
