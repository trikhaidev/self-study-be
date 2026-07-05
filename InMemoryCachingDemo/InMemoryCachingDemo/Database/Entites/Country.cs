using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InMemoryCachingDemo.Database.Entites
{
    [Table("Country")]
    public class Country
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(150)]
        [Column(TypeName = "NVARCHAR")]
        public string Name { get; set; } = null!;
        [StringLength(150)]
        [Column(TypeName ="NVARCHAR")]
        public string? NameOfVietNamese { get; set; }
        public ICollection<City> Cities { get; set; } = null!;
    }
}
