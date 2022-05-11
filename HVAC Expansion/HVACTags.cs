using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HVACExpansion
{
    class HVACTags
    {
        public static readonly Tag FullyCondensable = TagManager.Create(nameof(FullyCondensable));
        public static readonly Tag FullyEvaporatable = TagManager.Create(nameof(FullyEvaporatable));
    }
}
