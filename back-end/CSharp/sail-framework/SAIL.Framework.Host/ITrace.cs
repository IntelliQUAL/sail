using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using System.Diagnostics;

namespace SAIL.Framework.Host
{
    public interface ITrace
    {
        void Emit(TraceLevel traceLevel, string message);   // Traces a single message.
    }
}
