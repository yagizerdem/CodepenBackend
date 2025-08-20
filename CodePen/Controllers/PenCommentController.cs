using DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Models.DTO;
using Models.Entity;
using Models.Exceptions;
using Models.ResponseTypes;
using Service;
using Service.Business;
using Utils.ExtensionMethods;

namespace CodePen.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PenCommentController : ControllerBase
    {

        private readonly PenCommentService _penCommentService;
        private readonly UserManager<ApplicationUserEntity> _userManager;
        private readonly ApplicationDbContext _db;
        public PenCommentController(
            PenCommentService penCommentService, 
            UserManager<ApplicationUserEntity> userManager,
            ApplicationDbContext db)
        {
            _penCommentService = penCommentService;
            _userManager = userManager;
            _db = db;
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

        [HttpGet("get-comments")]
        [Authorize]
        public async Task<IActionResult> GetPenComments(
            [FromQuery] string? userId,
            [FromQuery] string? content,
            [FromQuery] int penId, 
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10)
        {
            pageSize = Math.Min(pageSize, 50); // restrict to maximum 50 items per page
            var user = await GetCurrentUserAsync();

            var query = _db.PenComments.AsQueryable();
            query = query
                .ApplyExactMatch(x => x.User.Id, userId)
                .ApplyExactMatch(x => x.PenId.ToString(), penId.ToString())
                .ApplySubstringMatch(x => x.Content, content)
                .ApplySorting(desc: true, a => a.CreatedAt)
                .ApplyPagination(page, pageSize);


            List<PenCommentEntity> comments = await query.ToListAsync(); 

            return Ok(ApiResponse<List<PenCommentEntity>>.SuccessResponse(
                data: comments,
                statusCode: System.Net.HttpStatusCode.OK,
                message: "Pen comments retrieved"));
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
