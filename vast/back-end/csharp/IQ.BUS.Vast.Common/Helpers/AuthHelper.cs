using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using SAIL.Framework.Host;

using IQ.Entities.VastDB;
using IQ.Entities.VastDB.Const;
using IQ.RepositoryInterfaces.Vast;
using IQ.Entities.VastMetaDB.Enums;

namespace IQ.BUS.Vast.Common.Helpers
{
    public static class AuthHelper
    {
        private const string CHAR_PLUS = "+";
        private const string CHAR_PLUS_ESCAPED = "PLUS";

        public static string EncryptAuth(IContext context, string authTokenDataToEncrypt)
        {
            IStringCrypt crypt = context.Get<IStringCrypt>();

            string encryptedData = crypt.Encrypt(authTokenDataToEncrypt);

            // Escape Problem Chars
            encryptedData = encryptedData.Replace(CHAR_PLUS, CHAR_PLUS_ESCAPED);

            return encryptedData;
        }

        public static string DecryptAuth(IContext context, string authTokenDataToDecrypt)
        {
            IStringCrypt crypt = context.Get<IStringCrypt>();

            authTokenDataToDecrypt = authTokenDataToDecrypt.Replace(CHAR_PLUS_ESCAPED, CHAR_PLUS);

            string decryptedData = crypt.Decrypt(authTokenDataToDecrypt);

            return decryptedData;
        }

        /// <summary>
        /// Created:        08/10/2015
        /// Author:         David J. McKee
        /// Purpose:        Determines if the given table is accessable via public access.
        /// </summary>        
        public static string CheckForPublicAccess(IContext context,
                                                    System.Type rootType,
                                                    IConnectionHost connectionHost,
                                                    string actionType,
                                                    string formId)
        {
            string sysLoginId = string.Empty;

            try
            {
                // CRITICAL: WE MAY GET DUPLICATE VALUES

                // Search sysTableAuth
                // ID | New | Create | Read | Update | Delete | Search
                string domainName = ViewModelToDataModel.GetDomainFromQueryString(context, connectionHost);
                string accountCode = ViewModelToDataModel.GetAccountCodeFromQueryString(context, connectionHost, domainName);
                string instanceGUID = ViewModelToDataModel.BuildInstanceGUID(domainName, accountCode);
                string moduleId = ViewModelToDataModel.GetModuleFromQueryString(context, connectionHost);

                string customAssemblyLineName;
                string operationNameList;
                IQ.BUS.Vast.Common.CommonConcerns.InitAssemblyLineContext(context, actionType, out customAssemblyLineName, out operationNameList);

                string metabaseName;
                IStandardTable repo = IQ.BUS.Vast.Common.MetaDataHelper.ReadMetabaseName(context, moduleId, out metabaseName);

                Entity entity = repo.Read(context, instanceGUID, metabaseName, string.Empty, string.Empty, string.Empty, Tables.TABLE_GUID_SYS_TABLE_AUTH, Columns.COL_GUID_ID, formId);

                if (entity != null)
                {
                    string authTypeForAction = entity.GetColumnValue(actionType);

                    if (authTypeForAction == AuthorizationType.Public.ToString())
                    {
                        sysLoginId = KnownValues.KNOWN_VALUE_PUBLIC_USER;
                    }
                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }

            return sysLoginId;
        }

        /// <summary>
        /// Created:        08/10/2015
        /// Author:         David J. McKee
        /// Purpose:        Ensures only one response from a GET or POST
        /// </summary>        
        public static string RemoveDuplicat(string p)
        {
            string result = p;

            if (string.IsNullOrWhiteSpace(p) == false)
            {
                string[] parts = p.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                result = parts[0];
            }

            return result;
        }
    }
}
