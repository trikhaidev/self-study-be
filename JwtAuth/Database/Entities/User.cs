using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JwtAuth.Database.Entities;
[Table("User")]
public class User
{
    [Key]
    public int Id {get;set;}

    [Required]
    [StringLength(50)]
    public string Username {get;set;} = null!;
    
    [Required]
    [StringLength(50)]
    public string Password {get;set;} = null!;

    [StringLength(50)]
    public string? Email {get;set;}

    public DateOnly? DateOfBirth {get;set;}
}