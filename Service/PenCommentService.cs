using AutoMapper;
using DataAccess;
using Models.DTO;
using Models.Entity;
using Service.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class PenCommentService
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly PenRelatedLogic _penRelatedLogic;
        private readonly PenCommentRelatedLogic _penCommentRelatedLogic;
        
        public PenCommentService(
            ApplicationDbContext db,
            IMapper mapper,
            PenRelatedLogic penRelatedLogic,
            PenCommentRelatedLogic penCommentRelatedLogic)
        {
            _db = db;
            _mapper = mapper;
            _penRelatedLogic = penRelatedLogic;
            _penCommentRelatedLogic = penCommentRelatedLogic;
        }


        public async Task<PenCommentEntity> CreatePenComment(CreatePenCommentDTO dto, ApplicationUserEntity user)
        {
            // run bussiness logic if needed

            await _penRelatedLogic.EnsureExistByIdAndActive(dto.PenId);
            PenCommentEntity comment = _mapper.Map<PenCommentEntity>(dto);
            comment.User = user;
            await _db.PenComments.AddAsync(comment);
            await _db.SaveChangesAsync();

            return comment;
        }

        public async Task<PenCommentEntity> SoftDeleteComment(int penCommentId, ApplicationUserEntity user)
        {
            var penCommentFromDb = await _penCommentRelatedLogic.EnsureOwnershipOfPenComment(penCommentId, user);
            penCommentFromDb.Status = Models.Enums.EntityStatus.Deleted;    
            _db.PenComments.Update(penCommentFromDb);
            await _db.SaveChangesAsync();
             
            return penCommentFromDb;
        }

        public async Task<PenCommentEntity> HardDeleteComment(int penCommentId, ApplicationUserEntity user)
        {
            var penCommentFromDb = await _penCommentRelatedLogic.EnsureOwnershipOfPenComment(penCommentId, user);
            _db.PenComments.Remove(penCommentFromDb);
            await _db.SaveChangesAsync();

            return penCommentFromDb;
        }

    }
}
