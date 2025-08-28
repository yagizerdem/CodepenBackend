using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Identity.Client;
using Models.DTO;
using Models.Entity;
using Models.Exceptions;
using Models.ResponseTypes;
using Service;

namespace CodePen.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArticleController  : ControllerBase
    {

        private readonly ArticleService _articleService;
        private readonly UserManager<ApplicationUserEntity> _userManager;
        public ArticleController(
            ArticleService articleService, 
            UserManager<ApplicationUserEntity> userManager)
        {
            _articleService = articleService;
            _userManager = userManager;
        }

        [HttpPost("create")]
        [Authorize]
        public async Task<IActionResult> CreateArticle([FromForm] CreateArticleDTO dto)
        {
            var user = await GetCurrentUserAsync();
            var artilceFromDb =  await _articleService.CreateArticle(dto, user);

            return Ok(ApiResponse<ArticleEntity>.SuccessResponse(
                data: artilceFromDb,
                message:"article created successfully",
                statusCode:System.Net.HttpStatusCode.Created));
        }

        [HttpPost("update")]
        [Authorize] 
        public async Task<IActionResult> UpdateArticle([FromForm] UpdateArticleDTO dto)
        {
            var user = await GetCurrentUserAsync();
            var artilceFromDb = await _articleService.UpdateArticle(dto, user);
            return Ok(ApiResponse<ArticleEntity>.SuccessResponse(
                data: artilceFromDb,
                message: "article updated successfully",
                statusCode: System.Net.HttpStatusCode.OK));
        }

        [HttpPost("remove/{articleId}")]
        [Authorize]
        public async Task<IActionResult> SoftDelete(int articleId)
        {
            var user = await GetCurrentUserAsync();
            var artilceFromDb = await _articleService.SoftDeleteArticle(articleId, user);
          
            return Ok(ApiResponse<ArticleEntity>.SuccessResponse(
                data: artilceFromDb,
                message: "article removed successfully",
                statusCode: System.Net.HttpStatusCode.OK));

        }

        [HttpGet("get-by-id/{articleId}")]
        [Authorize]
        public async Task<IActionResult> GetById(int articleId)
        {
            var user = await GetCurrentUserAsync(); 
            var artilceFromDb = await _articleService.GetArticleById(articleId, user);
            return Ok(ApiResponse<ArticleEntity>.SuccessResponse(
                data: artilceFromDb,
                message: "article fetched successfully",
                statusCode: System.Net.HttpStatusCode.OK));
        }

        [HttpGet("get-articles")]
        [Authorize]
        public async Task<IActionResult> GetArticles(
            [FromQuery] string? Title,
            [FromQuery] int pageNumber = 1, 
            [FromQuery] int pageSize = 10)
        {
            pageSize = pageSize < 1 ? 1 : pageSize;
            pageSize = Math.Min(pageSize, 10); // restirict to maximum 10 articles in one go 

            var user = await GetCurrentUserAsync();
            var articles = await _articleService.GetArticles(pageNumber, pageSize, Title, user);
            return Ok(ApiResponse<List<ArticleEntity>>.SuccessResponse(
                data: articles,
                message: "articles fetched successfully",
                statusCode: System.Net.HttpStatusCode.OK));
        }

        [HttpPost("create-bookmark/{articleId}")]
        [Authorize]
        public async Task<IActionResult> BookMarkArticle(int articleId)
        {
            var user = await GetCurrentUserAsync();
            var bookmark = await _articleService.CreateBookmark(articleId, user);
            
            return Ok(ApiResponse<BookMark>.SuccessResponse(
                data: bookmark,
                message: "article bookmarked successfully",
                statusCode: System.Net.HttpStatusCode.OK));

        }

        [HttpPost("remove-bookmark/{articleId}")]
        [Authorize]

        public async Task<IActionResult> SoftRemoveBookmark(int articleId)
        {
            var user = await GetCurrentUserAsync();
            var payload = await _articleService.SoftDeleteBookmark(articleId, user);

            return Ok(ApiResponse<BookMark>.SuccessResponse(
                data: payload,
                message: "bookmark removed successfully",
                statusCode: System.Net.HttpStatusCode.OK));
        }

        [HttpGet("get-bookmarks")]
        [Authorize]

        public async Task<IActionResult> GetBookMarks(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var user = await GetCurrentUserAsync();
            pageSize = Math.Min(Math.Max(pageSize, 1), 10); // restrict between 1 and 10
            var bookMarks = await _articleService.GetBookmarks(page, pageSize, user);
            
            return Ok(ApiResponse<List<BookMark>>.SuccessResponse(
                data: bookMarks,
                message: "bookmarks fetched successfully",
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
