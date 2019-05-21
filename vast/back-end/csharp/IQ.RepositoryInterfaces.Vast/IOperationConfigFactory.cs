using System;
using SAIL.Framework.Host;

namespace IQ.RepositoryInterfaces.Vast
{
    public interface IOperationConfigFactory
    {
        IOperationConfig AutoLocate(IContext context, string configSourceId);
        IOperationConfig GetConfig(string configSource, string configSourceKey, string configSourceID, string configSourceInstanceGuid, string configSourceDatabaseName);
    }
}
