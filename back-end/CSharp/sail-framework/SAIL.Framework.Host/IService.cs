using System;
using System.Text;
using System.Collections.Generic;

using SAIL.Framework.Host.Enums;

namespace SAIL.Framework.Host
{
    /// <summary>
    /// Created:        04/27/2017
    /// Author:         David J. McKee
    /// Purpose:        The purpose of this interface is to define the pattern for any service for the purpose of late binding / plug-ins.
    /// </summary>
    public interface IService
    {
        /// Created:        04/27/2017
        /// Author:         David J. McKee
        /// Purpose:        Provides a never changing interface for services.
        /// Assumptions:    
        ///                 1. Versioning is handled by the assembly name or class name.
        ///                 2. The Response Format is assumed to also be the request format unless manually changed.
        /// Tip:            Use IConnectionContext to get to the raw HttpContext
        /// <param name="dataPayload">Raw JSON, XML or other</param>
        /// <param name="responseFormat"></param>
        /// <returns>Formatted Response in XML, JSON, or Binary (base64 string)</returns>
        string Execute(IContext context, string dataPayload, ResponseFormat responseFormat);
    }
}
