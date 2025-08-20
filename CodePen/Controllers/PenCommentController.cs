using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Models.DTO;
using Models.Entity;
using Models.Exceptions;
using Models.ResponseTypes;
using Service;
using Service.Business;

namespace CodePen.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PenCommentController : ControllerBase
    {

        private readonly PenCommentService _penCommentService;
        private readonly UserManager<ApplicationUserEntity> _userManager;   
        public PenCommentController(
            PenCommentService penCommentService, 
            UserManager<ApplicationUserEntity> userManager)
        {
            _penCommentService = penCommentService;
            _userManager = userManager;
        }

        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> CreatePenComment([FromBody] CreatePenCommentDTO dto)
        {
            var user = await GetCurrentUserAsync();
            var penCommentFromDb = await _penCommentService.CreatePenComment(dto, user);

            return Ok(ApiResponse<PenCommentEntity>.SuccessResponse(
                data: penCommentFromDb,
                statusCode: System.Net.HttpStatusCode.Created,
                message: "Pen comment created"));
        }

        [HttpPost("remove/{penCommentId}")]
        [Authorize]
        public async Task<IActionResult> SoftDeletePenComment(int penCommentId)
        {
            var user = await GetCurrentUserAsync();
            var penCommentFromDb = await _penCommentService.SoftDeleteComment(penCommentId, user);

            return Ok(ApiResponse<PenCommentEntity>.SuccessResponse(
                data: penCommentFromDb,
                statusCode: System.Net.HttpStatusCode.OK,
                message: "Pen comment soft deleted"));
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
