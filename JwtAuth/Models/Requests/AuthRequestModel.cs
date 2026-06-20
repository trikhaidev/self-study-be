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

public class AuthRequestModel_Register
{
    [Required]
    [StringLength(50)]
    public string Username {get;set;} = null!;

    [EmailAddress]
    public string? Email {get;set;}

    [DataType(DataType.Date)]
    public DateOnly? DateOfBirth {get;set;}

    [Required]
    [StringLength(50)]
    public string Password {get;set;} = null!;

    [Required]
    [StringLength(50)]
    [Compare("Password")]
    public string ConfirmPassword {get;set;} = null!;
}