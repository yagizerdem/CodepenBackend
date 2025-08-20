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

        public const string PenAlreadyLiked = "PEN_ALREADY_LIKED";

        public const string PenNotLiked = "PEN_NOT_LIKED";

        public const string PenCommentNotFound = "PEN_COMMENT_NOT_FOUND";

        public const string ProfilePictureNotFound = "PROFILE_PICTURE_NOT_FOUND";

        public const string SelfFollowRequest = "SELF_FOLLOW_REQUEST";

        public const string ActiveFollowRequest = "ACTIVE_FOLLOW_REQUEST";
        public static HttpStatusCode MapToStatusCode(string errorCode)
        {
            return errorCode switch
            {
                UserNotFound => HttpStatusCode.NotFound,
                PenNotFound => HttpStatusCode.NotFound,
                NotAllowed => HttpStatusCode.Forbidden,
                OldPenVersionNotFound => HttpStatusCode.NotFound,
                ProfilePictureNotFound => HttpStatusCode.NotFound,
                PenAlreadyLiked => HttpStatusCode.Conflict,
                PenNotLiked => HttpStatusCode.Conflict,
                PenCommentNotFound => HttpStatusCode.NotFound,
                SelfFollowRequest => HttpStatusCode.BadRequest,
                ActiveFollowRequest => HttpStatusCode.BadRequest,
                _ => HttpStatusCode.BadRequest
            };
        }
    }
}
