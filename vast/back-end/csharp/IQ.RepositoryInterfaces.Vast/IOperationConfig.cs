using System;
using System.Collections.Generic;

using SAIL.Framework.Host;

namespace IQ.RepositoryInterfaces.Vast
{
    public interface IOperationConfig
    {
        Dictionary<string, string> GetSettings(IContext context);
        string ReadSetting(IContext context, string key, string defaultValue);
        bool ReadSetting(IContext context, string key, bool defaultValue);
        int ReadSetting(IContext context, string key, int defaultValue);
        double ReadSetting(IContext context, string key, double defaultValue);
    }
}
