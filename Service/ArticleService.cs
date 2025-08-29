using AutoMapper;
using DataAccess;
using Microsoft.EntityFrameworkCore;
using Models.DTO;
using Models.Entity;
using Models.Exceptions;
using Service.Business;
using Utils.ServiceErrorCodes;

namespace Service
{
    public class ArticleService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly ArticleRelatedLogic _articleRelatedLogic;
        private readonly ApplicationUserRelatedLogic _userRelatedLogic;
        public ArticleService(
            ApplicationDbContext db,
            IMapper mapper,
            ArticleRelatedLogic articleRelatedLogic,
            ApplicationUserRelatedLogic userRelatedLogic)
        {
            _db = db;
            _mapper = mapper;
            _articleRelatedLogic = articleRelatedLogic;
            _userRelatedLogic = userRelatedLogic;
        }

        public async Task<ArticleEntity> CreateArticle(CreateArticleDTO dto, ApplicationUserEntity author)
        {
            ArticleEntity article = _mapper.Map<ArticleEntity>(dto);

            if (dto.CoverImage != null)
            {
                await using var memoryStream = new MemoryStream();
                await dto.CoverImage.CopyToAsync(memoryStream);
                byte[] coverImage = memoryStream.ToArray();
                article.CoverImage = coverImage;
            }

            article.Author = author;
            article.AuthorId = author.Id;

            await _db.Articles.AddAsync(article);
            await _db.SaveChangesAsync();
        
            return article;
        }

        public async Task<ArticleEntity> UpdateArticle(UpdateArticleDTO dto, ApplicationUserEntity author)
        {
            var article = await _articleRelatedLogic.EnsureArticleExistsAndActiveById(dto.Id);
            
            if (dto.Title != null) article.Title = dto.Title;
            if(dto.FullText != null) article.FullText = dto.FullText;
            if(dto.Visibility != null) article.Visibility = (Models.Enums.ArticleVisibility)dto.Visibility;
            if(dto.Abstract != null) article.Abstract = dto.Abstract;

            if (dto.CoverImage != null)
            {
                await using var memoryStream = new MemoryStream();
                await dto.CoverImage.CopyToAsync(memoryStream);
                byte[] coverImage = memoryStream.ToArray();
                article.CoverImage = coverImage;
            }
            
            _db.Articles.Update(article);
            await _db.SaveChangesAsync();
            return article;
        }

        public async Task<ArticleEntity> GetArticleById(int id , ApplicationUserEntity user)
        {
            var article = await _articleRelatedLogic.EnsureArticleExistsAndActiveById(id);

            if (article.AuthorId == user.Id) return article; // author can access his own article
            if (article.Visibility == Models.Enums.ArticleVisibility.Public) return article; // public article can be accessed by anyone
            if (article.Visibility == Models.Enums.ArticleVisibility.Private) throw new ServiceException(
                    message: "You are not authorized to view this private article.",
                    isOperational: true,
                    machineCode: ServiceErrorCodes.NotAllowed,
                    errors: new List<string> { "You are not authorized to view this private article." }
                    );

            if(article.Visibility == Models.Enums.ArticleVisibility.OnlyFollowers)
            {
                var authorFromDb = await _userRelatedLogic.EnsureUserExistAndActiveById(article.AuthorId);
                var flag = await _db.Relations.AnyAsync(r => r.FollowingId == authorFromDb.Id &&
                    r.FollowerId == user.Id && r.Status == Models.Enums.EntityStatus.Active);
                if (!flag)
                {
                    throw new ServiceException(
                        message: "You must be a follower to view this article.",
                        isOperational: true,
                        machineCode: ServiceErrorCodes.NotAllowed,
                        errors: new List<string> { "You are not authorized to view this article because you are not a follower." }
                    );
                }
            }

            return article;
        }

        public async Task<ArticleEntity> SoftDeleteArticle(int id, ApplicationUserEntity user)
        {
            var articleFromDb  = await _articleRelatedLogic.EnsureArticleExistsAndActiveById(id);
            
            if(articleFromDb.AuthorId != user.Id)
            {
                 throw new ServiceException(
                      message: "you are not authorized to delete this article",
                      isOperational: true,
                      machineCode: ServiceErrorCodes.NotAllowed,
                      errors: ["you are not authorized to delete this article"]
                      );
            }

            articleFromDb.Status = Models.Enums.EntityStatus.Deleted;
            _db.Update(articleFromDb);
            await _db.SaveChangesAsync();
            return articleFromDb;
        }

        public async Task<ArticleEntity> HardDeleteArticle(int id, ApplicationUserEntity user)
        {
            var articleFromDb = await _articleRelatedLogic.EnsureArticleExistsById(id);
            if (articleFromDb.Author.Id != user.Id)
            {
                throw new ServiceException(
                     message: "you are not authorized to delete this article",
                     isOperational: true,
                     machineCode: ServiceErrorCodes.NotAllowed,
                     errors: ["you are not authorized to delete this article"]
                     );
            }
            _db.Articles.Remove(articleFromDb);
            await _db.SaveChangesAsync();
            return articleFromDb;
        }   


        public async Task<List<ArticleEntity>> GetArticles(
            int pageNumber, 
            int pageSize, 
            string? titleFilter,
            ApplicationUserEntity user)
        {
            var query = _db.Articles
                .Include(a => a.Author)
                .Where(a => a.Status == Models.Enums.EntityStatus.Active)
                .AsQueryable();
            // Filter by title if provided
            if (!string.IsNullOrEmpty(titleFilter))
            {
                query = query.Where(a => a.Title.Contains(titleFilter));
            }
            // Apply visibility rules
            query = query.Where(a =>
                a.Visibility == Models.Enums.ArticleVisibility.Public || // Public articles are accessible to everyone
                (a.Visibility == Models.Enums.ArticleVisibility.Private && a.AuthorId == user.Id) || // Private articles are only accessible to the author
                (a.Visibility == Models.Enums.ArticleVisibility.OnlyFollowers && 
                    _db.Relations.Any(r => r.FollowingId == a.AuthorId && r.FollowerId == user.Id && r.Status == Models.Enums.EntityStatus.Active)) // OnlyFollowers articles are accessible to followers
            );
            // Pagination
            var articles = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return articles;
        }   


        public async Task<BookMark> CreateBookmark(int articleId, ApplicationUserEntity user)
        {
            var articleFromDb = await _articleRelatedLogic.EnsureArticleExistsAndActiveById(articleId);
            await _articleRelatedLogic.EnsureArticleIsAvailableForUser(articleFromDb, user);
            await _articleRelatedLogic.EnsureBookmarNotExistsAndNotActive(articleId, user.Id);
            
            var bookmark = new BookMark
            {
                ArticleId = articleId,
                UserId = user.Id,
            };
            await _db.BookMarks.AddAsync(bookmark);
            await _db.SaveChangesAsync();
            return bookmark;

        }

        public async Task<BookMark> SoftDeleteBookmark(int articleId, ApplicationUserEntity user)
        {
            var bookmarkFromDb = await _articleRelatedLogic.EnsureBookMarkExistAndActive(articleId, user.Id);

            bookmarkFromDb.Status = Models.Enums.EntityStatus.Deleted;
            _db.BookMarks.Update(bookmarkFromDb);
            await _db.SaveChangesAsync();

            return bookmarkFromDb;
        }

        public async Task<BookMark> HardDeleteBookmark(int articleId, ApplicationUserEntity user)
        {
            var bookmarkFromDb = await _articleRelatedLogic.EnsureBookMarkExist(articleId, user.Id);

            _db.BookMarks.Remove(bookmarkFromDb);
            await _db.SaveChangesAsync();

            return bookmarkFromDb;
        }

        public async Task<List<BookMark>> GetBookmarks(
            int pageNumber,
            int pageSize,
            ApplicationUserEntity user)
        {
            var query = _db.BookMarks
                .Include(bm => bm.Article)
                .ThenInclude(a => a.Author)
                .Where(bm => bm.Status == Models.Enums.EntityStatus.Active && bm.UserId == user.Id)
                .AsQueryable();
            // Pagination
            var bookmarks = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return bookmarks;
        }

    }
}
