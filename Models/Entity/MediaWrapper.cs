using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entity
{
    public class MediaWrapper : BaseEntity
    {
        public long? Size { get; set; }
        public string? FileName { get; set; }
        public string? MimeType { get; set; }
        public byte[]? Data { get; set; }
    }
}
