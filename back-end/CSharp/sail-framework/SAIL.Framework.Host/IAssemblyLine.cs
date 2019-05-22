using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SAIL.Framework.Host.Bootstrap;

namespace SAIL.Framework.Host
{
    /// <summary>
    /// Created:        11/03/2014
    /// Author:         David J. McKee
    /// Purpose:        Multiple IOperations are generally combined into an assembly line.
    ///                 Assembly lines are then used by business processes.
    /// </summary>
    public interface IAssemblyLine<I, O>
    {
        O Execute(FlowTransport<I> context);
    }
}
