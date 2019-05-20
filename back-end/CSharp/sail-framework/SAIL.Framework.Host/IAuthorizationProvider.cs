using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using SAIL.Framework.Host.Types;

namespace SAIL.Framework.Host
{
    public interface IAuthorizationProvider
    {
        List<Claim> SearchClaims(IContext context, IAuthToken authToken);
        // Example: claimType:=role, claimValue:=Admin
        bool HasClaim(IContext context, IAuthToken authToken, string claimType, string claimValue);
    }
}
