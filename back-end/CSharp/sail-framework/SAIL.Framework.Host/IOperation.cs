using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SAIL.Framework.Host
{
    /// <summary>
    /// Created:        07/18/2017
    /// Author:         David J. McKee
    /// Purpose:        An internal operation generally used by a service or as a step within a pipeline.
    /// </summary>
    /// <typeparam name="I"></typeparam>
    /// <typeparam name="O"></typeparam>
    public interface IOperation<I, O>
    {
        void Execute(IContext context, I input, ref O output);
    }
}
