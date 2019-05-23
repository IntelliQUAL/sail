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
    /// Purpose:        Finds a given queue by name
    /// </summary>
    public interface IQueueFactory
    {
        IQueue Find(IContext context, string queueName);
    }
}
