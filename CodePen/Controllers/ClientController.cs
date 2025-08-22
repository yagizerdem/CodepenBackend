using DataAccess;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Models.DTO;
using Models.Entity;
using Models.Exceptions;
using Models.ResponseTypes;
using Service;
using Utils.ExtensionMethods;

namespace CodePen.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientController : ControllerBase
    {
        private readonly UserManager<ApplicationUserEntity> _userManager;
        private readonly ApplicationUserService _applicationUserService;
        private readonly ApplicationDbContext _db;
        private int _maxPageSize = 100;
        public ClientController(
            UserManager<ApplicationUserEntity> userManager,
            ApplicationUserService applicationUserService,
            ApplicationDbContext db)
        {
            _userManager = userManager;
            _applicationUserService = applicationUserService;
            _db = db;
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


        [HttpGet("get-users")]
        public async Task<IActionResult> GetUsers(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? userName = null,
            [FromQuery] string? fullName = null)
        {
            page = page < 1 ? 1 : page;
            pageSize = Math.Min(pageSize, _maxPageSize); // limit page size to 100

            var user = await GetCurrentUserAsync();
            var query = _db.Users.AsQueryable();


            query = query
                .ApplySubstringMatch(x => x.UserName, userName)
                .ApplySubstringMatch(x => x.FullName, fullName)
                .ApplyPagination(page, pageSize)
                .ApplySorting(desc: true, keySelector: x => x.CreatedAt);

            List<ApplicationUserEntity> users = await query.ToListAsync();

            return Ok(ApiResponse<List<ApplicationUserEntity>>.SuccessResponse(
                data: users,
                message: "users fetched successfully",
                statusCode: System.Net.HttpStatusCode.OK));
        }

        [HttpGet("get-me")]
        public async Task<IActionResult> GetMe()
        {
            var user = await GetCurrentUserAsync();

            return Ok(ApiResponse<ApplicationUserEntity>.SuccessResponse(
                data: user,
                message: "user fetched successfully",
                statusCode: System.Net.HttpStatusCode.OK));
        }

        [HttpGet("get-profile-image/{userId}")]
        public async Task<IActionResult> GetProfileImages(string userId)
        {
            var user = await GetCurrentUserAsync();
            MediaWrapper? profileImageWrapper = await _applicationUserService.GetProfileImage(userId, user);

            if(profileImageWrapper ==  null)
                throw new AppException(
                    message: "profile image not found",
                    statusCode: System.Net.HttpStatusCode.NotFound,
                    isOperational: true,
                    errors: ["profile image not found"]);

            return File(
                profileImageWrapper?.Data ?? Array.Empty<byte>(),
                profileImageWrapper?.MimeType ?? "application/octet-stream",
                profileImageWrapper?.FileName ?? "profile-image.png");

        }

        [HttpGet("is-loggedin")]
        public IActionResult IsLoggedIn()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                throw new AppException(
                    message: "user is not logged in",
                    statusCode: System.Net.HttpStatusCode.Unauthorized,
                    isOperational: true,
                    errors: ["user is not logged in"]);
            }

            return Ok(ApiResponse<bool>.SuccessResponse(
                data: true,
                message: "user is logged in",
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
