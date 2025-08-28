using DataAccess;
using Microsoft.EntityFrameworkCore;
using Models.Entity;
using Models.Exceptions;
using Utils.ServiceErrorCodes;

namespace Service.Business
{
    public class ArticleRelatedLogic
    {
        private readonly ApplicationDbContext _db;
        public ArticleRelatedLogic(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<ArticleEntity> EnsureArticleExistsById(int id)
        {
            var articleFromDB = await _db.Articles.FirstOrDefaultAsync(a => a.Id == id) ??
                 throw new ServiceException(
                     message: "article not found",
                     isOperational: true,
                     machineCode: ServiceErrorCodes.ArticleNotFound,
                     errors: ["article not found"]
                     );

            return articleFromDB;
        }

        public async Task<ArticleEntity> EnsureArticleExistsAndActiveById(int id)
        {
            var articleFromDB = await _db.Articles.FirstOrDefaultAsync(a => a.Id == id 
            && a.Status == Models.Enums.EntityStatus.Active) ??
                 throw new ServiceException(
                     message: "article not found",
                     isOperational: true,
                     machineCode: ServiceErrorCodes.ArticleNotFound,
                     errors: ["article not found"]
                     );

            return articleFromDB;
        }

        public async Task EnsureArticleIsAvailableForUser(ArticleEntity article, ApplicationUserEntity user)
        {
            // private visibility check
            if (article.Visibility == Models.Enums.ArticleVisibility.Private)
            {
                if(article.AuthorId != user.Id) throw new ServiceException(
                    message: "You are not authorized to view this private article.",
                    isOperational: true,
                    machineCode: ServiceErrorCodes.NotAllowed,
                    errors: new List<string> { "You are not authorized to view this private article." }
                    );
            }

            // only followers visibility check
            if(article.Visibility == Models.Enums.ArticleVisibility.OnlyFollowers &&
                article.AuthorId != user.Id)
            {
                var flag = _db.Relations.Any(r => r.FollowingId == article.AuthorId &&
                    r.FollowerId == user.Id && r.Status == Models.Enums.EntityStatus.Active);
            
                if (!flag) throw new ServiceException(
                    message: "You must be a follower to view this article.",
                    isOperational: true,
                    machineCode: ServiceErrorCodes.NotAllowed,
                    errors: new List<string> { "You are not authorized to view this article because you are not a follower." }
                    );
            }

        }
 
        public async Task EnsureBookmarNotExistsAndNotActive(int articleId , string userId)
        {
            var existingBookmark = await _db.BookMarks
                .FirstOrDefaultAsync(bm => bm.ArticleId == articleId && bm.UserId == userId &&
                bm.Status == Models.Enums.EntityStatus.Active);
            if (existingBookmark != null)
            {
                throw new ServiceException(
                    message: "Article already bookmarked",
                    isOperational: true,
                    machineCode: ServiceErrorCodes.BookMarkAlreadyExists,
                    errors: new List<string> { "You have already bookmarked this article." }
                );
            }
        }

        public async Task<BookMark> EnsureBookMarkExistAndActive(int aritcleId, string userId)
        {
            var bookmarkFromDb = await _db.BookMarks.FirstOrDefaultAsync(
                x => x.ArticleId == aritcleId && x.UserId == userId &&
                x.Status == Models.Enums.EntityStatus.Active) ??
                throw new ServiceException(
                   message:"bookmark not exists",
                   errors: ["bookmark not exists"],
                   isOperational:true,
                   machineCode:ServiceErrorCodes.BookmarkNotExists);

            return bookmarkFromDb;
        }

        public async Task<BookMark> EnsureBookMarkExist(int aritcleId, string userId)
        {
            var bookmarkFromDb = await _db.BookMarks.FirstOrDefaultAsync(
                x => x.ArticleId == aritcleId && x.UserId == userId ) ??
                throw new ServiceException(
                   message: "bookmark not exists",
                   errors: ["bookmark not exists"],
                   isOperational: true,
                   machineCode: ServiceErrorCodes.BookmarkNotExists);

            return bookmarkFromDb;
        }

    }
}
