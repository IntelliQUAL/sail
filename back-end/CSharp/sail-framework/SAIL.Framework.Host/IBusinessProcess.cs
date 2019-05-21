using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using SAIL.Framework.Host.Enums;

namespace SAIL.Framework.Host
{
    public interface IBusinessProcess
    {
        /// Purpose:        Provides a never changing interface for web services.
        /// Assumptions:    
        ///                 1. Versioning is handled by the assembly name or class name.
        ///                 2. The Response Format is also almost always also the request format.
        /// Tip:            Use IConnectionContext to get to the raw HttpContext
        /// <param name="dataPayload">Raw XML, JSON, or other HTTP POST text</param>
        /// <param name="responseFormat"></param>
        /// <returns>Formatted Response in XML, JSON, or Binary (base64 string)</returns>
        string Execute(string dataPayload, ResponseFormat responseFormat);
    }
}
