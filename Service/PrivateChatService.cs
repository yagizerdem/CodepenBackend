using DataAccess;
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


    }
}
