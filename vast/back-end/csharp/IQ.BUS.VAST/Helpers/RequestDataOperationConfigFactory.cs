using IQ.RepositoryInterfaces.Vast;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using SAIL.Framework.Host;

using IQ.Entities.VastDB;

namespace IQ.BUS.Vast.Helpers
{
    /// <summary>
    /// Created:        12/20/2016
    /// Author:         David J. McKee
    /// Purpose:        In the event that an operation list, not an assembly line, is requested from the API, request parameters 
    ///                     may be used as operation config values.
    /// </summary>
    public class RequestDataOperationConfigFactory : IOperationConfigFactory, IOperationConfig
    {
        private Dictionary<string, string> _settings = new Dictionary<string, string>();    // Copied from entity on initial create.
        private Entity _entity = null;                                                      // Reference to entity (volatile memory)

        public RequestDataOperationConfigFactory()
        {

        }

        public RequestDataOperationConfigFactory(Entity settingEntity)
        {
            this.LoadFromEntity(settingEntity);
        }

        public void LoadFromEntity(Entity settingEntity)
        {
            // Important: We need to clone the values in case they are removed.
            foreach (string key in settingEntity.ColumnIdValuePair.Keys)
            {
                _settings[key] = settingEntity.ColumnIdValuePair[key];
            }

            _entity = settingEntity;
        }

        public void LoadFromConnectionHost(IConnectionHost connectionHost)
        {
            // Load settings from query string
            _settings = connectionHost.QueryString;

            // Load entity from form and header
            _entity = new Entity();
            _entity.ColumnIdValuePair = new Dictionary<string, string>();

            Dictionary<String, String> headers = connectionHost.Headers;

            foreach (string headerKey in headers.Keys)
            {
                _entity.ColumnIdValuePair[headerKey] = headers[headerKey];
            }

            Dictionary<String, String> form = connectionHost.Form;

            foreach (string formKey in form.Keys)
            {
                _entity.ColumnIdValuePair[formKey] = headers[formKey];
            }
        }

        IOperationConfig IOperationConfigFactory.AutoLocate(IContext context, string configSourceId)
        {
            return this;
        }

        IOperationConfig IOperationConfigFactory.GetConfig(string configSource, string configSourceKey, string configSourceID, string configSourceInstanceGuid, string configSourceDatabaseName)
        {
            return this;
        }

        Dictionary<string, string> IOperationConfig.GetSettings(IContext context)
        {
            return _settings;
        }

        double IOperationConfig.ReadSetting(IContext context, string key, double defaultValue)
        {
            double result = defaultValue;

            string value = ReadSetting(key);

            if (string.IsNullOrWhiteSpace(value) == false)
            {
                double newValue;
                if (double.TryParse(value, out newValue))
                {
                    result = newValue;
                }
            }

            return result;
        }

        int IOperationConfig.ReadSetting(IContext context, string key, int defaultValue)
        {
            int result = defaultValue;

            string value = ReadSetting(key);

            if (string.IsNullOrWhiteSpace(value) == false)
            {
                int newValue;
                if (int.TryParse(value, out newValue))
                {
                    result = newValue;
                }
            }

            return result;
        }

        bool IOperationConfig.ReadSetting(IContext context, string key, bool defaultValue)
        {
            bool result = defaultValue;

            string value = ReadSetting(key);

            if (string.IsNullOrWhiteSpace(value) == false)
            {
                bool newValue;
                if (bool.TryParse(value, out newValue))
                {
                    result = newValue;
                }
            }

            return result;
        }

        string IOperationConfig.ReadSetting(IContext context, string key, string defaultValue)
        {
            string result = defaultValue;

            string value = ReadSetting(key);

            if (string.IsNullOrWhiteSpace(value) == false)
            {
                result = value;
            }

            return result;
        }

        private string ReadSetting(string key)
        {
            string result = string.Empty;

            if (_settings != null)
            {
                if (_settings.ContainsKey(key))
                {
                    result = _settings[key];
                }
            }

            if (string.IsNullOrWhiteSpace(result))
            {
                if (_entity != null)
                {
                    if (_entity.ColumnIdValuePair.ContainsKey(key))
                    {
                        result = _entity.ColumnIdValuePair[key];
                    }
                }
            }

            return result;
        }
    }
}
