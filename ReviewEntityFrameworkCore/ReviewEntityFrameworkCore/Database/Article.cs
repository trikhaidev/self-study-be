using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ReviewEntityFrameworkCore.Database
{
    [Table(nameof(Article))]
    public class Article
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [MaxLength(500)]
        [Column(TypeName = "NVARCHAR")]
        public string Title { get; set; } = null!;
        [Required]
        [Column(TypeName = "NVARCHAR(MAX)")]
        public string Description { get; set; } = null!;

        public int? AuthorId { get; set; }
        public Author? Author { get; set; }
    }
}
