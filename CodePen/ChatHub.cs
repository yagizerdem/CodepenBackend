using DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Models.Entity;
using Service;

namespace CodePen
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _db;
        private readonly PrivateChatService _privateChatService;
        public ChatHub(
            ApplicationDbContext db,
            PrivateChatService privateChatService)
        {
            _db = db;
            _privateChatService = privateChatService;
        }


        public async Task SendMessage(string receiverId, string message)
        {
            PrivateChatMessageEntity chatMessage = new()
            {
                SenderId = Context.UserIdentifier!,
                ReceiverId = receiverId,
                Message = message
            };

            await _privateChatService.CreatePrivateChatMessage(chatMessage);

            await Clients.User(receiverId).SendAsync("ReceiveMessage", chatMessage);
            await Clients.User(Context.UserIdentifier!).SendAsync("ReceiveMessage", chatMessage);
        }
        
    }
}
