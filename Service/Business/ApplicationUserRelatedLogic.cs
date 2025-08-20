using DataAccess;
using Microsoft.AspNetCore.Identity;
using Models.Entity;
using Models.Exceptions;
using Utils.ServiceErrorCodes;
using Models.Enums;

namespace Service.Business
{
    public class ApplicationUserRelatedLogic
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUserEntity> _userManager;
        public ApplicationUserRelatedLogic(
            ApplicationDbContext db,
            UserManager<ApplicationUserEntity> userManager)
        {
            _db = db;
            _userManager = userManager;
        }
        public async Task<ApplicationUserEntity> EnsureUserExistAndActiveByEmail(string email)
        {
            var  userFromDb = await _userManager.FindByEmailAsync(email)
                ?? throw new ServiceException(
                    message:"user not found",
                    errors: [$"user not found with email {email}"],
                    isOperational:true,
                    machineCode: ServiceErrorCodes.UserNotFound
                    );

            if (userFromDb.Status == EntityStatus.Deleted)
                throw new ServiceException(
                   message: "user not found",
                   errors: [$"user not found with email {email}"],
                   isOperational: true,
                   machineCode: ServiceErrorCodes.UserNotFound
                   );

            return userFromDb;

        }

        public async Task<ApplicationUserEntity> EnsureUserExistAndActiveById(string userId)
        {
            var userFromDb = await _userManager.FindByIdAsync(userId)
                ?? throw new ServiceException(
                    message: "user not found",
                    errors: [$"user not found with id {userId}"],
                    isOperational: true,
                    machineCode: ServiceErrorCodes.UserNotFound
                    );
            if (userFromDb.Status == EntityStatus.Deleted)
                throw new ServiceException(
                   message: "user not found",
                   errors: [$"user not found with id {userId}"],
                   isOperational: true,
                   machineCode: ServiceErrorCodes.UserNotFound
                   );
            return userFromDb;
        }

    }
}
