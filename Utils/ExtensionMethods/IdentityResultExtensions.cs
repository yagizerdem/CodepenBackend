using Microsoft.AspNetCore.Identity;
using Models.Exceptions;
using System.Net;

namespace Utils.ExtensionMethods
{
    public static class IdentityResultExtensions
    {
        public static void ThrowIfFailed(this IdentityResult result, string contextMessage = "Identity operation failed")
        {
            if (result.Succeeded) return;

            var errors = result.Errors.ToList();
            var statusCode = MapStatusCode(errors);

            throw new AppException(
                message: contextMessage,
                statusCode: statusCode,
                errors: errors.Select(e => e.Description).ToList(),
                isOperational: true
            );
        }


        private static HttpStatusCode MapStatusCode(IEnumerable<IdentityError> errors)
        {
            foreach (var error in errors)
            {
                switch (error.Code)
                {
                    // User issues
                    case "DuplicateUserName":
                    case "DuplicateEmail":
                        return HttpStatusCode.Conflict; // 409

                    case "InvalidUserName":
                    case "InvalidEmail":
                        return HttpStatusCode.BadRequest; // 400

                    // Password issues
                    case "PasswordTooShort":
                    case "PasswordRequiresNonAlphanumeric":
                    case "PasswordRequiresDigit":
                    case "PasswordRequiresUpper":
                    case "PasswordRequiresLower":
                        return HttpStatusCode.BadRequest; // 400

                    // Role issues
                    case "UserAlreadyInRole":
                        return HttpStatusCode.Conflict; // 409

                    case "UserNotInRole":
                        return HttpStatusCode.BadRequest; // 400

                    case "RoleNotFound":
                        return HttpStatusCode.NotFound; // 404

                    // Lockout / security issues
                    case "UserLockedOut":
                        return HttpStatusCode.Forbidden; // 403

                    case "InvalidToken":
                        return HttpStatusCode.Unauthorized; // 401

                    default:
                        // Fallback for unknown codes
                        return HttpStatusCode.BadRequest;
                }
            }

            return HttpStatusCode.BadRequest; // default fallback
        }
    }

}
