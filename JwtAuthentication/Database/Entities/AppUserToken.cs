namespace JwtAuthentication.Database.Entities;

public class AppUserToken
{
    public string RefreshToken {get;set;} = null!;
    public string AccessToken {get;set;} = null!;
    public int UserId {get;set;}
    public bool IsActive {get;set;}
    public DateTime Exp {get;set;}
    public AppUser? User {get;set;}
}