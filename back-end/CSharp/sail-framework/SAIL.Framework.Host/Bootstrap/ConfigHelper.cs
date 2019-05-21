using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAIL.Framework.Host.Bootstrap
{
    public static class ConfigHelper
    {
        private static string[] _businessProcessPathList = null;

        private static IConfiguration _hostConfiguration = new HostConfiguration();

        public static IConfiguration HostConfiguration
        {
            get { return ConfigHelper._hostConfiguration; }
            set { ConfigHelper._hostConfiguration = value; }
        }

        public static string[] ReadBusinessProcessPathList()
        {
            if (_businessProcessPathList == null)
            {
                const string BUSINESS_PROCESS_PATH_LIST = "BusinessProcessPathList";

                string defaultPath = ExecutingAssemblyPath();

                // This contains a pipe delimited list of file system locations.
                const string PATH_SEP = "|";
                _businessProcessPathList = _hostConfiguration.ReadSetting(BUSINESS_PROCESS_PATH_LIST, defaultPath).Split(new string[] { PATH_SEP }, StringSplitOptions.RemoveEmptyEntries);
            }

            return _businessProcessPathList;
        }

        public static string ExecutingAssemblyPath()
        {
            string executingAssembly = System.Reflection.Assembly.GetExecutingAssembly().CodeBase.Substring("file:///".Length);

            string defaultPath = new System.IO.FileInfo(executingAssembly).DirectoryName;
            return defaultPath;
        }
    }
}
