using System.ComponentModel.DataAnnotations;

public class AuthRequestModel_Login
{
    [Required]
    [StringLength(50)]
    public string UserName {get;set;} = null!;
    [Required]
    [StringLength(50)]
    public string Password {get;set;} = null!;
}