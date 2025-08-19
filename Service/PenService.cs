using AutoMapper;
using DataAccess;
using Models.DTO;
using Models.Entity;
using Service.Business;

namespace Service
{
    public class PenService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly PenRelatedLogic _penRelatedLogic;
        public PenService(
            ApplicationDbContext db,
            IMapper mapper,
            PenRelatedLogic penRelatedLogic)
        {
            _db = db;
            _mapper = mapper;
            _penRelatedLogic = penRelatedLogic;
        }

        public async Task<PenEntity> CreatePen(CreatePenDTO dto, ApplicationUserEntity user)
        {
            // run some business logic if needed 

            var newPen = _mapper.Map<PenEntity>(dto);
            newPen.Author = user;
            await _db.Pens.AddAsync(newPen);
            await _db.SaveChangesAsync();
            return newPen;
        }

        public async Task<PenEntity> SoftDelete(int penId, ApplicationUserEntity user)
        {
            var penFromDb = await _penRelatedLogic.EnsureOwnershipOfPen(penId, user);
            penFromDb.Status = Models.Enums.EntityStatus.Deleted;
            _db.Pens.Update(penFromDb);
            await _db.SaveChangesAsync();
            return penFromDb;
        }

        public async Task<PenEntity> HardDelete(int penId, ApplicationUserEntity user)
        {
            var penFromDb = await _penRelatedLogic.EnsureOwnershipOfPen(penId, user);
            _db.Pens.Remove(penFromDb);
            await _db.SaveChangesAsync();
            return penFromDb;
        }

        public async Task<PenEntity> UpdatePen(int penId, UpdatePenDTO dto, ApplicationUserEntity user)
        {
            var penFromDb = await _penRelatedLogic.EnsureOwnershipOfPen(penId, user);
            _mapper.Map(dto, penFromDb);
            penFromDb.UpdatedAt = DateTime.UtcNow;
            _db.Pens.Update(penFromDb);
            await _db.SaveChangesAsync();
            return penFromDb;
        }

        public async Task<OldPenVersionsEntity> MigrateNewVersion(int penId, ApplicationUserEntity user)
        {
            var penFromDb = await _penRelatedLogic.EnsureOwnershipOfPen(penId, user);
            var oldVersion = _mapper.Map<OldPenVersionsEntity>(penFromDb);
            penFromDb.Version++;
            oldVersion.Pen = penFromDb;
            oldVersion.PenId = penFromDb.Id;

            await _db.OldPenVersions.AddAsync(oldVersion);
            _db.Pens.Update(penFromDb);
            await _db.SaveChangesAsync();

            return oldVersion;
        }

        public async Task<OldPenVersionsEntity> SoftDeleteOldVersion(int oldVersionId, ApplicationUserEntity user)
        {
            var oldVersionFromDb = await _penRelatedLogic.EnsureOwnershipOfOldVersion(oldVersionId, user);
            oldVersionFromDb.Status = Models.Enums.EntityStatus.Deleted;
            _db.OldPenVersions.Update(oldVersionFromDb);
            await _db.SaveChangesAsync();

            return oldVersionFromDb;
        }

        public async Task<OldPenVersionsEntity> HardDeleteOldVersion(int oldVersionId, ApplicationUserEntity user)
        {
            var oldVersionFromDb = await _penRelatedLogic.EnsureOwnershipOfOldVersion(oldVersionId, user);
            _db.OldPenVersions.Remove(oldVersionFromDb);
            await _db.SaveChangesAsync();

            return oldVersionFromDb;
        }
    
    }
}
