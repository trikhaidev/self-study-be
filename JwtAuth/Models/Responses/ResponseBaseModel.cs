public class ResponseBaseModel<T>
{
    public int StatusCode {get;set;}
    public bool IsOk {get;set;}
    public string? Message {get;set;}
    public T? Data {get;set;}
}