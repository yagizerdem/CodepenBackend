using DataAccess;
using Microsoft.EntityFrameworkCore;
using Models.Entity;
using Models.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.ServiceErrorCodes;

namespace Service.Business
{
    public class RelationRelatedLogic
    {
        private readonly ApplicationDbContext _db;
        public RelationRelatedLogic(
            ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task EnsureNoActivePendingFallowRequest(
            string senderId,
            string recieverId)
        {
            var followRequest = await _db.FollowRequests.FirstOrDefaultAsync(
                x => x.Status == Models.Enums.EntityStatus.Active &&
                x.SenderId == senderId && x.ReceiverId == recieverId);

            if (followRequest != null)
                throw new Models.Exceptions.ServiceException(
                    message: "You already have an active follow request with this user",
                    machineCode: ServiceErrorCodes.ActiveFollowRequest,
                    isOperational: true,
                    errors: ["You already have an active follow request with this user"]);

        }

        public async Task<FollowRequest> EnsureCanRejectFollowRequest(int followRequestId, ApplicationUserEntity currentUser)
        {
            if (currentUser == null)
            {
                throw new ServiceException(
                    message: "Current user is invalid",
                    isOperational: true,
                    errors: ["Current user is invalid"],
                    machineCode: ServiceErrorCodes.NotAllowed);
            }

            var followRequest = await _db.FollowRequests
                .FirstOrDefaultAsync(fr =>
                    fr.Id == followRequestId &&
                    fr.Status == Models.Enums.EntityStatus.Active &&
                    fr.FollowRequestStatus == Models.Enums.FollowRequestStatus.Pending &&
                    fr.ReceiverId == currentUser.Id);

            if (followRequest == null)
            {
                throw new ServiceException(
                    message: "Follow request does not exist or cannot be rejected",
                    isOperational: true,
                    errors: ["Follow request not found, not pending, or you are not the receiver" ],
                    machineCode: ServiceErrorCodes.NotAllowed);
            }

            return followRequest;
        }

        public async Task<FollowRequest> EnsureCanAcceptFollowRequest(int followRequestId, ApplicationUserEntity currentUser)
        {
            if (currentUser == null)
            {
                throw new ServiceException(
                    message: "Current user is invalid",
                    isOperational: true,
                    errors: ["Current user is invalid"],
                    machineCode: ServiceErrorCodes.NotAllowed);
            }
            var followRequest = await _db.FollowRequests
                .FirstOrDefaultAsync(fr =>
                    fr.Id == followRequestId &&
                    fr.Status == Models.Enums.EntityStatus.Active &&
                    fr.FollowRequestStatus == Models.Enums.FollowRequestStatus.Pending &&
                    fr.ReceiverId == currentUser.Id);
            if (followRequest == null)
            {
                throw new ServiceException(
                    message: "Follow request does not exist or cannot be accepted",
                    isOperational: true,
                    errors: ["Follow request not found, not pending, or you are not the receiver" ],
                    machineCode: ServiceErrorCodes.NotAllowed);
            }
            return followRequest;
        }

    }
}
