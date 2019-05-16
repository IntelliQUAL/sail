using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAIL.Framework.Host
{
    /// <summary>
    /// Created:        07/05/2017
    /// Author:         David J. McKee
    /// Purpose:        Provides access to common results for all service responses.
    /// Usage:          Generally applied to a view model response    
    /// Important:      This response may be used in conjunction with HTTP Result codes.
    /// </summary>
    public interface IServiceResponse
    {
        bool Success { get; set; }          // success or failure (True or False)
        string ErrorCode { get; set; }      // When Success = false, API specific error code
        string ErrorText { get; set; }      // System error message intended for technical people e.g. "Authentication Required" or "File Not Found" but should NOT be exception.ToString()
        string UserMessage { get; set; }    // e.g. alert('Your error has been logged. Please include error code 'ABC23-434234-ASGSG' when reporting this error.')
        string CorrelationId { get; set; }  // GUID Used to chain services together.
    }
}
