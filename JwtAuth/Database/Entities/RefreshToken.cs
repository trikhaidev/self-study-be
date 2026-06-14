using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JwtAuth.Database.Entities;
[Table("RefreshToken")]
public class RefreshToken
{
    [Key]
    public int Id {get;set;}

    [Required]
    public int UserId {get;set;}

    [Required]
    [StringLength(500)]
    [Column(TypeName = "NVARCHAR")]
    public string RefreshTokenHash {get;set;} = null!;

    [Required]
    public bool IsActive {get;set;}

    [Required]
    [DataType(DataType.DateTime)]
    public DateTime Exp {get;set;}

    [Required]
    [ForeignKey("UserId")]
    public User User {get;set;} = null!;
}