using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using SAIL.Framework.Host;

using IQ.Entities.VastDB;
using IQ.Entities.VastDB.Const;
using IQ.RepositoryInterfaces.Vast;
using IQ.Entities.VastMetaDB.SqlEsque;

namespace IQ.BUS.Vast.Common.Helpers
{
    internal class OperationConfig : IOperationConfig
    {
        string _configSource = string.Empty;
        string _configSourceKey = string.Empty;
        string _configSourceId = string.Empty;
        string _configSourceInstanceGuid = string.Empty;
        string _configSourceDatabaseName = string.Empty;

        Dictionary<string, string> _settings = null;

        public OperationConfig(string configSource, string configSourceKey, string configSourceId,
                                string configSourceInstanceGuid, string configSourceDatabaseName)
        {
            _configSource = configSource;
            _configSourceKey = configSourceKey;
            _configSourceId = configSourceId;
            _configSourceInstanceGuid = configSourceInstanceGuid;
            _configSourceDatabaseName = configSourceDatabaseName;
        }

        /// <summary>
        /// Important:  This is a great place for a cache.
        /// </summary>        
        string IOperationConfig.ReadSetting(IContext context, string key, string defaultValue)
        {
            string result = defaultValue;

            try
            {
                if (_settings == null)
                {
                    _settings = ReadSettings(context);
                }

                if (_settings != null)
                {
                    if (_settings.ContainsKey(key))
                    {
                        result = _settings[key];
                    }
                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }

            return result;
        }


        bool IOperationConfig.ReadSetting(IContext context, string key, bool defaultValue)
        {
            bool result = defaultValue;

            IOperationConfig operationConfig = (IOperationConfig)this;

            string settingText = operationConfig.ReadSetting(context, key, defaultValue.ToString());

            bool newBool;
            if (bool.TryParse(settingText, out newBool))
            {
                result = newBool;
            }

            return result;
        }

        int IOperationConfig.ReadSetting(IContext context, string key, int defaultValue)
        {
            int result = defaultValue;

            IOperationConfig operationConfig = (IOperationConfig)this;

            string settingText = operationConfig.ReadSetting(context, key, defaultValue.ToString());

            int newInt;
            if (int.TryParse(settingText, out newInt))
            {
                result = newInt;
            }

            return result;
        }


        Dictionary<string, string> IOperationConfig.GetSettings(IContext context)
        {
            Dictionary<string, string> settings = new Dictionary<string, string>();

            if (_settings == null)
            {
                _settings = ReadSettings(context);
            }

            if (_settings != null)
            {
                settings = _settings;
            }

            return settings;
        }

        private Dictionary<string, string> ReadSettings(IContext context)
        {
            try
            {
                IQ.Entities.VastDB.EntitySearch searchCriteria = new IQ.Entities.VastDB.EntitySearch();

                // Limit to the _configSourceId
                searchCriteria.Where = new Where(new Predicate(_configSourceKey, IQ.Entities.VastMetaDB.Enums.OperatorType.EqualTo, _configSourceId));

                string metabaseName;
                IStandardTable repo = IQ.BUS.Vast.Common.MetaDataHelper.ReadMetabaseName(context, _configSourceDatabaseName, out metabaseName);

                SearchResponse searchResponse = repo.Search(context, _configSourceInstanceGuid, metabaseName, string.Empty, string.Empty, _configSource, searchCriteria);

                if (searchResponse.RowList.Count > 0)
                {
                    _settings = new Dictionary<string, string>();

                    foreach (string[] row in searchResponse.RowList)
                    {
                        Dictionary<string, string> rowData = searchResponse.ReadRow(row);

                        string key = SearchResponse.SafeReadString(rowData, Columns.COL_GUID_SETTING_KEY); ;
                        string value = SearchResponse.SafeReadString(rowData, Columns.COL_GUID_SETTING_VALUE);

                        _settings.Add(key, value);
                    }
                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }

            return _settings;
        }


        double IOperationConfig.ReadSetting(IContext context, string key, double defaultValue)
        {
            double result = defaultValue;

            IOperationConfig operationConfig = (IOperationConfig)this;

            string settingText = operationConfig.ReadSetting(context, key, defaultValue.ToString());

            double newDouble;
            if (double.TryParse(settingText, out newDouble))
            {
                result = newDouble;
            }

            return result;
        }
    }
}
