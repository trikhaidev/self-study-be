namespace JwtAuthentication.Database.Entities;
public class AppJwtKey
{
    public int Id {get;set;}
    public string KeyId {get;set;} = null!;
    public string PublicKey {get;set;} = null!;
    public string PrivateKey {get;set;} = null!;
    public DateTime Exp {get;set;}
    public bool IsActive {get;set;}
}