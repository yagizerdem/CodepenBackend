using DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models.Entity;
using Models.Exceptions;
using Models.ResponseTypes;
using Service;

namespace CodePen.Controllers
{
    [Route("api/[controller]")]
    public class PrivateChatController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUserEntity> _userManager;
        private readonly PrivateChatService _privateChatService;
        public PrivateChatController(
            UserManager<ApplicationUserEntity> userManager,
             ApplicationDbContext db,
             PrivateChatService privateChatService)
        {
            _db = db;
            _privateChatService = privateChatService;
            _userManager = userManager;
        }

        [HttpGet("my-messages")]
        [Authorize] 
        public async Task<IActionResult> GetPrivateMessages(
             [FromQuery] string targetUserId,
            [FromQuery] int offset = 0,
            [FromQuery] int limit = 100)
        {
            limit = Math.Min(Math.Max(1, limit), 100); // ensure limit clapm 1 - 100
            var user = await GetCurrentUserAsync();

            var payload = await _privateChatService.GetPrivateChatMessages(user, targetUserId, offset, limit);
            return Ok(ApiResponse<List<PrivateChatMessageEntity>>.SuccessResponse(
                data:payload,
                message:"messages fetched successfully",
                statusCode:System.Net.HttpStatusCode.OK));
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
