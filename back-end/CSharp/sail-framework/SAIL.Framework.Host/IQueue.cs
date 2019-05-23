using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SAIL.Framework.Host
{
    /// <summary>
    /// Created:        06/11/2017
    /// Author:         David J. McKee
    /// Purpose:        Simple interface for a Queue
    /// Important:      A string is used for the request because of potential problems with object marshalling.
    /// </summary>
    public interface IQueue
    {
        // Execute the given service behind the given queue.
        // Important: 'request' is a string because of problems with object marshalling.
        void Enqueue(IContext context, string serviceName, string request);

        // returns request.
        // Note: Blocking call for x seconds until first message is received.
        string Receive(IContext context, int timeoutSeconds, out string serviceName);
    }
}
