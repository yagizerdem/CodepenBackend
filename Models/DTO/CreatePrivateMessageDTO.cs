using Microsoft.AspNetCore.Http;
using Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.DTO
{
    public class CreatePrivateMessageDTO
    {
        public string ReceiverId { get; set; } = null!;

        public string Message { get; set; } = null!;
    
        public List<IFormFile> Media { get; set; } = new();
    }

}
