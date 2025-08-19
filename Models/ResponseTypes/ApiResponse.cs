using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Models.ResponseTypes
{
    public class ApiResponse<T>
    {
        public T Data { get; init; }
        public string Message { get; init; }
        public bool Success { get; init; }
        public HttpStatusCode StatusCode { get; init; }
        public List<string> Errors { get; init; }

        private ApiResponse(T data, string message, bool success, HttpStatusCode statusCode, List<string>? errors = null)
        {
            Data = data;
            Message = message;
            Success = success;
            StatusCode = statusCode;
            Errors = errors ?? new List<string>();
        }

        public static ApiResponse<T> SuccessResponse(
            T data,
            string message = "",
            HttpStatusCode statusCode = HttpStatusCode.OK) =>
            new(data, message, true, statusCode);

        public static ApiResponse<T> ErrorResponse(
            string message,
            HttpStatusCode statusCode = HttpStatusCode.BadRequest,
            List<string>? errors = null) =>
            new(default!, message, false, statusCode, errors);

        public static ApiResponse<T> Empty(
            HttpStatusCode statusCode = HttpStatusCode.NoContent,
            string message = "") =>
            new(default!, message, true, statusCode);
    }

}
