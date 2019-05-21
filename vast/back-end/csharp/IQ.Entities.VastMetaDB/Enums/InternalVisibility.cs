using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQ.Entities.VastMetaDB.Enums
{
    public enum InternalVisibility
    {
        // This value was upgraded from a bool.
        External = 0,           // 0 = Include in result-set
        Internal = 1,           // 1 = Exclude from result-set
        ExternalHidden = 2      // 2 = Include in the result-set but hide within the UI.
    }
}
