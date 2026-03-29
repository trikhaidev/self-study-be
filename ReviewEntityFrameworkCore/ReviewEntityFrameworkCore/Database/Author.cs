using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ReviewEntityFrameworkCore.Database
{
    [Table(nameof(Author))]
    public class Author
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = null!;
        [Range(0,150)]
        public int? Age { get; set; }

        public ICollection<Article>? Articles { get; set;}
    }
}
