using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.DTO;
using Models.Entity;
using Models.Exceptions;
using Models.ResponseTypes;
using Service;

namespace CodePen.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientController : ControllerBase
    {
        private readonly UserManager<ApplicationUserEntity> _userManager;
        private readonly ApplicationUserService _applicationUserService;
        public ClientController(
            UserManager<ApplicationUserEntity> userManager,
            ApplicationUserService applicationUserService)
        {
            _userManager = userManager;
            _applicationUserService = applicationUserService;
        }

        [HttpPost("upload-profile-image")]
        public async Task<IActionResult> UploadProfilePicture([FromForm] UploadFileDTO dto)
        {
            var user = await GetCurrentUserAsync();
            var userFromDb = await _applicationUserService.UploadProfilePhoto(dto, user);

            return Ok(ApiResponse<object?>.SuccessResponse(
                data: userFromDb,
                message: "profile image uploaded successfully",
                statusCode: System.Net.HttpStatusCode.OK));
        }

        [HttpPost("remove-profile-image")]
        public async Task<IActionResult> RemoveProfileImage()
        {
            var user = await GetCurrentUserAsync();
            var userFromDb = await _applicationUserService.SoftDeleteProfilePhoto(user);    

            return Ok(ApiResponse<object?>.SuccessResponse(
                data: null,
                message: "profile image deleted",
                statusCode: System.Net.HttpStatusCode.OK));
        }

        //  helpers
        public async Task<ApplicationUserEntity> GetCurrentUserAsync()
        {
            var userId = _userManager.GetUserId(User) ??
                throw new AppException(
                    message: "user not found",
                    statusCode: System.Net.HttpStatusCode.NotFound,
                    isOperational: true,
                    errors: ["user not found"]);

            var user = await _userManager.FindByIdAsync(userId) ??
                throw new AppException(
                    message: "user not found",
                    statusCode: System.Net.HttpStatusCode.NotFound,
                    isOperational: true,
                    errors: ["user not found"]);

            if (user.Status == Models.Enums.EntityStatus.Deleted)
                throw new AppException(
                    message: "user not found",
                    statusCode: System.Net.HttpStatusCode.NotFound,
                    isOperational: true,
                    errors: ["user not found"]);

            return user;
        }
    }
}
