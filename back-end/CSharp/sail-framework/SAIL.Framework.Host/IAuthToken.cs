using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAIL.Framework.Host
{
    public interface IAuthToken
    {
        bool Load(IContext context);        // Loads the token from the context.
        string Read(IContext context);      // Reads the token as a string.
        void SetClaim(string claimType, string claimValue);
    }
}
