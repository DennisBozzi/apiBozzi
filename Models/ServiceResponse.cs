namespace apiBozzi.Models;

public class ServiceResponse<T>
{
    public T? Object { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool Success { get; set; } = true;
}