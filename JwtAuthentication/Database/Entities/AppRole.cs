using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JwtAuthentication.Database.Entities;
[Table("Role")]
public class AppRole
{
    [Key]
    public int Id {get;set;}
    
    [StringLength(100)]
    [Required]
    [Column(TypeName = "VARCHAR")]
    public string Name {get;set;} = null!;
}