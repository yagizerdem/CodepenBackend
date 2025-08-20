using DataAccess;
using Microsoft.EntityFrameworkCore;
using Models.Entity;
using Models.Enums;
using Models.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.ServiceErrorCodes;

namespace Service.Business
{
    public class PenCommentRelatedLogic
    {
        private readonly ApplicationDbContext _db;
        public PenCommentRelatedLogic(
            ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<PenCommentEntity> EnsureExistById(int penCommentId)
        {
            var penCommentFromDb = await _db.PenComments.FirstOrDefaultAsync(p => p.Id == penCommentId) ??
                throw new ServiceException(
                    message:"pen comment not found",
                    isOperational:true,
                    errors: ["pen comment not found"],
                    machineCode: ServiceErrorCodes.PenCommentNotFound
                    );

            return penCommentFromDb;
        }

        public async Task<PenCommentEntity> EnsureExistByIdAndActive(int penCommentId)
        {
            var penCommentFromDb = await EnsureExistById(penCommentId);
            if (penCommentFromDb.Status == Models.Enums.EntityStatus.Deleted)
                throw new ServiceException(
                    message: "pen comment not found",
                    isOperational: true,
                    errors: ["pen comment not found"],
                    machineCode: ServiceErrorCodes.PenCommentNotFound
                    );

            return penCommentFromDb;

        }

        public async Task<PenCommentEntity> EnsureOwnershipOfPenComment(int penCommentId, ApplicationUserEntity user)
        {
            var penCommentFromDb = await EnsureExistByIdAndActive(penCommentId);
            if(penCommentFromDb.UserId != user.Id)
                throw new ServiceException(
                    message: "you are not the owner of this pen comment",
                    isOperational: true,
                    errors: ["you are not the owner of this pen comment"],
                    machineCode: ServiceErrorCodes.NotAllowed
                    );

            return penCommentFromDb;
        }

    }
}
