using System;
namespace SAIL.Framework.Host
{
    public interface IAppConfigFactory
    {
        IAppConfig GetAppConfig(string appCode);
    }
}
