using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JwtAuth.Database.Entities;
[Table("JwtKey")]
public class JwtKey
{
    [Key]
    public int Id {get;set;}

    [Required]
    [StringLength(100)]
    [Column(TypeName = "NVARCHAR(100)")]
    public string Code {get;set;} = null!;

    [Required]
    [StringLength(500)]
    [Column(TypeName = "NVARCHAR(500)")]
    public string PublicKey {get;set;} = null!;

    [Required]
    public bool IsActive {get;set;}
    
    [Required]
    public DateTime Exp {get;set;}
}