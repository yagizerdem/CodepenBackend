using Models.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ResponseTypes
{
    public class GetPensRespnose
    {
        public int TotalHits { get; set; }

        public List<PenEntity> Pens { get; set; } = new List<PenEntity>();
    }
}
