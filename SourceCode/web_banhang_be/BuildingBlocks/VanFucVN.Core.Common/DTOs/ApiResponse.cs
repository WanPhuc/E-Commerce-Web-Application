namespace VanFucVN.Core.Common.DTOs;

using VanFucVN.Core.Common.Constants;

public class ApiResponse
{
    public int Status { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? Code { get; set; }

    public static ApiResponse Success(string message = "Success", int status = 200, string? code = null) => new ApiResponse
    {
        Status = status,
        Message = message,
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
}
