public class JwtConfig
{
    public string Issuer {get;set;} = null!;
    public string Audience {get;set;} = null!;
    public int Expires {get;set;}
}