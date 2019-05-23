using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAIL.Framework.Host
{
    public interface ISessionManager
    {
        string ReadSessionValue(string name, string defaultValue);
        void WriteSessionValue(string name, string value);
        void ClearSession();
    }
}
