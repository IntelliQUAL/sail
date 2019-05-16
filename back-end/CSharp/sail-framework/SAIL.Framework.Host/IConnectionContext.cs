using System;
using System.Text;
using System.Collections.Generic;

namespace SAIL.Framework.Host
{
    /// Created:        04/27/2017
    /// Author:         David J. McKee
    /// Purpose:        Provides access to the HttpContext
    /// </summary>
    public interface IConnectionContext
    {
        IConnectionHost ConnectionHost { get; set; }    // Reference to the equivalent of HttpContext 
        bool IsAsync { get; }                           // If true the return from .Execute is ignored. The assumption is anything that needs to be sent has already been sent.
    }
}
