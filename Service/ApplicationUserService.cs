using AutoMapper;
using DataAccess;
using Microsoft.AspNetCore.Identity;
using Models.DTO;
using Models.Entity;
using Service.Business;
using Utils.ExtensionMethods;

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

            return userFromDb;
        }

    }
}
