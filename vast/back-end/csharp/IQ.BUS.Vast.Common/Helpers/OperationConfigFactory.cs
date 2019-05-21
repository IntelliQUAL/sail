using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using SAIL.Framework.Host;

using IQ.RepositoryInterfaces.Vast;

namespace IQ.BUS.Vast.Common.Helpers
{
    public class OperationConfigFactory : IOperationConfigFactory
    {
        IOperationConfig IOperationConfigFactory.GetConfig(string configSource, string configSourceKey, string configSourceID, string configSourceInstanceGuid, string configSourceDatabaseName)
        {
            return new OperationConfig(configSource, configSourceKey, configSourceID, configSourceInstanceGuid, configSourceDatabaseName);
        }


        IOperationConfig IOperationConfigFactory.AutoLocate(IContext context, string configSourceId)
        {
            return IQ.BUS.Vast.Common.ConfigHelper.OperationConfig(context, configSourceId);
        }
    }
}
