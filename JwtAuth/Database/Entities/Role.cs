using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JwtAuth.Database.Entities;
[Table("Role")]
public class Role
{
    [Key]
    public int Id {get;set;}
    
    [Required]
    [StringLength(50)]
    public string Name {get;set;} = null!;

    [StringLength(100)]
    public string? Description {get;set;}
}