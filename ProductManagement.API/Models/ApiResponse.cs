using System.Net;

namespace ProductManagement.API.Models
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public int StatusCode { get; set; }

        private ApiResponse(bool success, string message, T data, HttpStatusCode statusCode)
        {
            Success = success;
            Message = message;
            Data = data;
            StatusCode = (int)statusCode;
        }

        #region Success Responses

        public static ApiResponse<T> SuccessResponse(T data, string message = "Operation successful")
            => new(true, message, data, HttpStatusCode.OK);

        public static ApiResponse<T> CreatedResponse(T data, string message = "Resource created successfully")
            => new(true, message, data, HttpStatusCode.Created);

        public static ApiResponse<T> NoContentResponse(string message = "Operation completed")
            => new(true, message, default, HttpStatusCode.NoContent);

        public static ApiResponse<T> AcceptedResponse(T data, string message = "Request accepted")
            => new(true, message, data, HttpStatusCode.Accepted);

        #endregion

        #region Error Responses

        public static ApiResponse<T> NotFoundResponse(string message = "Resource not found")
            => new(false, message, default, HttpStatusCode.NotFound);

        public static ApiResponse<T> BadRequestResponse(string message = "Invalid request")
            => new(false, message, default, HttpStatusCode.BadRequest);

        public static ApiResponse<T> UnauthorizedResponse(string message = "Unauthorized access")
            => new(false, message, default, HttpStatusCode.Unauthorized);

        public static ApiResponse<T> ForbiddenResponse(string message = "Access forbidden")
            => new(false, message, default, HttpStatusCode.Forbidden);

        public static ApiResponse<T> ConflictResponse(string message = "Conflict with current state")
            => new(false, message, default, HttpStatusCode.Conflict);

        public static ApiResponse<T> ServerErrorResponse(string message = "Internal server error")
            => new(false, message, default, HttpStatusCode.InternalServerError);

        public static ApiResponse<T> ServiceUnavailableResponse(string message = "Service unavailable")
            => new(false, message, default, HttpStatusCode.ServiceUnavailable);

        #endregion
    }
}