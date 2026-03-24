using System.ComponentModel.DataAnnotations;

namespace JwtAuthentication.Models.Request;
public class LoginRequest
{
    [Required]
    [StringLength(100)]
    public string UserName {get;set;} = null!;
    [Required]
    [StringLength(100)]
    public string Password {get;set;} = null!;
}