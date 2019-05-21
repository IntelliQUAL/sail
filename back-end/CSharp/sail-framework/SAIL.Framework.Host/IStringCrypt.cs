using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAIL.Framework.Host
{
    public interface IStringCrypt
    {
        string Encrypt(string unencrypted);
        string Decrypt(string encrypted);
    }
}
