using System.Net;

namespace ProductManagement.API.Models
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public int StatusCode { get; set; }

        public ApiResponse(bool success, string message, T data, HttpStatusCode statusCode)
        {
            Success = success;
            Message = message;
            Data = data;
            StatusCode = (int)statusCode;
        }

        public static ApiResponse<T> SuccessResponse(T data, string message = "Operation successful")
        {
            return new ApiResponse<T>(true, message, data, HttpStatusCode.OK);
        }

        public static ApiResponse<T> CreatedResponse(T data, string message = "Resource created successfully")
        {
            return new ApiResponse<T>(true, message, data, HttpStatusCode.Created);
        }

        public static ApiResponse<T> NoContentResponse(string message = "Operation completed")
        {
            return new ApiResponse<T>(true, message, default, HttpStatusCode.NoContent);
        }

        public static ApiResponse<T> NotFoundResponse(string message = "Resource not found")
        {
            return new ApiResponse<T>(false, message, default, HttpStatusCode.NotFound);
        }

        public static ApiResponse<T> BadRequestResponse(string message = "Invalid request")
        {
            return new ApiResponse<T>(false, message, default, HttpStatusCode.BadRequest);
        }

        public static ApiResponse<T> UnauthorizedResponse(string message = "Unauthorized access")
        {
            return new ApiResponse<T>(false, message, default, HttpStatusCode.Unauthorized);
        }

        public static ApiResponse<T> ServerErrorResponse(string message = "Internal server error")
        {
            return new ApiResponse<T>(false, message, default, HttpStatusCode.InternalServerError);
        }
    }
} 