using System;
using System.Collections.Generic;
using System.Text;

namespace SAIL.Framework.Host
{
    /// <summary>
    /// Created:        05/16/2019
    /// Author:         David J. McKee
    /// Purpose:        Simple pattern for reading host or applicatio configuration information.
    /// </summary>
    public interface IConfiguration
    {
        string ReadSetting(string key, string defaultValue);
        bool ReadSetting(string key, bool defaultValue);
    }
}
