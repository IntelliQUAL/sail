using System;
using System.Text;
using System.Linq;
using System.Web;
using System.Collections.Generic;

using SAIL.Framework.Host;

namespace SAIL.Host.API.Helpers
{
    /// <summary>
    /// Created:        04/27/2017
    /// Author:         David J. McKee
    /// Purpose:        Implements a default boostrap binder needed to get things started.
    /// </summary>
    public class Binder : IBinding
    {
        object IBinding.LoadViaFullName(IContext context, string namespacePlusClassName, StringBuilder issueListt, params object[] args)
        {
            object instance = null;

            try
            {
                instance = LateBinding.LoadViaFullName(context, namespacePlusClassName, issueListt, args);
            }
            catch // (System.Exception ex)
            {

            }

            return instance;
        }

        object IBinding.LoadViaFullName(IContext context, string namespacePlusClassName, params object[] args)
        {
            object instance = null;

            StringBuilder issueListt = new StringBuilder();

            try
            {
                instance = LateBinding.LoadViaFullName(context, namespacePlusClassName, issueListt, args);
            }
            catch // (System.Exception ex)
            {

            }

            return instance;
        }

        List<string> IBinding.ReadServicePathList(IContext context)
        {
            string[] servicePathArray = LateBinding.ReadBusinessProcessPathList(context);

            List<string> servicePathList = new List<string>(servicePathArray);

            return servicePathList;
        }
    }
}