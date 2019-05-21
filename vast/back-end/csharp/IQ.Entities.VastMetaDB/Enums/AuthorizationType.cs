using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace IQ.Entities.VastMetaDB.Enums
{
    public enum AuthorizationType
    {
        Public = 1,     // unauthenticated users
        Private = 2,    // all authenticated users
        Special = 3,    // each user requires explicit access
        Referrer = 4    // Contextual {{ip or domain list}} - public to specific domains or IPs
    }
}
