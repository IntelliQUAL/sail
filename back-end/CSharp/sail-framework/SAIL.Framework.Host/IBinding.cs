using System;
using System.Text;
using System.Collections.Generic;

namespace SAIL.Framework.Host
{
    /// <summary>
    /// Created:        05/16/2019
    /// Author:         David J. McKee
    /// Purpose:        The purpose of this interface is to define the pattern for late binding / use of plug-ins.
    /// </summary>
    public interface IBinding
    {
        // Late binds a given type based on it's full name.
        // CRITICAL CONVENTION: The assembly name and the namespace MUST match. The class must be contained within the root namespace.
        // For example:
        //  Input: namespacePlusClassName   - Full namespace / assembly name . class name
        //  Input: issuesList               - if the class fails to load the list will contain one or more reasons.
        //  Input: args                     - optional parameters which may be passed to the constructor.
        object LoadViaFullName(IContext context, string namespacePlusClassName, params object[] args);
        object LoadViaFullName(IContext context, string namespacePlusClassName, StringBuilder issueList, params object[] args);
        List<string> ReadServicePathList(IContext context);
    }
}
