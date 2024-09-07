namespace dotnetLab.WebApi.Infrastructure.ResponseWrapper;

public class ApiResponse<T>
{
    public ApiResponse() { }

    public ApiResponse(T data, bool success = true, string message = null)
    {
        this.Success = success;
        this.Message = message;
        this.Data = data;
    }

    public string Id { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }
}