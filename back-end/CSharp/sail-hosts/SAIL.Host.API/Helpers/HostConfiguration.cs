using System;
using System.Xml;
using System.Web;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using SAIL.Framework.Host;

namespace SAIL.Host.API.Helpers
{
    public class HostConfiguration : IHostConfig
    {
        private const string FILE_NAME = "HostConfig.xml";

        private string _filename = string.Empty;
        private Hashtable _nameValuePair = null;
        private object _nameValuePairSyncLock = new object();

        public HostConfiguration()
        {
            _filename = FILE_NAME;
        }

        public HostConfiguration(string filename)
        {
            _filename = filename;
        }

        #region IConfiguration Members

        string IConfiguration.ReadSetting(string key, string defaultValue)
        {
            string hostConfigValue = defaultValue;

            try
            {
                EnsureHostSettings();

                key = key.Trim();

                lock (_nameValuePairSyncLock)
                {
                    if (_nameValuePair.ContainsKey(key))
                    {
                        hostConfigValue = _nameValuePair[key].ToString();
                    }
                }
            }
            catch { }

            return hostConfigValue;
        }

        private void EnsureHostSettings()
        {
            if (_nameValuePair == null)
            {
                _nameValuePair = new Hashtable();

                string basePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);

                const string EXPECTED_PREFIX = "file:\\";
                if (basePath.StartsWith(EXPECTED_PREFIX))
                {
                    basePath = basePath.Substring(EXPECTED_PREFIX.Length);
                }

                string hostConfigFilename = System.IO.Path.Combine(basePath, _filename);

                if (System.IO.File.Exists(hostConfigFilename))
                {
                    XmlDocument hostConfigDom = new XmlDocument();
                    hostConfigDom.Load(hostConfigFilename);

                    foreach (XmlNode setting in hostConfigDom.DocumentElement.ChildNodes)
                    {
                        if (setting.LocalName == "Setting")
                        {
                            XmlNode keyNode = setting.SelectSingleNode("Key");

                            if (keyNode != null)
                            {
                                string keyName = keyNode.InnerText.Trim();

                                if (string.IsNullOrEmpty(keyName) == false)
                                {
                                    XmlNode valueNode = setting.SelectSingleNode("Value");

                                    string value = string.Empty;

                                    if (valueNode != null)
                                    {
                                        value = valueNode.InnerText.Trim();
                                    }

                                    lock (_nameValuePair.SyncRoot)
                                    {
                                        _nameValuePair[keyName] = value;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        bool IConfiguration.ReadSetting(string key, bool defaultValue)
        {
            bool result = defaultValue;

            string boolAsText = ((IConfiguration)this).ReadSetting(key, defaultValue.ToString());

            bool newBool;
            if (bool.TryParse(boolAsText, out newBool))
            {
                result = newBool;
            }

            return result;
        }
        #endregion
    }
}