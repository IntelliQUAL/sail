using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SAIL.Framework.Host
{
    public interface IAuthenticationProvider
    {
        bool AuthenticateToken(IContext context, IAuthToken authToken);
    }
}
