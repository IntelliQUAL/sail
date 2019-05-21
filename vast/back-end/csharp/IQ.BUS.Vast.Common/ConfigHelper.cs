using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using SAIL.Framework.Host;

using IQ.BUS.Vast.Common.Helpers;
using IQ.RepositoryInterfaces.Vast;


namespace IQ.BUS.Vast.Common
{
    public static class ConfigHelper
    {
        public static IOperationConfig OperationConfig(IContext context, string configSourceId)
        {
            IOperationConfig operationConfig = null;

            string configSource = string.Empty;
            string configSourceKey = string.Empty;
            string configSourceInstanceGuid = string.Empty;
            string configSourceDatabaseName = string.Empty;

            if (context.GetByName(IQ.Entities.VastDB.Const.Context.CONFIG_SOURCE) != null)
            {
                configSource = context.GetByName(IQ.Entities.VastDB.Const.Context.CONFIG_SOURCE).ToString();
            }

            if (context.GetByName(IQ.Entities.VastDB.Const.Context.CONFIG_SOURCE_KEY) != null)
            {
                configSourceKey = context.GetByName(IQ.Entities.VastDB.Const.Context.CONFIG_SOURCE_KEY).ToString();
            }

            if (context.GetByName(IQ.Entities.VastDB.Const.Context.CONFIG_SOURCE_INSTANCE_GUID) != null)
            {
                configSourceInstanceGuid = context.GetByName(IQ.Entities.VastDB.Const.Context.CONFIG_SOURCE_INSTANCE_GUID).ToString();
            }

            if (context.GetByName(IQ.Entities.VastDB.Const.Context.CONFIG_SOURCE_DATABASE_NAME) != null)
            {
                configSourceDatabaseName = context.GetByName(IQ.Entities.VastDB.Const.Context.CONFIG_SOURCE_DATABASE_NAME).ToString();
            }

            // Load the Configuration Factory
            IOperationConfigFactory operationConfigFactory = context.Get<IOperationConfigFactory>();

            if (operationConfigFactory == null)
            {
                operationConfigFactory = new OperationConfigFactory();
            }

            operationConfig = operationConfigFactory.GetConfig(configSource, configSourceKey, configSourceId, configSourceInstanceGuid, configSourceDatabaseName);

            return operationConfig;
        }
    }
}
