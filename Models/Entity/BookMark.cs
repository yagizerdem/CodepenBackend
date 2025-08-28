using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Entity
{
    public class BookMark : BaseEntity
    {
        public string UserId { get; set; }
        public ApplicationUserEntity User { get; set; }
        public int ArticleId { get; set; }
        public ArticleEntity Article { get; set; }
    }
}
