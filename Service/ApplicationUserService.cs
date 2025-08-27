using AutoMapper;
using DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.DTO;
using Models.Entity;
using Models.Exceptions;
using Service.Business;
using Utils.ExtensionMethods;
using Utils.ServiceErrorCodes;

namespace Service
{
    public class ApplicationUserService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUserEntity> _userManager;
        private readonly ApplicationUserRelatedLogic _applicationUserRelatedLogic;
        private readonly SignInManager<ApplicationUserEntity> _signInManager;
        public ApplicationUserService(
            ApplicationDbContext db,
            IMapper mapper,
            UserManager<ApplicationUserEntity> userManager,
            ApplicationUserRelatedLogic applicationUserRelatedLogic,
            SignInManager<ApplicationUserEntity> signInManager)
        {
            _db = db;
            _mapper = mapper;
            _userManager = userManager;
            _applicationUserRelatedLogic = applicationUserRelatedLogic;
            _signInManager = signInManager;
        }

        public async Task<ApplicationUserEntity> CreateUser(RegisterDTO dto)
        {
            var appUser = _mapper.Map<ApplicationUserEntity>(dto);  
            string password = dto.Password;
            var identityResult = await _userManager.CreateAsync(appUser, password);
            identityResult.ThrowIfFailed();

            return appUser;
        }

        public async Task<ApplicationUserEntity> LogIn(LogInDTO dto)
        {
            var userFromDb = await _applicationUserRelatedLogic.EnsureUserExistAndActiveByEmail(dto.Email);
            var result = await _signInManager.PasswordSignInAsync(
                userFromDb,
                dto.Password,
                isPersistent: true,    
                lockoutOnFailure: true 
            );

            result.ThrowIfFailed();

            return userFromDb;
        }

        public async Task<ApplicationUserEntity> UploadProfilePhoto(UploadFileDTO dto, ApplicationUserEntity user)
        {
            if(user.ProfilePictureId != null)
            {
                var oldProfileWrapper= await _db.MediaWrapper.FirstOrDefaultAsync(x => x.Id  == user.ProfilePictureId);
                if (oldProfileWrapper != null)
                {
                    oldProfileWrapper.Status = Models.Enums.EntityStatus.Deleted;
                    _db.MediaWrapper.Update(oldProfileWrapper);
                }
            }


            user.ProfilePictureId = null; // guarantee that the profile picture is reset
            user.ProfilePicture = null; // guarantee that the profile picture is reset

            MediaWrapper wrapper = new()
            {
                FileName = dto.File.FileName,
                Size = dto.File.Length,
                MimeType = dto.File.ContentType,
                Data = ConvertToByteArray(dto.File)
            };

            await _db.MediaWrapper.AddAsync(wrapper);
            user.ProfilePicture = wrapper;
            user.ProfilePictureId = wrapper.Id;
            _db.ApplicationUsers.Update(user);
            await _db.SaveChangesAsync();

            return user;
        }

        public async Task<ApplicationUserEntity> SoftDeleteProfilePhoto(ApplicationUserEntity user)
        {
            if (user.ProfilePictureId == null)
                throw new ServiceException(
                        message: "profile picture not found",
                        errors: ["profile picture not found"],
                        isOperational: true,
                        machineCode: ServiceErrorCodes.ProfilePictureNotFound
                    );

            var wrapper = await _db.MediaWrapper.FirstOrDefaultAsync(x => x.Id == user.ProfilePictureId);
            if(wrapper != null)
            {
                wrapper.Status = Models.Enums.EntityStatus.Deleted; // mark the profile picture as deleted
                _db.MediaWrapper.Update(wrapper);
            }

            user.ProfilePictureId = null; // reset the profile picture id
            user.ProfilePicture = null; // reset the profile picture
            _db.ApplicationUsers.Update(user);
            await _db.SaveChangesAsync();

            return user;
        }

        public async Task<MediaWrapper?> GetProfileImage(string userId, ApplicationUserEntity user)
        {
            var flag = false; // indicates user can access image of profile 

            // validate with business logic to determine if user can access the profile image
            if (userId == user.Id)
                flag = true;


            // validation end 
            if (!flag)
                return null;

            var userFromDb = await _db.ApplicationUsers
                .FirstOrDefaultAsync(x => x.Id == user.Id &&
                x.Status == Models.Enums.EntityStatus.Active)
                ?? throw new ServiceException(
                    message: "user not found",
                    errors: ["user not found"],
                    isOperational: true,
                    machineCode: ServiceErrorCodes.UserNotFound
                );

            if(userFromDb.ProfilePictureId == null)
                throw new ServiceException(
                    message: "profile image not found",
                    errors: ["profile image found"],
                    isOperational: true,
                    machineCode: ServiceErrorCodes.ProfilePictureNotFound
                ); 


            var mediaWrapperFromDb = _db.MediaWrapper.FirstOrDefault(x => x.Id == 
            userFromDb.ProfilePictureId && 
            x.Status == Models.Enums.EntityStatus.Active);

            if (mediaWrapperFromDb == null
                || mediaWrapperFromDb.Status == Models.Enums.EntityStatus.Deleted
                || mediaWrapperFromDb.Data == null
                || mediaWrapperFromDb.Size == 0
                || mediaWrapperFromDb.Data.Length == 0) flag = false;


            return mediaWrapperFromDb;
        }
        
        public async Task<(int followers, int followings)> GetUsersFollowerFollowingsCount(
            string targetUserId,
            ApplicationUserEntity currentUser)
        {

            var followersTask = await _db.Relations
                .AsNoTracking()
                .Where(r => r.Status == Models.Enums.EntityStatus.Active
                         && r.FollowingId == targetUserId)   
                .CountAsync();

            var followingsTask = await _db.Relations
                .AsNoTracking()
                .Where(r => r.Status == Models.Enums.EntityStatus.Active
                         && r.FollowerId == targetUserId)    
                .CountAsync();

            return (followersTask, followingsTask);
        }


        // helper 
        private static byte[] ConvertToByteArray(IFormFile file)
        {
            using var ms = new MemoryStream();
            file.CopyTo(ms);
            return ms.ToArray();
        }

    }
}
