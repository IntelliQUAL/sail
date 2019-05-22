using System;
using System.Collections.Generic;

namespace SAIL.Framework.Host
{
    public interface IAuthCredentials
    {
        Dictionary<string, string> Credentials { get; }
    }
}
