using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SAIL.Framework.Host
{
    /// <summary>
    /// Purpose:        Used to isolate the instance in it's own memory space.
    /// </summary>    
    public interface IProtectedMemory
    {
        bool RunInProtectedMemory { get; }
        int TimeoutSeconds { get; }
        int PoleIntervalMilliseconds { get; }
        int ListenerCount { get; }                                                      // Number of processes to monitor the queue.
        string ProcessArguments(IBusinessProcess businessProcess,
                                                    IConnectionHost connectionHost,
                                                    string listenerGuid);
        string ProcessFileName { get; }
        ITrace TraceEmit { get; }
    }
}
