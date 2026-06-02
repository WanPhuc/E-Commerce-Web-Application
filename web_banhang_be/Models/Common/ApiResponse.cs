namespace WebBanHang.Models.Common;

// 1. Lớp thường để xử lý các phản hồi không cần Data (như lỗi 403)
public class ApiResponse
{
    public int Status { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Code { get; set; }
    public static ApiResponse Success(string message = "Success", int status = 200, string? code = null) => new ApiResponse
    {
        Status = status,
        Message = message
        ,
        Code = code ?? (status == 201 ? SuccessCodes.Common.Created : SuccessCodes.Common.Ok)
    };

    public static ApiResponse Fail(string message = "Error", int status = 400) => new ApiResponse
    {
        Status = status,
        Message = message
    };

    public static ApiResponse Fail(string message, int status, string code) => new ApiResponse
    {
        Status = status,
        Message = message,
        Code = code
    };
}

// 2. Lớp Generic kế thừa từ lớp trên để dùng cho các phản hồi có Data
public class ApiResponse<T> : ApiResponse
{
    public T? Data { get; set; } = default!;

    public static ApiResponse<T> Success(T? data, string message = "Success", int status = 200, string? code = null) => new ApiResponse<T>
    {
        Status = status,
        Message = message,
        Code = code ?? (status == 201 ? SuccessCodes.Common.Created : SuccessCodes.Common.Ok),
        Data = data
    };
    public static new ApiResponse<T> Fail(string message = "Error", int status = 400) => new ApiResponse<T>
    {
        Status = status,
        Message = message,
        Data = default!
    };

    public static new ApiResponse<T> Fail(string message, int status, string code) => new ApiResponse<T>
    {
        Status = status,
        Message = message,
        Code = code,
        Data = default!
    };

    // Bạn có thể override hoặc tạo thêm Fail có T nếu cần, 
    // nhưng thường chỉ cần dùng ApiResponse.Fail là đủ.
}