using AutoMapper;
using DataAccess;
using Microsoft.AspNetCore.Identity;
using Models.DTO;
using Models.Entity;
using Utils.ExtensionMethods;

namespace Service
{
    public class ApplicationUserService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUserEntity> _userManager;
        public ApplicationUserService(
            ApplicationDbContext db,
            IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<ApplicationUserEntity> CreateUser(RegisterDTO dto)
        {
            var appUser = _mapper.Map<ApplicationUserEntity>(dto);  
            string password = dto.Password;
            var identityResult = await _userManager.CreateAsync(appUser, password);
            identityResult.ThrowIfFailed();

            return appUser;
        }

        public async Task<ApplicationUserEntity> LogIn()
        {

            return null;
        }

    }
}
