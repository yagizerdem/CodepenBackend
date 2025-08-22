using DataAccess;
using Microsoft.Extensions.DependencyInjection;
using Models.Entity;
using Service;

namespace TestService
{
    public class RelationServiceTest : ServiceBase
    {

        private readonly RelationService _relationService;
        private readonly ApplicationDbContext _db;
        public RelationServiceTest()
        {
            _db = _serviceProvider.GetRequiredService<ApplicationDbContext>();
            _relationService = _serviceProvider.GetRequiredService<RelationService>();

            this.SeedAppUsersFast();
        }



        [Fact]
        public void Test1()
        {
            List<ApplicationUserEntity> user = this._db.ApplicationUsers.ToList();


            ;
        }
    }
}
