using System;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SAIL.Framework.Host
{
    public class DefaultTracer : ITrace
    {
        //[DllImport("Kernel32.dll", EntryPoint = "OutputDebugString")]
        //internal static extern void OutputDebugString(String s);

        void ITrace.Emit(TraceLevel traceLevel, string message)
        {
            //OutputDebugString(message + '\n');

            System.Diagnostics.Trace.WriteLine(message);
        }
    }
}
