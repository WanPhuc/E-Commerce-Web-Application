namespace WebBanHang.Models.Common;

public class ApiResponse<T>
{
    public int Status { get; set; }
    public string Message { get; set; } = string.Empty;
    public T Data { get; set; } = default!;

    public static ApiResponse<T> Ok(T data, string message="Sucssess",int status=200) => new ApiResponse<T>{
        Status = status,
        Message = message,
        Data = data
    };
    public static ApiResponse<T> Fail(string message="Error",int status=400) => new ApiResponse<T>{
        Status = status,
        Message = message,
        Data = default!
    };
}
