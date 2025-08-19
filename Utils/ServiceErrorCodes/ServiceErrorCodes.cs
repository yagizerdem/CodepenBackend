using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Utils.ServiceErrorCodes
{
    public static class ServiceErrorCodes
    {
        public const string UserNotFound = "USER_NOT_FOUND";
        
        public const string PenNotFound = "PEN_NOT_FOUND";  

        public const string NotAllowed = "NOT_ALLOWED";

        public const string OldPenVersionNotFound = "OLD_PEN_VERSION_NOT_FOUND";

        public static HttpStatusCode MapToStatusCode(string errorCode)
        {
            return errorCode switch
            {
                UserNotFound => HttpStatusCode.NotFound,
                PenNotFound => HttpStatusCode.NotFound,
                NotAllowed => HttpStatusCode.Forbidden,
                OldPenVersionNotFound => HttpStatusCode.NotFound,
                _ => HttpStatusCode.BadRequest
            };
        }
    }
}
