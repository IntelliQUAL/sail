using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SAIL.Framework.Host.Types
{
    public class Claim
    {
        public Claim()
        {

        }

        public Claim(string claimType, string claimValue)
        {
            this.Type = claimType;
            this.Value = claimValue;
        }

        public string Type = string.Empty;
        public string Value = string.Empty;
    }
}
