using Sang.Service.Common.UtilityManager;
using System.Net;

namespace Sang.Service.Common.ApiResponse
{
    public sealed class ApiResponse
    {

        public ResponseStatus Status { get; set; }
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public string? Result { get; set; }              

        public static ApiResponse Success(object resultData)
        {
            return new ApiResponse
            {
                Status = ResponseStatus.Success,
                StatusCode = (int)HttpStatusCode.OK,
                Message = "Records successfully retrieved.",
                Result = resultData == null ? null : Utils.SerializeData(resultData)
            };
        }
        public static ApiResponse Delete(string? message = null)
        {
            return new ApiResponse
            {
                Status = ResponseStatus.Success,
                StatusCode = (int)HttpStatusCode.OK,
                Message = message ?? "Record(s) Deleted"
            };
        }

        public static ApiResponse Created(string message, object? resultData = null)
        {
            return new ApiResponse
            {
                Status = ResponseStatus.Success,
                StatusCode = (int)HttpStatusCode.Created,
                Message = message,
                Result = resultData != null ? Utils.SerializeData(resultData) : null
            };
        }
        public static ApiResponse NotFound(string? message = null)
        {
            return new ApiResponse
            {
                Status = ResponseStatus.Failure,
                StatusCode = (int)HttpStatusCode.NotFound,
                Message = message ?? "Record(s) Not Found"
            };
        }
        public static ApiResponse BadRequest(string message)
        {
            return new ApiResponse
            {
                Status = ResponseStatus.Failure,
                StatusCode = (int)HttpStatusCode.BadRequest,
                Message = message 
            };
        }
        public static ApiResponse Failure(ErrorType errorType, string? message)
        {
            return new ApiResponse
            {
                Status = ResponseStatus.Failure,
                StatusCode = GetErrorType(errorType),
                Message = message
            };
        }
        private static int GetErrorType(ErrorType errorType) => errorType switch
        {
            ErrorType.CONFLICT => (int)HttpStatusCode.Conflict,
            ErrorType.INVALID => (int)HttpStatusCode.BadRequest,
            _ => (int)HttpStatusCode.InternalServerError
        };        
    }

    public sealed class ApiResponse<T>
    {
        public ResponseStatus Status { get; set; }
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public IEnumerable<T>? Result { get; set; }

        public static ApiResponse<T> Success<T>(IEnumerable<T> resultData)
        {
            return new ApiResponse<T>
            {
                Status = ResponseStatus.Success,
                StatusCode = (int)HttpStatusCode.OK,
                Result = resultData
            };
        }
        public static ApiResponse<T> Created<T>(IEnumerable<T> resultData)
        {
            return new ApiResponse<T>
            {
                Status = ResponseStatus.Success,
                StatusCode = (int)HttpStatusCode.Created,
                Result = resultData
            };
        }        
    }
}
