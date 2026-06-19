namespace JwtAuth.Services.options;
public class RefreshTokenConfig
{
    public string Key {get;set;} = null!;
    public bool HttpOnly {get;set;}
    public bool Secure {get;set;}
    public int SameSite {get;set;}
    public string Path {get;set;} = null!;
    public int ExpiressDay {get;set;}
}