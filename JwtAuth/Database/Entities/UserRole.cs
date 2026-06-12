using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JwtAuth.Database.Entities;
[Table("UserRole")]
public class UserRole
{
    public int UserId {get;set;}
    public int RoleId {get;set;}

    [Required]
    [ForeignKey("UserId")]
    public User User {get;set;} = null!;
    [Required]
    [ForeignKey("RoleId")]
    public Role Role {get;set;} = null!;
}