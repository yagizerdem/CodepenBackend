using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Models.Exceptions
{
    public class AppException : Exception
    {
        public IReadOnlyList<string> Errors { get; }
        public HttpStatusCode StatusCode { get; }
        public bool IsOperational { get; }

        public AppException(
            string message,
            HttpStatusCode statusCode,
            bool isOperational = true,
            IEnumerable<string>? errors = null,
            Exception? innerException = null)
            : base(message, innerException)
        {
            StatusCode = statusCode;
            IsOperational = isOperational;
            Errors = errors?.ToList() ?? new List<string>();
        }


    }
}

