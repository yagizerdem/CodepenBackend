using DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public class PrivateChatService
    {
        private readonly ApplicationDbContext _db;
        private readonly ApplicationUserRelatedLogic _applicationUserRelatedLogic;
        public PrivateChatService(
            ApplicationDbContext db,
            ApplicationUserRelatedLogic applicationUserRelatedLogic)
        {
            _db = db;
            _applicationUserRelatedLogic = applicationUserRelatedLogic;
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
                .Include(m => m.Media)
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


        public async Task<PrivateChatMessageEntity> CreatePrivateMessageWithMedia(CreatePrivateMessageDTO dto, ApplicationUserEntity sender)
        {
            var recieverFromDb = await _applicationUserRelatedLogic.EnsureUserExistAndActiveById(dto.ReceiverId);
            List<MediaWrapper> mediaWrappers = new();
            // creat media wrapper list if any
            foreach (IFormFile file  in dto.Media)
            {
                byte[] fileBytes = null;
                using (var ms = new MemoryStream())
                {
                    file.CopyTo(ms);
                    fileBytes = ms.ToArray();
                }

                MediaWrapper wrapper = new()
                {
                    FileName = file.FileName,
                    MimeType= file.ContentType,
                    Size = file.Length,
                    Data = fileBytes,
                };
                mediaWrappers.Add(wrapper);
            }

            PrivateChatMessageEntity message = new()
            {
                Message = dto.Message,
                Receiver = recieverFromDb,
                ReceiverId = recieverFromDb.Id,
                Sender = sender,
                SenderId = sender.Id,
            };

            if (mediaWrappers.Count > 0)
            {
                message.Media = mediaWrappers;
            }


            await _db.MediaWrapper.AddRangeAsync(mediaWrappers);
            await _db.PrivateChatMessages.AddAsync(message);

            await _db.SaveChangesAsync();

            return message;
        }
    

        public async Task<MediaWrapper?> GetPrivateMessageMedia(long mediaId)
        {
            var media = await _db.MediaWrapper
                .FirstOrDefaultAsync(m => m.Id == mediaId &&
                m.Status == Models.Enums.EntityStatus.Active);
            return media;
        }
    }
}
