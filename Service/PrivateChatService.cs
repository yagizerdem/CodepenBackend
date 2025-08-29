using DataAccess;
using Microsoft.EntityFrameworkCore;
using Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class PrivateChatService
    {
        private readonly ApplicationDbContext _db;
        public PrivateChatService(
            ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<PrivateChatMessageEntity> CreatePrivateChatMessage(PrivateChatMessageEntity message)
        {
            var createdMessage = (await _db.PrivateChatMessages.AddAsync(message)).Entity;
            await _db.SaveChangesAsync();
            return createdMessage;
        }

        public async Task<List<PrivateChatMessageEntity>> GetPrivateChatMessages(
            ApplicationUserEntity user, // current user that is fethching messages
            string targetUserId,
            int offset,
            int limit)
        {

            var payload = await _db.PrivateChatMessages
                .Where(m => m.ReceiverId == user.Id  && 
                m.SenderId == targetUserId || 
                (m.SenderId == user.Id &&
                m.ReceiverId == targetUserId) &&
                m.Status == Models.Enums.EntityStatus.Active)
                .OrderBy(m => m.CreatedAt)
                .Skip(offset)
                .Take(limit)
                .ToListAsync();

            return payload;
        }

    }
}
