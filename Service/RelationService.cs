using DataAccess;
using Microsoft.EntityFrameworkCore;
using Models.Entity;
using Models.Exceptions;
using Service.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.ServiceErrorCodes;

namespace Service
{
    public class RelationService
    {
        private readonly ApplicationDbContext _db;
        private readonly RelationRelatedLogic _relationRelatedLogic;
        private readonly ApplicationUserRelatedLogic _applicationUserRelatedLogic;
        public RelationService(
            ApplicationDbContext db,
            RelationRelatedLogic relationRelatedLogic,
            ApplicationUserRelatedLogic applicationUserRelatedLogic)
        {
            _db = db;
            _relationRelatedLogic = relationRelatedLogic;
            _applicationUserRelatedLogic = applicationUserRelatedLogic;
        }


        public async Task<FollowRequest> CreateFollowRequest(ApplicationUserEntity sender, string recieverId)
        {
            var reciever = await _applicationUserRelatedLogic.EnsureUserExistAndActiveById(recieverId);

            await _relationRelatedLogic.EnsureUsersAreNotInRelation(
                follower: sender,
                following: reciever);

            if (sender.Id == reciever.Id)
                throw new ServiceException(
                    message: "You cannot send a follow request to yourself",
                    machineCode: ServiceErrorCodes.SelfFollowRequest,
                    isOperational: true,
                    errors: ["You cannot send a follow request to yourself"]);

            await _relationRelatedLogic.EnsureNoActivePendingFallowRequest(
                senderId: sender.Id,
                recieverId: reciever.Id);

            var followRequest = new FollowRequest()
            {
                Sender = sender,
                Receiver = reciever,
                SenderId = sender.Id,
                ReceiverId = reciever.Id,
            };
            await _db.FollowRequests.AddAsync(followRequest);
            await _db.SaveChangesAsync();

            return followRequest;
        }

        public async Task<FollowRequest> RejectFollowRequest(int followRequestId, ApplicationUserEntity currentUser)
        {
            var followRequest = await _relationRelatedLogic.EnsureCanRejectFollowRequest(followRequestId, currentUser);

            followRequest.FollowRequestStatus = Models.Enums.FollowRequestStatus.Rejected;
            followRequest.Status = Models.Enums.EntityStatus.Deleted;
            followRequest.UpdatedAt = DateTime.UtcNow;
            _db.FollowRequests.Update(followRequest);
            await _db.SaveChangesAsync();

            return followRequest;
        }
    
        public async Task<FollowRequest> AcceptFollowRequest(int followRequestId, ApplicationUserEntity currentUser)
        {
            var followRequest = await _relationRelatedLogic.EnsureCanAcceptFollowRequest(followRequestId, currentUser);

            var followerUser = await _db.ApplicationUsers.
                FirstOrDefaultAsync(u => u.Id == followRequest.SenderId)
                ?? throw new ServiceException(
                    message:"following user not found",
                    errors: ["following user not found"],
                    isOperational:true,
                    machineCode:ServiceErrorCodes.UserNotFound);
            
            await _relationRelatedLogic.EnsureUsersAreNotInRelation(
                follower: followerUser,
                following: currentUser);

            // update the request 
            followRequest.FollowRequestStatus = Models.Enums.FollowRequestStatus.Accepted;
            followRequest.Status = Models.Enums.EntityStatus.Deleted;
            followRequest.UpdatedAt = DateTime.UtcNow;
            followRequest.Status = Models.Enums.EntityStatus.Active;
            _db.FollowRequests.Update(followRequest);
            // create the relation
            var relation = new RelationEntity()
            {
                Follower = followerUser,
                Following = currentUser,
                FollowerId = followerUser.Id,
                FollowingId = currentUser.Id,
                Status = Models.Enums.EntityStatus.Active
            };
            await _db.Relations.AddAsync(relation);

            // ensure update in 1 transaction
            await _db.SaveChangesAsync();
         
            return followRequest;
        }

        public async Task<RelationEntity> SoftDeleteRelation(ApplicationUserEntity follower, string followingId)
        {
            var following =  await _applicationUserRelatedLogic.EnsureUserExistAndActiveById(followingId);
            var relation = await _relationRelatedLogic.EnsureUsersAreInRelation(
                follower: follower,
                following: following);
        
            
            relation.Status = Models.Enums.EntityStatus.Deleted;
            _db.Relations.Update(relation); 
            await _db.SaveChangesAsync();

            return relation;
        }

        public async Task<RelationEntity> HardDeleteRelation(ApplicationUserEntity follower, string followingId)
        {
            var following = await _applicationUserRelatedLogic.EnsureUserExistAndActiveById(followingId);
            var relation = await _relationRelatedLogic.EnsureUsersAreInRelation(
                follower: follower,
                following: following);


            _db.Relations.Remove(relation);
            await _db.SaveChangesAsync();

            return relation;
        }

    }
}
