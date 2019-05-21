using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using SAIL.Framework.Host;

using IQ.Entities.VastDB.Const;
using IQ.BUS.Vast.Common.Consts;

namespace IQ.BUS.Vast.Common.Helpers
{
    public static class ViewModelToDataModel
    {
        private const string KEY_DEFAULT_DOMAIN_NAME = "VastDefaultDomainName";
        private const string IQ_CLOUD_ACCOUNT_CODE = "444FreedomFromFutility";       // Master Account

        public const string DOMAIN_ACCOUNT_SEP = "-";

        public static string BuildInstanceGUID(string domainName, string accountCode)
        {
            StringBuilder instanceGUID = new StringBuilder();

            instanceGUID.Append(domainName.Trim().ToLower());

            if (string.IsNullOrWhiteSpace(accountCode) == false)
            {
                instanceGUID.Append(DOMAIN_ACCOUNT_SEP);
                instanceGUID.Append(accountCode.Trim().ToLower());
            }

            return instanceGUID.ToString();
        }

        /// <summary>
        /// Created:        06/09/2016
        /// Author:         David J. McKee
        /// Purpose:        Reads the domain name from the query string. Removes duplicates. Substitues a default domain if no domain is passed.
        /// Important:      This is the only method that should be used for reading a domain name
        /// </summary>        
        public static string GetDomainFromQueryString(IContext context, IConnectionHost connectionHost)
        {
            string domainName = string.Empty;

            try
            {
                if (connectionHost == null)
                {
                    domainName = context.GetByName(Entities.VastDB.Const.Context.DOMAIN_NAME).ToString();
                }

                if (connectionHost != null)
                {
                    domainName = CheckForEmpty(AuthHelper.RemoveDuplicat(connectionHost.RequestAny(IQ.ViewModel.Vast.Const.QueryString.DOMAIN_NAME)));
                }

                if (string.IsNullOrWhiteSpace(domainName))
                { 
                    domainName = context.Get<IHostConfiguration>().ReadSetting(KEY_DEFAULT_DOMAIN_NAME, Root.IQ_CLOUD_DOMAIN);
                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }

            return domainName;
        }

        public static string CheckForEmpty(string value)
        {
            const string TREAT_AS_EMPTY = "undefined";
            const string TREAT_AS_NULL = "null";

            if ((value == TREAT_AS_EMPTY) ||
                (value == TREAT_AS_NULL))
            {
                value = string.Empty;
            }

            return value;
        }

        /// <summary>
        /// Created:        08/10/2015
        /// Author:         David J. McKee
        /// Purpose:        Ensures only one response from a GET or POST
        /// </summary>        
        public static string RemoveDuplicates(string p)
        {
            string result = p;

            if (string.IsNullOrWhiteSpace(p) == false)
            {
                string[] parts = p.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                result = parts[0];
            }

            return result;
        }

        public static string GetAccountCodeFromQueryString(IContext context, IConnectionHost connectionHost)
        {
            string accountCode = string.Empty;

            try
            {
                if (connectionHost == null)
                {
                    accountCode = context.GetByName(Entities.VastDB.Const.Context.ACCOUNT_CODE).ToString();
                }

                if (connectionHost != null)
                {
                    accountCode = CheckForEmpty(RemoveDuplicates(connectionHost.RequestAny(Entities.VastDB.Const.QueryString.ACCOUNT_CODE)));
                }

                if (string.IsNullOrWhiteSpace(accountCode))
                {
                    if (context.GetByName(IQ.Entities.VastDB.Const.Context.DOMAIN_NAME) != null)
                    {
                        string domainName = context.GetByName(IQ.Entities.VastDB.Const.Context.DOMAIN_NAME).ToString();

                        if (domainName == KEY_DEFAULT_DOMAIN_NAME)
                        {
                            const string KEY_DEFAULT_ACCOUNT_CODE = "VastDefaultAccountCode";

                            accountCode = context.Get<IHostConfiguration>().ReadSetting(KEY_DEFAULT_ACCOUNT_CODE, IQ_CLOUD_ACCOUNT_CODE);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }

            return accountCode;
        }

        public static string GetAccountCodeFromQueryString(IContext context, IConnectionHost connectionHost, string domainName)
        {
            string accountCode = string.Empty;

            try
            {
                if (connectionHost == null)
                {
                    accountCode = context.GetByName(Entities.VastDB.Const.Context.ACCOUNT_CODE).ToString();
                }

                if (connectionHost != null)
                {
                    accountCode = CheckForEmpty(Helpers.AuthHelper.RemoveDuplicat(connectionHost.RequestAny(IQ.ViewModel.Vast.Const.QueryString.ACCOUNT_CODE)));
                }

                if (string.IsNullOrWhiteSpace(accountCode))
                {
                    if (domainName == Consts.Root.IQ_CLOUD_DOMAIN)
                    {
                        const string KEY_DEFAULT_ACCOUNT_CODE = "VastDefaultAccountCode";

                        accountCode = context.Get<IHostConfiguration>().ReadSetting(KEY_DEFAULT_ACCOUNT_CODE, Consts.Root.IQ_CLOUD_ACCOUNT_CODE);
                    }
                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }

            return accountCode;
        }

        public static string GetModuleFromQueryString(IContext context, IConnectionHost connectionHost)
        {
            string moduleId = string.Empty;

            try
            {
                if (connectionHost == null)
                {
                    moduleId = context.GetByName(Entities.VastDB.Const.Context.DATABASE_ID).ToString();
                }

                if (connectionHost != null)
                {
                    moduleId = CheckForEmpty(Helpers.AuthHelper.RemoveDuplicat(connectionHost.RequestAny(IQ.ViewModel.Vast.Const.QueryString.MODULE_ID)));
                }

                if (string.IsNullOrWhiteSpace(moduleId))
                {
                    const string KEY_DEFAULT_MODULE_ID = "VastDefaultModule";

                    moduleId = context.Get<IHostConfiguration>().ReadSetting(KEY_DEFAULT_MODULE_ID, string.Empty);
                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }

            return moduleId;
        }
    }
}
