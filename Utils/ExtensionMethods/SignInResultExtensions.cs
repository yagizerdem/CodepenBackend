using Microsoft.AspNetCore.Identity;
using Models.Exceptions;
using System.Net;

namespace Utils.ExtensionMethods
{
    public static class SignInResultExtensions
    {
        public static void ThrowIfFailed(this SignInResult result, string contextMessage = "Sign-in failed")
        {
            if (result.Succeeded) return;

            var errors = new List<string>();
            HttpStatusCode statusCode;

            if (result.IsLockedOut)
            {
                statusCode = HttpStatusCode.Forbidden; // 403
                errors.Add("User is locked out.");
            }
            else if (result.IsNotAllowed)
            {
                statusCode = HttpStatusCode.Forbidden; // 403
                errors.Add("User is not allowed to sign in (e.g. email not confirmed).");
            }
            else if (result.RequiresTwoFactor)
            {
                statusCode = HttpStatusCode.Unauthorized; // 401
                errors.Add("Two-factor authentication is required.");
            }
            else
            {
                statusCode = HttpStatusCode.Unauthorized; // 401
                errors.Add("Invalid username or password.");
            }

            throw new AppException(
                message: contextMessage,
                statusCode: statusCode,
                errors: errors,
                isOperational: true
            );
        }
    }
}
