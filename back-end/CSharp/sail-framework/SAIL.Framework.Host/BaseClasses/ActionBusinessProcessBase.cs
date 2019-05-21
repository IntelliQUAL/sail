using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using SAIL.Framework.Host.Bootstrap;

namespace SAIL.Framework.Host.BaseClasses
{
    /// <summary>
    /// Created:        10/05/2014
    /// Author:         David J. McKee
    /// Purpose:        BusinessProcessBase plus context.
    /// </summary>    
    public abstract class ActionBusinessProcessBase<I, O> : BusinessProcessBase<I, O>
    {
        public abstract O ProcessRequest(IContext context, I request);

        public override O ProcessRequest(I request)
        {
            IContext context = CrossCuttingConcerns.BuildDefaultServiceContext(_connectionHost, this);

            return ProcessRequest(context, request);
        }

        public override I ExampleRequest
        {
            get
            {
                return (I)Activator.CreateInstance(typeof(I));
            }
        }

        public override O ExampleResponse
        {
            get
            {
                return (O)Activator.CreateInstance(typeof(O));
            }
        }
    }
}
