using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Enums
{
    public enum ArticleVisibility
    {
        Public, // everyone can see
        Private, // only author can see
        OnlyFollowers // only followers of the author can see
    }
}
