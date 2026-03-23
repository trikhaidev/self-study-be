namespace JwtAuthentication.Database.Entities;
public class AppUserRole
{
    public int UserId {get;set;}
    public int RoleId {get;set;}

    public AppUser? User {get;set;}
    public AppRole? Role {get;set;}
}