using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entity
{
    public class PrivateChatMessageEntity : BaseEntity
    {
        public int Id { get; set; }

        public string SenderId { get; set; } = null!;
        public ApplicationUserEntity Sender { get; set; } = null!;

        public string ReceiverId { get; set; } = null!;

        public ApplicationUserEntity Receiver { get; set; } = null!;

        public string Message { get; set; } = null!;

        public List<MediaWrapper>? Media { get; set; } = new List<MediaWrapper>();

    }
}
