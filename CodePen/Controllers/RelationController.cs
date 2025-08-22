using DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Entity;
using Models.Exceptions;
using Models.ResponseTypes;
using Service;

namespace CodePen.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RelationController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUserEntity> _userManager;
        private readonly RelationService _relationService;
        public RelationController(
            ApplicationDbContext db, 
            UserManager<ApplicationUserEntity> userManager,
            RelationService relationService)
        {
            _db = db;
            _userManager = userManager;
            _relationService = relationService;
        }

        [HttpPost("send-follow-request/{userId}")]
        [Authorize]

        public async Task<IActionResult> SendFollowRequest(string userId)
        {
            var user = await GetCurrentUserAsync();
            var relation = await _relationService.CreateFollowRequest(user, userId);

            return Ok(ApiResponse<FollowRequest>.SuccessResponse(
                data: relation,
                message: "follow request sended successfully",
                statusCode: System.Net.HttpStatusCode.Created));
        }

        [HttpPost("reject-follow-request/{followRequestId}")]
        [Authorize]
        public async Task<IActionResult> RejectFollowRequest(int followRequestId)
        {
            var user = await GetCurrentUserAsync();
            var relation = await _relationService.RejectFollowRequest(followRequestId, user);
            
            return Ok(ApiResponse<FollowRequest>.SuccessResponse(
                data: relation,
                message: "follow request rejected successfully",
                statusCode: System.Net.HttpStatusCode.OK));
        }

        [HttpPost("accept-follow-request/{followRequestId}")]
        [Authorize]
        public async Task<IActionResult> AcceptFollowRequest(int followRequestId)
        {
            var user = await GetCurrentUserAsync();
            var relation = await _relationService.AcceptFollowRequest(followRequestId, user);
            
            return Ok(ApiResponse<FollowRequest>.SuccessResponse(
                data: relation,
                message: "follow request accepted successfully",
                statusCode: System.Net.HttpStatusCode.OK));
        }

        [HttpPost("unfollow/{userId}")]
        [Authorize]
        public async Task<IActionResult> UnfollowUser(string userId)
        {
            var user = await GetCurrentUserAsync();
            var relation = await _relationService.SoftDeleteRelation(user, userId);

            return Ok(ApiResponse<RelationEntity>.SuccessResponse(
                data: relation,
                message: "unfollowed successfully",
                statusCode: System.Net.HttpStatusCode.OK));
        }

        [HttpPost("remove-follower/{userId}")]
        [Authorize]
        public async Task<IActionResult> RemoveFollower(string userId)
        {
            var user = await GetCurrentUserAsync();
            var follower = await _db.ApplicationUsers.FirstOrDefaultAsync(
                x => x.Id == userId
                && x.Status == Models.Enums.EntityStatus.Active) ??
                throw new AppException(
                    message:"follower not found",
                    errors:["follower not found"],
                    isOperational:true,
                    statusCode:System.Net.HttpStatusCode.NotFound);

            var relation = await _relationService.SoftDeleteRelation(follower, user.Id);
            return Ok(ApiResponse<RelationEntity>.SuccessResponse(
                data: relation,
                message: "follower removed successfully",
                statusCode: System.Net.HttpStatusCode.OK));
        }

        [HttpGet("get-followers/{userId}")]
        [Authorize] 
        public async Task<IActionResult> GetFollowers(string userId)
        {
            var user = await GetCurrentUserAsync();
            var followers = await _relationService.GetFollowers(currentUser: user, targetUserId: userId);

            return Ok(ApiResponse<List<ApplicationUserEntity>>.SuccessResponse(
                data: followers,
                message: "followers retrieved successfully",
                statusCode: System.Net.HttpStatusCode.OK));
        }


        // helpers
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
