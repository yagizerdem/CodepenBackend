using DataAccess;
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
            followRequest.FollowRequestStatus = Models.Enums.FollowRequestStatus.Accepted;
            followRequest.Status = Models.Enums.EntityStatus.Deleted;
            followRequest.UpdatedAt = DateTime.UtcNow;
            followRequest.Status = Models.Enums.EntityStatus.Active;
            _db.FollowRequests.Update(followRequest);
            await _db.SaveChangesAsync();
         
            return followRequest;
        }

    }
}
