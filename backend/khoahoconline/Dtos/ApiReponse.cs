namespace khoahoconline.Dtos
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }

        public ApiResponse() { }
        public ApiResponse(bool success, string? message, T? data)
        {
            Success = success;
            Message = message;
            Data = data;
        }

        public static ApiResponse<T> SuccessResponse(T? data)
        {
            return new ApiResponse<T>(true, "Success", data);
        }

        public static ApiResponse<T> SuccessResponse(T? data, string message)
        {
            return new ApiResponse<T>(true, message, data);
        }

        public static ApiResponse<T> ErrorResponse(string message)
        {
            return new ApiResponse<T>(false, message, default);
        }

        public static ApiResponse<T> FailureResponse(string message)
        {
            return new ApiResponse<T>(false, message, default);
        }
    }
}