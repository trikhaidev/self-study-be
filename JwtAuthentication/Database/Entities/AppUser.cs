using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JwtAuthentication.Database.Entities;
[Table("User")]
public class AppUser
{
    [Key]
    public int Id {get;set;}

    [Required]
    [StringLength(100)]
    public string FullName {get;set;} = null!;

    [Column(TypeName = "Date")]
    public DateOnly? BirthDay {get;set;}

    [StringLength(250)]
    public string? Address {get;set;}

    [Required]
    [StringLength(100)]
    [Column(TypeName = "VARCHAR")]
    public string UserName {get;set;} = null!;

    [Required]
    [StringLength(100)]
    [Column(TypeName = "NVARCHAR")]
    public string Password {get;set;} = null!;

    public List<AppUserRole>? UserRoles {get;set;}
}