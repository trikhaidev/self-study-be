namespace JwtAuthentication.Models.Response;
public class BaseResponseModel<T>
{
    public bool IsError {get;set;}
    public T? Data {get;set;}
    public string? Message {get;set;}
}