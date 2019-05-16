using System;
using System.Collections.Generic;
using System.Text;

namespace SAIL.Framework.Host
{
    /// <summary>
    /// Created:        04/27/2017
    /// Author:         David J. McKee
    /// Purpose:        Provides a common way to swallow exceptions.
    /// </summary>
    public interface IExceptionHandler
    {
        // ASSUMPTION: The correlation id is contained withint the context.         
        void HandleException(IContext context, System.Exception ex);                  // Most common error handler        
        void HandleException(IContext context, System.Exception ex, IServiceResponse serviceResponse);
        void HandleException(IContext context, System.Exception ex, string detail);   // Allows the developer to include additional debug information about the exception.        
    }
}
