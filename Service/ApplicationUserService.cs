using DataAccess;
using Models.DTO;
using Models.Entity;

namespace Service
{
    public class ApplicationUserService
    {
        private readonly ApplicationDbContext _db;
        public ApplicationUserService(
            ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<ApplicationUserEntity> CreateUser(RegisterDTO dto)
        {

            return null;
        }

    }
}
