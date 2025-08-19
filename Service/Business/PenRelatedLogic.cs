using DataAccess;
using Microsoft.EntityFrameworkCore;
using Models.Entity;
using Models.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.ServiceErrorCodes;

namespace Service.Business
{
    public class PenRelatedLogic
    {
        private readonly ApplicationDbContext _db;
        public PenRelatedLogic(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<PenEntity> EnsureExistById(int penId)
        {
            var penFromDb = await _db.Pens.FirstOrDefaultAsync(p => p.Id == penId) ??
                throw new ServiceException(
                    message: $"Not exists",
                    errors: [$"Pen with id {penId} not exists"],
                    isOperational: true,
                    machineCode:ServiceErrorCodes.PenNotFound);

            return penFromDb;
        }

        public async Task<PenEntity> EnsureExistByIdAndActive(int penId)
        {
            var penFromDb = await _db.Pens.FirstOrDefaultAsync(p => p.Id == penId) ??
                throw new ServiceException(
                    message: $"Not exists",
                    errors: [$"Pen with id {penId} not exists"],
                    isOperational: true,
                    machineCode: ServiceErrorCodes.PenNotFound);

            if(penFromDb.Status == Models.Enums.EntityStatus.Deleted)
                throw new ServiceException(
                    message: $"Not exists",
                    errors: [$"Pen with id {penId} not exists"],
                    isOperational: true,
                    machineCode: ServiceErrorCodes.PenNotFound);

            return penFromDb;
        }

        public async Task<PenEntity> EnsureOwnershipOfPen(int penId, ApplicationUserEntity user)
        {
            var penFromDb = await EnsureExistByIdAndActive(penId);
            if(penFromDb.Author.Id != user.Id)
                throw new ServiceException(
                    message: $"Not allowed",
                    errors: ["You are not allowed to do this action"],
                    isOperational: true,
                    machineCode: ServiceErrorCodes.NotAllowed);

            return penFromDb;
        }
    

        public async Task<OldPenVersionsEntity> EnsureOldVersionExistById(int oldVersionId)
        {
            var oldVersionFromDb = await _db.OldPenVersions
                .Include(v => v.Pen)
                .FirstOrDefaultAsync(v => v.Id == oldVersionId) ??
                throw new ServiceException(
                    message: $"Not exists",
                    errors: [$"Old version with id {oldVersionId} not exists"],
                    isOperational: true,
                    machineCode: ServiceErrorCodes.OldPenVersionNotFound);
            return oldVersionFromDb;
        }
    
        public async Task<OldPenVersionsEntity> EnsureOldVersionExistByIdAndActive(int oldVersionId)
        {
            var oldVersionFromDb = await EnsureOldVersionExistById(oldVersionId);
            if(oldVersionFromDb.Status == Models.Enums.EntityStatus.Deleted)
                throw new ServiceException(
                    message: $"Not exists",
                    errors: [$"Old version with id {oldVersionId} not exists"],
                    isOperational: true,
                    machineCode: ServiceErrorCodes.OldPenVersionNotFound);
            return oldVersionFromDb;
        }

        public async Task<OldPenVersionsEntity> EnsureOwnershipOfOldVersion(int oldVersionId, ApplicationUserEntity user)
        {
            var oldVersionFromDb = await EnsureOldVersionExistByIdAndActive(oldVersionId);

            if(oldVersionFromDb.Pen == null)
                throw new ServiceException(
                    message: $"Not exists",
                    errors: [$"Old version with id {oldVersionId} not exists"],
                    isOperational: true,
                    machineCode: ServiceErrorCodes.OldPenVersionNotFound);

            if (oldVersionFromDb.Pen.Author.Id != user.Id)
                throw new ServiceException(
                    message: $"Not allowed",
                    errors: ["You are not allowed to do this action"],
                    isOperational: true,
                    machineCode: ServiceErrorCodes.NotAllowed);
            return oldVersionFromDb;
        }

    }
}
