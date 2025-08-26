using DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public class PenController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUserEntity> _userManager;
        private readonly PenService _penService;
        private int _maxPageSize = 30;  
        public PenController(
            ApplicationDbContext db,
            UserManager<ApplicationUserEntity> userManager,
            PenService penService)
        {
            _db = db;
            _userManager = userManager;
            _penService = penService;
        }

        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> CreatePen([FromBody] CreatePenDTO dto)
        {
            var user = await GetCurrentUserAsync();
            var response = await _penService.CreatePen(dto, user);

            return Ok(ApiResponse<PenEntity>.SuccessResponse(
                data: response,
                message: "pen created successfully",
                statusCode: System.Net.HttpStatusCode.Created));
        }

        [HttpPost("remove/{penId}")]
        [Authorize]
        public async Task<IActionResult> RemovePen(int penId)
        {
            var user = await GetCurrentUserAsync();
            var response = await _penService.SoftDelete(penId, user);

            return Ok(ApiResponse<PenEntity>.SuccessResponse(
                data: response,
                message: "pen removed successfully",
                statusCode: System.Net.HttpStatusCode.OK));
        }

        [HttpPost("update/{penId}")]
        [Authorize]
        public async Task<IActionResult> UpdatePen(int penId, [FromBody] UpdatePenDTO dto)
        {
            var user = await GetCurrentUserAsync();
            var penFromDb = await _penService.UpdatePen(penId, dto, user);

            return Ok(ApiResponse<PenEntity>.SuccessResponse(
                data: penFromDb,
                message: "pen updated successfully",
                statusCode: System.Net.HttpStatusCode.OK));
        }

        [HttpPost("migrate-version/{penId}")]
        [Authorize]
        public async Task<IActionResult> MigrateNewVersion(int penId)
        {
            var user = await GetCurrentUserAsync();
            var oldVersionFromDb = await _penService.MigrateNewVersion(penId, user);

            return Ok(ApiResponse<OldPenVersionsEntity>.SuccessResponse(
                data: oldVersionFromDb,
                message: "new version created",
                statusCode: System.Net.HttpStatusCode.OK));
        }

        [HttpPost("remove-oldversion/{oldVersionId}")]
        [Authorize] 

        public async Task<IActionResult> RemoveOldVersion(int oldVersionId)
        {
            var user = await GetCurrentUserAsync();
            var response = await _penService.SoftDeleteOldVersion(oldVersionId, user);
         
            return Ok(ApiResponse<OldPenVersionsEntity>.SuccessResponse(
                data: response,
                message: "old version removed successfully",
                statusCode: System.Net.HttpStatusCode.OK));
        }

        [HttpGet("get-pens")]
        [Authorize]
        public async Task<IActionResult> GetPens([FromQuery] string? Title,
                [FromQuery] string? Description,
                [FromQuery] string? AuthorUserName,
                [FromQuery] int page = 1,
                [FromQuery] int pageSize = 10)
        {
            pageSize = Math.Min(pageSize, _maxPageSize);
            page = Math.Max(1, page);
            var query = _db.Pens.AsQueryable();

            query = query
               .Include(x => x.Author)
               .ApplySubstringMatch(a => a.Title, Title!)
               .ApplySubstringMatch(a => a.Description!, Description!)
               .ApplySubstringMatch(a => a.Author.UserName!, AuthorUserName!)
               .Where(x => x.Status == Models.Enums.EntityStatus.Active);



            var totalCount = await query.CountAsync();

            query = query
                .ApplySorting(desc: true, a => a.CreatedAt)
                .ApplyPagination(page, pageSize);

            var pensFromDb = await query.ToListAsync();

            var response = new GetPensRespnose()
            {
                TotalHits = totalCount,
                Pens = pensFromDb
            };

            return Ok(ApiResponse<GetPensRespnose>.SuccessResponse(
                data: response,
                statusCode:System.Net.HttpStatusCode.OK,
                message:"pens fetches successfully"));

        }

        [HttpPost("like-pen/{penId}")]
        [Authorize]
        public async Task<IActionResult> LikePen(int penId)
        {
            var user = await GetCurrentUserAsync();
            var response = await _penService.LikePen(penId, user);
         
            return Ok(ApiResponse<PenLikeEntity>.SuccessResponse(
                data: response,
                message: "pen liked successfully",
                statusCode: System.Net.HttpStatusCode.OK));
        }

        [HttpPost("unlike-pen/{penId}")]
        [Authorize]
        public async Task<IActionResult> UnlikePen(int penId)
        {
            var user = await GetCurrentUserAsync();
            var response = await _penService.UnlikePen(penId, user);
         
            return Ok(ApiResponse<PenLikeEntity>.SuccessResponse(
                data: response,
                message: "pen unliked successfully",
                statusCode: System.Net.HttpStatusCode.OK));
        }

        [HttpGet("get-old-versions/{penId}")]
        [Authorize]
        public async Task<IActionResult> GetOldVersions(
                int penId,
                [FromQuery] int Version,
                [FromQuery] int page = 1,
                [FromQuery] int pageSize = 10)
        {
            pageSize = Math.Min(pageSize, _maxPageSize);
            page = Math.Max(1, page);
            var query = _db.OldPenVersions.AsQueryable();

            query = query
               .ApplyExactMatch(x => (x.Pen != null ? x.Pen.Id.ToString() : ""), penId.ToString());

            if(Version > 0)
            {

                query = query
                    .ApplyExactMatch(x => x.Version.ToString(), Version.ToString());
            };
               
             query = query.ApplySorting(desc: true, a => a.CreatedAt)
               .ApplyPagination(page, pageSize);
            
            var oldVersionsFromDb = await query.ToListAsync();

            return Ok(ApiResponse<List<OldPenVersionsEntity>>.SuccessResponse(
                data: oldVersionsFromDb,
                statusCode: System.Net.HttpStatusCode.OK,
                message: "old versions fetched successfully"));
        }



        [HttpGet("get-old-versionnames/{penId}")]
        [Authorize]
        public async Task<IActionResult> GetOldVersionNames(int penId)
        {
            var oldVersionsFromDb = await _db.OldPenVersions
                .Where(p => p.PenId == penId && p.Status == Models.Enums.EntityStatus.Active)
                .Select(x => new OldVersionNameDTO
                {
                    Id = x.Id,
                    Version = x.Version,
                    AuthorId = x.Pen != null ? x.Pen.AuthorId : null,
                })
                .ToListAsync();

            if (oldVersionsFromDb == null || oldVersionsFromDb.Count == 0)
            {
                throw new AppException(
                    isOperational: true,
                    message: $"pen with id {penId} not found",
                    statusCode: System.Net.HttpStatusCode.NotFound);
            }

            return Ok(ApiResponse<List<OldVersionNameDTO>>.SuccessResponse(
                data: oldVersionsFromDb,
                statusCode: System.Net.HttpStatusCode.OK,
                message: "old versions fetched successfully"));
        }



        [HttpGet("get-pen-byid/{id}")]
        [Authorize]
        public async Task<IActionResult> GetPenById(int id)
        {

            var penFromDb = await _db.Pens
                .Include(p => p.Author)
                .FirstOrDefaultAsync(p => p.Id == id && p.Status == Models.Enums.EntityStatus.Active) ??
                throw new AppException(
                    message: $"pen with id {id} not found",
                    statusCode: System.Net.HttpStatusCode.NotFound);

            return Ok(ApiResponse<PenEntity>.SuccessResponse(
                data: penFromDb,
                message: "pen fetched successfully",
                statusCode: System.Net.HttpStatusCode.OK));
        }


        [HttpGet("get-author-bypenid/{penId}")]
        [Authorize]
        public async Task<IActionResult> GetuthorByPenId(int penId)
        {
            var penFromDb = await _db.Pens.Include(p => p.Author)
                .FirstOrDefaultAsync(p => p.Id == penId) ??
                 throw new AppException(
                    message: $"pen with id {penId} not found",
                    statusCode: System.Net.HttpStatusCode.NotFound);

            ApplicationUserEntity author = penFromDb.Author ??
                throw new AppException(
                    message: "author not found",
                    statusCode: System.Net.HttpStatusCode.NotFound);

            return Ok(ApiResponse<ApplicationUserEntity>.SuccessResponse(
                data:author,
                message:"author fetched successfully",
                statusCode:System.Net.HttpStatusCode.OK));
        }

        [HttpGet("get-total-likes-ofpen/{penId}")]
        [Authorize]

        public async Task<IActionResult> GetTotalLikesOfPen(int penId)
        {
            var totalLikes = await _db.PenLikes
                .Where(pl => pl.PenId == penId && pl.Status == Models.Enums.EntityStatus.Active)
                .CountAsync();
            return Ok(ApiResponse<int>.SuccessResponse(
                data: totalLikes,
                message: "total likes fetched successfully",
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
            
            if(user.Status == Models.Enums.EntityStatus.Deleted)
                throw new AppException(
                    message: "user not found",
                    statusCode: System.Net.HttpStatusCode.NotFound,
                    isOperational: true,
                    errors: ["user not found"]);

            return user;
        }

    }
}
