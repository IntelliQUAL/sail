using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using SAIL.Framework.Host;
using SAIL.Framework.Host.Enums;
using SAIL.Framework.Host.Bootstrap;
using SAIL.Framework.Host.BaseClasses;

using IQ.BUS.Vast.Helpers;
using IQ.Entities.VastDB.Const;
using IQ.BUS.Vast.Common.Helpers;
using IQ.RepositoryInterfaces.Vast;

namespace IQ.BUS.Vast.BaseClasses
{
    public abstract class REST : RepositoryBusinessProcessBase<IQ.ViewModel.Vast.Entity,
                                                                IQ.ViewModel.Vast.Entity,
                                                                IQ.ViewModel.Vast.Entity,
                                                                IQ.ViewModel.Vast.Entity,
                                                                IQ.ViewModel.Vast.Entity,
                                                                IQ.ViewModel.Vast.Entity,
                                                                IQ.ViewModel.Vast.EntitySearch,
                                                                IQ.ViewModel.Vast.Grid>
    {
        /// <summary>
        /// Created:        07/17/2015
        /// Author:         David J. McKee
        /// Purpose:        Authenticates the current user.      
        /// </summary>        
        public virtual bool AuthenticateUser(IContext context,
                                                IConnectionHost connectionHost,
                                                string actionType,
                                                IViewModelResult result,
                                                ViewModel.Vast.Entity requestForSecurityCheck,
                                                out string sysLoginId,
                                                out string authToken,
                                                out string logSetting)
        {
            bool isAuthenticated = false;

            sysLoginId = string.Empty;
            authToken = string.Empty;
            logSetting = string.Empty;

            try
            {
                if ((context.GetByName(Context.SYS_LOGIN_ID) == null) ||
                    (string.IsNullOrWhiteSpace(context.GetByName(Context.SYS_LOGIN_ID).ToString())))
                {
                    authToken = connectionHost.RequestAny(IQ.ViewModel.Vast.Const.QueryString.AUTH_TOKEN);

                    authToken = ViewModelToDataModel.CheckForEmpty(authToken);

                    string formId = GetFormID(context, connectionHost, result);

                    if (string.IsNullOrWhiteSpace(authToken))
                    {
                        // Check for Public Access                        
                        sysLoginId = AuthHelper.CheckForPublicAccess(context, this.GetType(), connectionHost, actionType, formId);
                    }
                    else
                    {
                        sysLoginId = ParseSysLoginID(context, authToken, out logSetting);
                        PerformSysLoginCheck(context, actionType, result, requestForSecurityCheck, ref sysLoginId, formId);
                    }

                    context.SetByName(Context.SYS_LOGIN_ID, sysLoginId);
                    context.SetByName(Context.LOG_SETTING, logSetting);
                }
                else
                {
                    sysLoginId = context.GetByName(Context.SYS_LOGIN_ID).ToString();
                }

                isAuthenticated = (string.IsNullOrWhiteSpace(sysLoginId) == false);
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }
            finally
            {
                context.SetByName(IQ.Entities.VastDB.Const.Context.CONTEXT_AUTH_TOKEN, authToken);
            }

            return isAuthenticated;
        }

        private void PerformSysLoginCheck(IContext context, string actionType, IViewModelResult result, ViewModel.Vast.Entity requestForSecurityCheck, ref string sysLoginId, string formId)
        {
            try
            {
                switch (formId)
                {
                    case Tables.TABLE_GUID_SYS_LOGIN:
                        sysLoginId = ProcessSysLoginTableSecurity(context, actionType, result, requestForSecurityCheck, sysLoginId);
                        break;

                    case Tables.TABLE_GUID_SYS_DATABASE:
                        sysLoginId = ProcessSysDatabaseTableSecurity(context, actionType, result, requestForSecurityCheck, sysLoginId);
                        break;
                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }
        }

        private string ProcessSysLoginTableSecurity(IContext context, string actionType, IViewModelResult result, ViewModel.Vast.Entity requestForSecurityCheck, string sysLoginId)
        {
            // Extra check for a person attempting to update the sysLogin table.
            string moduleId = GetModuleID(context, _connectionHost, result);

            if (moduleId == Databases.CAT_GUID_MASTER)
            {
                switch (actionType)
                {
                    case Actions.ACTION_NEW:
                    case Actions.ACTION_CREATE:
                        break;

                    case Actions.ACTION_READ:
                    case Actions.ACTION_DELETE:
                        if (requestForSecurityCheck != null)
                        {
                            if ((requestForSecurityCheck.PrimaryKey != sysLoginId) && (requestForSecurityCheck.PrimaryKey != "{" + Columns.COL_GUID_SYS_LOGIN_ID + "}"))
                            {
                                sysLoginId = string.Empty;
                            }
                        }
                        break;

                    case Actions.ACTION_UPDATE:
                        // Check to ensure the user is performing this action on their own account.
                        if ((requestForSecurityCheck != null) &&
                            (requestForSecurityCheck.FieldGroupList.Count > 0))
                        {
                            string localSysLoginId = requestForSecurityCheck.FieldValueByID(Columns.COL_GUID_SYS_LOGIN_ID);

                            // Check for actual sysLoginID or sysLoginID placeholder.
                            if ((localSysLoginId != sysLoginId) && (localSysLoginId != "{" + Columns.COL_GUID_SYS_LOGIN_ID + "}"))
                            {
                                sysLoginId = string.Empty;
                            }
                        }
                        break;

                    case Actions.ACTION_SEARCH:
                        sysLoginId = string.Empty;  // Never allow.
                        break;

                }
            }
            return sysLoginId;
        }

        private string ProcessSysDatabaseTableSecurity(IContext context,
                                                            string actionType,
                                                            IViewModelResult result,
                                                            ViewModel.Vast.Entity requestForSecurityCheck,
                                                            string sysLoginId)
        {
            // Extra check for a person attempting to update the sysLogin table.
            string moduleId = GetModuleID(context, _connectionHost, result);

            if (moduleId == Databases.CAT_GUID_MASTER)
            {
                switch (actionType)
                {
                    case Actions.ACTION_NEW:
                    case Actions.ACTION_CREATE:
                        break;

                    case Actions.ACTION_READ:
                    case Actions.ACTION_DELETE:
                    case Actions.ACTION_UPDATE:
                        if (requestForSecurityCheck != null)
                        {
                            // Search the sysLoginDatabase table for a match
                            IQ.Entities.VastDB.EntitySearch sysLoginDatabaseSearch = new IQ.Entities.VastDB.EntitySearch();

                            sysLoginDatabaseSearch.Where =
                                new IQ.Entities.VastMetaDB.SqlEsque.Where(
                                                                            new IQ.Entities.VastMetaDB.SqlEsque.Predicate(Columns.COL_GUID_SYS_DATABASE_ID, IQ.Entities.VastMetaDB.Enums.OperatorType.EqualTo, requestForSecurityCheck.PrimaryKey)
                                                                            );
                            sysLoginDatabaseSearch.Where.And = new IQ.Entities.VastMetaDB.SqlEsque.AND(
                                                                            new IQ.Entities.VastMetaDB.SqlEsque.Predicate(Columns.COL_GUID_SYS_LOGIN_ID, IQ.Entities.VastMetaDB.Enums.OperatorType.EqualTo, sysLoginId)
                                                                            );

                            IStandardTable repo = IQ.BUS.Vast.Common.CommonConcerns.InitStandardTableRepo(context, requestForSecurityCheck.DomainName);

                            EnsureDomainAndAccount(context, requestForSecurityCheck, context.Get<IConnectionHost>(), actionType);

                            string instanceGUID = ViewModelToDataModel.BuildInstanceGUID(requestForSecurityCheck.DomainName, requestForSecurityCheck.AccountCode);

                            IQ.Entities.VastDB.SearchResponse sysLoginDatabaseSearchResponse =
                                repo.Search(context, instanceGUID, Databases.CAT_GUID_MASTER, string.Empty, string.Empty, Tables.TABLE_GUID_SYS_LOGIN_DATABASE, sysLoginDatabaseSearch);

                            if ((sysLoginDatabaseSearchResponse == null) ||
                                (sysLoginDatabaseSearchResponse.RowList == null) ||
                                (sysLoginDatabaseSearchResponse.RowList.Count < 1))
                            {
                                sysLoginId = string.Empty;
                            }
                        }
                        break;

                    case Actions.ACTION_SEARCH:
                        sysLoginId = string.Empty;  // Never allow.
                        break;

                }
            }
            return sysLoginId;
        }

        /// <summary>
        /// Created:        07/19/2015
        /// Author:         David J. McKee
        /// Purpose:        Single location where ModuleID is read.
        /// </summary>        
        public virtual string GetModuleID(IContext context,
                                            IConnectionHost connectionHost,
                                            IViewModelResult result)
        {
            string moduleId = string.Empty;

            try
            {
                if (context.GetByName(Context.DATABASE_ID) == null)
                {
                    moduleId = ViewModelToDataModel.GetModuleFromQueryString(context, _connectionHost);
                    context.SetByName(Context.DATABASE_ID, moduleId);
                }
                else
                {
                    moduleId = (context.GetByName(Context.DATABASE_ID).ToString());
                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }

            return moduleId;
        }

        /// <summary>
        /// Created:        07/182015
        /// Author:         David J. McKee
        /// Purpose:        Single location where FormID is read.
        /// </summary>        
        public virtual string GetFormID(IContext context,
                                        IConnectionHost connectionHost,
                                        IViewModelResult result)
        {
            string formId = string.Empty;

            try
            {
                if (context.GetByName(Context.TABLE_ID) == null)
                {
                    formId = connectionHost.RequestAny(IQ.ViewModel.Vast.Const.QueryString.FORM_ID);
                    formId = AuthHelper.RemoveDuplicat(formId);
                    context.SetByName(Context.TABLE_ID, formId);
                }
                else
                {
                    formId = context.GetByName(Context.TABLE_ID).ToString();
                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }

            return formId;
        }

        /// <summary>
        /// Created:        09/29/2016
        /// Author:         David J. McKee
        /// Purpose:        Reads the custom assembly line name or allows an override.
        /// </summary>
        public virtual string GetCustomAssemblyLineName(IContext context,
                                                        IConnectionHost connectionHost,
                                                        string actionType)
        {
            string customAssemblyLineName = string.Empty;

            if (context.GetByName(Context.CUSTOM_ASSEMBLY_LINE_NAME) == null)
            {
                customAssemblyLineName = connectionHost.RequestAny(IQ.ViewModel.Vast.Const.QueryString.CUSTOM_DATA_PIPELINE);
                context.SetByName(Context.CUSTOM_ASSEMBLY_LINE_NAME, customAssemblyLineName);
            }

            return customAssemblyLineName;
        }

        /// <summary>
        /// Created:        10/13/2016
        /// Author:         David J. McKee
        /// Purpose:        Reads a list of operations or allows an override.
        /// </summary>
        public virtual string GetOperationNameList(IContext context,
                                                        IConnectionHost connectionHost,
                                                        string actionType)
        {
            string operationNameList = string.Empty;

            if (context.GetByName(Context.OPERATION_NAME_LIST) == null)
            {
                operationNameList = connectionHost.RequestAny(IQ.ViewModel.Vast.Const.QueryString.OPERATION_LIST);
                context.SetByName(Context.OPERATION_NAME_LIST, operationNameList);
            }

            return operationNameList;
        }

        /// <summary>
        /// Created:        11/12/2016
        /// Author:         David J. McKee
        /// Purpose:        Gets the account code from the request
        /// </summary>
        /// <param name="context"></param>
        /// <param name="connectionHost"></param>
        /// <param name="actionType"></param>
        /// <param name="readId">PrimaryKey from Read Request</param>
        /// <returns></returns>
        public virtual string GetAccountCode(IContext context,
                                                IConnectionHost connectionHost,
                                                string actionType,
                                                string primaryKey)
        {
            return ViewModelToDataModel.GetAccountCodeFromQueryString(context, connectionHost);
        }

        /// <summary>
        /// Created:        01/30/2015
        /// Author:         David J. McKee
        /// Purpose:        NEW - Returns an Empty view model which represents a valid request to ProcessCreateRequest     
        /// </summary>        
        public override ViewModel.Vast.Entity ProcessNewRequest(IContext context)
        {
            ViewModel.Vast.Entity result = new ViewModel.Vast.Entity();

            try
            {
                string actionType = IQ.Entities.VastDB.Const.Actions.ACTION_NEW;

                string sysLoginId;
                string authToken;
                string logSetting;

                if (AuthenticateUser(context, _connectionHost, actionType, result, null, out sysLoginId, out authToken, out logSetting))
                {
                    string accountCode = GetAccountCode(context, _connectionHost, actionType, string.Empty);
                    string moduleId = GetModuleID(context, _connectionHost, result);
                    string formId = GetFormID(context, _connectionHost, result);

                    result = ExecuteBasicAction(context, sysLoginId, accountCode, moduleId, formId, actionType, string.Empty, logSetting,
                                                new IQ.BUS.VAST.AssemblyLines.New(), ref _connectionHost);
                }
                else
                {
                    result.Result.Success = false;
                    result.Result.ErrorCode = IQ.Entities.VastDB.Const.Errors.ERROR_CODE_AUTHENTICATION_FAILED;
                    result.Result.ErrorText = IQ.Entities.VastDB.Const.Errors.ERROR_TEXT_AUTHENTICATION_FAILED;
                    result.Result.UserMessage = IQ.Entities.VastDB.Const.Errors.USER_MESSAGE_AUTHENTICATION_FAILED;
                }

                result.AuthToken = authToken;
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }

            return result;
        }

        /// <summary>
        /// Created:        02/26/2015
        /// Author:         David J. McKee
        /// Purpose:        CREATE - Inserts a new record into the database.
        /// </summary>       
        public override ViewModel.Vast.Entity ProcessCreateRequest(IContext context, ViewModel.Vast.Entity request)
        {
            ViewModel.Vast.Entity result = new ViewModel.Vast.Entity();

            try
            {
                string actionType = IQ.Entities.VastDB.Const.Actions.ACTION_CREATE;

                string sysLoginId;
                string authToken;
                string logSetting;

                if (AuthenticateUser(context, _connectionHost, actionType, result, request, out sysLoginId, out authToken, out logSetting))
                {
                    string accountCode = GetAccountCode(context, _connectionHost, actionType, string.Empty);
                    string moduleId = GetModuleID(context, _connectionHost, result);
                    string formId = GetFormID(context, _connectionHost, result);

                    result = ExecuteCreateOrUpdate(context, sysLoginId, accountCode, moduleId, formId, _connectionHost, request, actionType, logSetting,
                                                    new IQ.BUS.VAST.AssemblyLines.Create());

                    if (string.IsNullOrWhiteSpace(result.AuthToken))
                    {
                        result.AuthToken = authToken;
                    }
                }
                else
                {
                    result.Result.Success = false;
                    result.Result.ErrorCode = IQ.Entities.VastDB.Const.Errors.ERROR_CODE_AUTHENTICATION_FAILED;
                    result.Result.ErrorText = IQ.Entities.VastDB.Const.Errors.ERROR_TEXT_AUTHENTICATION_FAILED;
                    result.Result.UserMessage = IQ.Entities.VastDB.Const.Errors.USER_MESSAGE_AUTHENTICATION_FAILED;
                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }

            return result;
        }

        /// <summary>
        /// Created:        02/26/2015
        /// Author:         David J. McKee
        /// Purpose:        READ - Reads a single row
        /// </summary>      
        public override ViewModel.Vast.Entity ProcessReadRequest(IContext context, string id)
        {
            ViewModel.Vast.Entity result = new ViewModel.Vast.Entity();

            try
            {
                string actionType = IQ.Entities.VastDB.Const.Actions.ACTION_READ;

                string sysLoginId;
                string authToken;
                string logSetting;

                ViewModel.Vast.Entity requestForSecurityCheck = new ViewModel.Vast.Entity();
                requestForSecurityCheck.PrimaryKey = id;

                if (AuthenticateUser(context, _connectionHost, actionType, result, requestForSecurityCheck, out sysLoginId, out authToken, out logSetting))
                {
                    string accountCode = GetAccountCode(context, _connectionHost, actionType, id);
                    string moduleId = GetModuleID(context, _connectionHost, result);
                    string formId = GetFormID(context, _connectionHost, result);

                    result = ExecuteBasicAction(context, sysLoginId, accountCode, moduleId, formId, actionType, id, logSetting,
                                                new IQ.BUS.VAST.AssemblyLines.Read(), ref _connectionHost);

                    result.AuthToken = authToken;
                }
                else
                {
                    result.Result.Success = false;
                    result.Result.ErrorCode = IQ.Entities.VastDB.Const.Errors.ERROR_CODE_AUTHENTICATION_FAILED;
                    result.Result.ErrorText = IQ.Entities.VastDB.Const.Errors.ERROR_TEXT_AUTHENTICATION_FAILED;
                    result.Result.UserMessage = IQ.Entities.VastDB.Const.Errors.USER_MESSAGE_AUTHENTICATION_FAILED;
                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }

            return result;
        }

        /// <summary>
        /// Created:        02/26/2015
        /// Author:         David J. McKee
        /// Purpose:        UPDATE - Updates an existing row
        /// </summary>        
        public override ViewModel.Vast.Entity ProcessUpdateRequest(IContext context, ViewModel.Vast.Entity request)
        {
            ViewModel.Vast.Entity result = new ViewModel.Vast.Entity();

            try
            {
                string actionType = IQ.Entities.VastDB.Const.Actions.ACTION_UPDATE;

                string sysLoginId;
                string authToken;
                string logSetting;

                if (AuthenticateUser(context, _connectionHost, actionType, result, request, out sysLoginId, out authToken, out logSetting))
                {
                    string accountCode = GetAccountCode(context, _connectionHost, actionType, request.PrimaryKey);
                    string moduleId = GetModuleID(context, _connectionHost, result);
                    string formId = GetFormID(context, _connectionHost, result);

                    result = ExecuteCreateOrUpdate(context,
                                                    sysLoginId, accountCode, moduleId, formId, _connectionHost, request, actionType, logSetting,
                                                    new IQ.BUS.VAST.AssemblyLines.Update());

                    result.AuthToken = authToken;
                }
                else
                {
                    result.Result.Success = false;
                    result.Result.ErrorCode = IQ.Entities.VastDB.Const.Errors.ERROR_CODE_AUTHENTICATION_FAILED;
                    result.Result.ErrorText = IQ.Entities.VastDB.Const.Errors.ERROR_TEXT_AUTHENTICATION_FAILED;
                    result.Result.UserMessage = IQ.Entities.VastDB.Const.Errors.USER_MESSAGE_AUTHENTICATION_FAILED;
                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }

            return result;
        }

        /// <summary>
        /// Created:        02/26/2015
        /// Author:         David J. McKee
        /// Purpose:        DELETE - Deletes an existing row
        /// </summary>
        public override ViewModel.Vast.Entity ProcessDeleteRequest(IContext context, string id)
        {
            ViewModel.Vast.Entity result = new ViewModel.Vast.Entity();

            try
            {
                string actionType = IQ.Entities.VastDB.Const.Actions.ACTION_DELETE;

                string sysLoginId;
                string authToken;
                string logSetting;

                id = ViewModelToDataModel.CheckForEmpty(id);

                if (string.IsNullOrWhiteSpace(id) == false)
                {
                    ViewModel.Vast.Entity requestForSecurityCheck = new ViewModel.Vast.Entity();

                    requestForSecurityCheck.PrimaryKey = id;

                    if (AuthenticateUser(context, _connectionHost, actionType, result, requestForSecurityCheck, out sysLoginId, out authToken, out logSetting))
                    {
                        string accountCode = GetAccountCode(context, _connectionHost, actionType, id);
                        string moduleId = GetModuleID(context, _connectionHost, result);
                        string formId = GetFormID(context, _connectionHost, result);

                        result = ExecuteBasicAction(context, sysLoginId, accountCode, moduleId, formId, actionType, id, logSetting,
                                                    new IQ.BUS.VAST.AssemblyLines.Delete(), ref _connectionHost);
                    }
                    else
                    {
                        result.Result.Success = false;
                        result.Result.ErrorCode = IQ.Entities.VastDB.Const.Errors.ERROR_CODE_AUTHENTICATION_FAILED;
                        result.Result.ErrorText = IQ.Entities.VastDB.Const.Errors.ERROR_TEXT_AUTHENTICATION_FAILED;
                        result.Result.UserMessage = IQ.Entities.VastDB.Const.Errors.USER_MESSAGE_AUTHENTICATION_FAILED;
                    }

                    result.AuthToken = authToken;
                }

            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }

            return result;
        }

        /// <summary>
        /// Created:        02/26/2015
        /// Author:         David J. McKee
        /// Purpose:        Search All - Returns multiple rows
        /// </summary>
        public override ViewModel.Vast.Grid ProcessSearchAllRequest(IContext context)
        {
            ViewModel.Vast.Grid result = new ViewModel.Vast.Grid();

            try
            {
                string actionType = IQ.Entities.VastDB.Const.Actions.ACTION_SEARCH;

                string sysLoginId;
                string authToken;
                string logSetting;

                if (AuthenticateUser(context, _connectionHost, actionType, result, null, out sysLoginId, out authToken, out logSetting))
                {
                    string accountCode = GetAccountCode(context, _connectionHost, actionType, string.Empty);
                    string moduleId = GetModuleID(context, _connectionHost, result);
                    string formId = GetFormID(context, _connectionHost, result);

                    result = ExecuteSearch(context, sysLoginId, accountCode, moduleId, formId, logSetting, null);
                }
                else
                {
                    result.Result.Success = false;
                    result.Result.ErrorCode = IQ.Entities.VastDB.Const.Errors.ERROR_CODE_AUTHENTICATION_FAILED;
                    result.Result.ErrorText = IQ.Entities.VastDB.Const.Errors.ERROR_TEXT_AUTHENTICATION_FAILED;
                    result.Result.UserMessage = IQ.Entities.VastDB.Const.Errors.USER_MESSAGE_AUTHENTICATION_FAILED;
                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }

            return result;
        }

        /// <summary>
        /// Created:        02/26/2015
        /// Author:         David J. McKee
        /// Purpose:        Search - Returns multiple rows
        /// </summary>
        public override ViewModel.Vast.Grid ProcessSearchRequest(IContext context, ViewModel.Vast.EntitySearch request)
        {
            ViewModel.Vast.Grid result = new ViewModel.Vast.Grid();

            try
            {
                string actionType = IQ.Entities.VastDB.Const.Actions.ACTION_SEARCH;

                string sysLoginId;
                string authToken;
                string logSetting;

                if (AuthenticateUser(context, _connectionHost, actionType, result, null, out sysLoginId, out authToken, out logSetting))
                {
                    string accountCode = GetAccountCode(context, _connectionHost, actionType, string.Empty);
                    string moduleId = GetModuleID(context, _connectionHost, result);
                    string formId = GetFormID(context, _connectionHost, result);

                    result = ExecuteSearch(context, sysLoginId, accountCode, moduleId, formId, logSetting, request);
                }
                else
                {
                    result.Result.Success = false;
                    result.Result.ErrorCode = IQ.Entities.VastDB.Const.Errors.ERROR_CODE_AUTHENTICATION_FAILED;
                    result.Result.ErrorText = IQ.Entities.VastDB.Const.Errors.ERROR_TEXT_AUTHENTICATION_FAILED;
                    result.Result.UserMessage = IQ.Entities.VastDB.Const.Errors.USER_MESSAGE_AUTHENTICATION_FAILED;
                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }

            return result;
        }

        /// <summary>
        /// Created:        06/29/2015
        /// Author:         David J. McKee
        /// Purpose:        Extracts the sysLoginID from the given AuthToken
        /// </summary>        
        internal static string ParseSysLoginID(IContext context, string authToken, out string logSetting)
        {
            string sysLoginId = string.Empty;
            logSetting = string.Empty;

            try
            {
                if (string.IsNullOrWhiteSpace(authToken) == false)
                {
                    string encodedData = AuthHelper.DecryptAuth(context, authToken);

                    ICsvIO csvIo = context.Get<ICsvIO>();

                    string[] columnPositionMap = new string[] { Columns.COL_GUID_PASSWORD_HASH, IQ.BUS.Vast.Common.Consts.Params.AUTH_TOKEN_TIMEOUT, Columns.COL_GUID_SYS_LOGIN_ID, Columns.COL_GUID_LOG_SETTING };

                    Dictionary<string, string> data = csvIo.ParseCsvLineSingle(encodedData, columnPositionMap);

                    sysLoginId = data[Columns.COL_GUID_SYS_LOGIN_ID];

                    if (data.ContainsKey(Columns.COL_GUID_LOG_SETTING))
                    {
                        logSetting = data[Columns.COL_GUID_LOG_SETTING];
                    }
                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex, "authToken: " + authToken);
            }

            return sysLoginId;
        }

        /// <summary>
        /// Created:        01/30/2015
        /// Author:         David J. McKee
        /// Purpose:        
        ///                 Sets the Instance, Schema, Table, Parent Table, and Parent Table Primary Key into the context
        /// </summary>        
        private void SetContextValues(IContext context,
                                                string accountCode,
                                                string moduleId,
                                                string logSetting,
                                                string actionType)
        {
            accountCode = IQ.BUS.Vast.Common.Helpers.AuthHelper.RemoveDuplicat(accountCode);
            moduleId = IQ.BUS.Vast.Common.Helpers.AuthHelper.RemoveDuplicat(moduleId);

            IConnectionHost connectionHost = context.Get<IConnectionHost>();

            string instanceGUID = string.Empty;
            string instanceStatus = string.Empty;

            if (context.GetByName(Context.INSTANCE_GUID) == null)
            {
                string domainName = ViewModelToDataModel.GetDomainFromQueryString(context, connectionHost);

                context.SetByName(Context.DOMAIN_NAME, domainName);

                instanceGUID = ViewModelToDataModel.BuildInstanceGUID(domainName, accountCode);

                context.SetByName(Context.INSTANCE_GUID, instanceGUID);
            }
            else
            {
                instanceGUID = context.GetByName(Context.INSTANCE_GUID).ToString();
            }

            // Account Code
            if (context.GetByName(Context.ACCOUNT_CODE) == null)
            {
                context.SetByName(Context.ACCOUNT_CODE, accountCode);
            }

            if (context.GetByName(Context.INSTANCE_STATUS) == null)
            {
                instanceStatus = connectionHost.RequestAny(IQ.ViewModel.Vast.Const.QueryString.ACCOUNT_STATUS);

                if (string.IsNullOrWhiteSpace(instanceStatus))
                {
                    // Assume Prod
                    instanceStatus = IQ.ViewModel.Vast.Enums.InstanceStatus.Prod.ToString();
                }

                context.SetByName(Context.INSTANCE_STATUS, instanceStatus);
            }
            else
            {
                instanceStatus = context.GetByName(Context.INSTANCE_STATUS).ToString();
            }

            context.Set(new WebsiteReader(instanceGUID, instanceStatus));

            // View Model: Module = Database
            if (context.GetByName(Context.DATABASE_ID) == null)
            {
                context.SetByName(Context.DATABASE_ID, moduleId);
            }

            // Custom Assembly Line
            if (context.GetByName(Context.CUSTOM_ASSEMBLY_LINE_NAME) == null)
            {
                string customAssemblyLineName = this.GetCustomAssemblyLineName(context, connectionHost, actionType);
                context.SetByName(Context.CUSTOM_ASSEMBLY_LINE_NAME, customAssemblyLineName);
            }

            // Operation List
            if (context.GetByName(Context.OPERATION_NAME_LIST) == null)
            {
                string operationNameList = this.GetOperationNameList(context, connectionHost, actionType);
                context.SetByName(Context.OPERATION_NAME_LIST, operationNameList);
            }

            if (context.GetByName(Context.LOG_SETTING) == null)
            {
                context.SetByName(Context.LOG_SETTING, logSetting);
            }
        }

        /// <summary>
        /// Created:        07/17/2015
        /// Author:         David J. McKee
        /// Purpose:        Executes a search requ4est
        /// </summary>        
        internal ViewModel.Vast.Grid ExecuteSearch(IContext context,
                                                    string sysLoginId,
                                                    string accountCode,
                                                    string moduleId,
                                                    string formId,
                                                    string logSetting,
                                                    ViewModel.Vast.EntitySearch request)
        {
            ViewModel.Vast.Grid response = new ViewModel.Vast.Grid();

            DateTime startTime = DateTime.Now;

            try
            {
                // Trace the entry
                // TraceEntry(context, sysLoginId, formId, Actions.ACTION_SEARCH, logSetting);

                if (request == null)
                {
                    // SearchAll
                    request = new ViewModel.Vast.EntitySearch();
                }

                SetContextValues(context, accountCode, moduleId, logSetting, IQ.Entities.VastDB.Const.Actions.ACTION_SEARCH);

                // Convert the View Model Entity Request into a Data Model Entity Request
                IQ.Entities.VastDB.EntitySearch dataModelEntity = new IQ.Entities.VastDB.EntitySearch();

                IQ.BUS.Vast.Common.SearchHelper.ViewModelEntitySearchFromDbEntity(context, sysLoginId, formId, request, dataModelEntity);

                // Create a new Instance of the Assembly Line
                IAssemblyLine<IQ.Entities.VastDB.EntitySearch, IQ.Entities.VastDB.SearchResponse> newAssemblyLine = new IQ.BUS.VAST.AssemblyLines.Search();

                // Create the Assembly Line Context
                FlowTransport<IQ.Entities.VastDB.EntitySearch> alContext = new FlowTransport<IQ.Entities.VastDB.EntitySearch>(dataModelEntity, context);

                // Execute the Assembly Line
                IQ.Entities.VastDB.SearchResponse searchResponse = newAssemblyLine.Execute(alContext);

                // Convert the Data Model Response into a View Model Response
                response = IQ.BUS.Vast.Common.SearchHelper.ViewModelGridFromDbEntity(context, request, searchResponse);
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }
            finally
            {
                TraceExit(context, Actions.ACTION_SEARCH, formId, "N/A", Actions.ACTION_SEARCH, formId, "N/A", sysLoginId, logSetting, string.Empty, startTime);
            }

            return response;
        }

        /// <summary>
        /// Created:        07/17/2015
        /// Author:         David J. McKee
        /// Purpose:        Ensures the domain and account code are present.
        /// </summary>        
        private void EnsureDomainAndAccount(IContext context,
                                            ViewModel.Vast.Entity request,
                                            IConnectionHost connectionHost,
                                            string actionType)
        {
            EnsureDomain(context, request, connectionHost);
            EnsureAccountCode(context, request, connectionHost, actionType);
        }

        /// <summary>
        /// Created:        06/09/2016
        /// Author:         David J. McKee
        /// Purpose:        Ensures that the DomainName is set.
        /// Important:      If no domain name can be found, a default domain name may be returned.
        /// </summary>        
        private static void EnsureDomain(IContext context, ViewModel.Vast.Entity request, IConnectionHost connectionHost)
        {
            if (string.IsNullOrWhiteSpace(request.DomainName))
            {
                request.DomainName = ViewModelToDataModel.GetDomainFromQueryString(context, connectionHost);
            }

            if (context.GetByName(IQ.Entities.VastDB.Const.Context.DOMAIN_NAME) == null)
            {
                context.SetByName(IQ.Entities.VastDB.Const.Context.DOMAIN_NAME, request.DomainName);
            }
        }

        private void EnsureAccountCode(IContext context, ViewModel.Vast.Entity request, IConnectionHost connectionHost, string actionType)
        {
            if (string.IsNullOrWhiteSpace(request.AccountCode))
            {
                request.AccountCode = GetAccountCode(context, connectionHost, actionType, string.Empty);
            }

            if (context.GetByName(IQ.Entities.VastDB.Const.Context.ACCOUNT_CODE) == null)
            {
                context.SetByName(IQ.Entities.VastDB.Const.Context.ACCOUNT_CODE, request.AccountCode);
            }
        }

        /// <summary>
        /// Created:        06/09/2015
        /// Author:         David J. McKee
        /// Purpose:        Copies all QueryString parameters into the request.
        /// </summary>        
        private static void AppendQueryString(IContext context, ViewModel.Vast.Entity request,
                                                IConnectionHost connectionHost)
        {
            try
            {
                if (connectionHost != null)
                {
                    Dictionary<string, string> queryString = connectionHost.QueryString;

                    foreach (string queryStringParamKey in queryString.Keys)
                    {
                        ViewModel.Vast.Field column = request.FieldByGuid(queryStringParamKey);

                        if (column == null)
                        {
                            // Ensure Column Group List
                            if (request.FieldGroupList == null)
                            {
                                request.FieldGroupList = new List<ViewModel.Vast.FieldGroup>();
                            }

                            if (request.FieldGroupList.Count == 0)
                            {
                                request.FieldGroupList.Add(new ViewModel.Vast.FieldGroup());
                            }

                            // Ensure Column Row List
                            if (request.FieldGroupList[0].FieldRowList == null)
                            {
                                request.FieldGroupList[0].FieldRowList = new List<ViewModel.Vast.FieldRow>();
                            }

                            if (request.FieldGroupList[0].FieldRowList.Count == 0)
                            {
                                request.FieldGroupList[0].FieldRowList.Add(new ViewModel.Vast.FieldRow());
                            }

                            // Ensure Column List
                            if (request.FieldGroupList[0].FieldRowList[0].FieldList == null)
                            {
                                request.FieldGroupList[0].FieldRowList[0].FieldList = new List<ViewModel.Vast.Field>();
                            }

                            if (request.FieldGroupList[0].FieldRowList[0].FieldList.Count == 0)
                            {
                                request.FieldGroupList[0].FieldRowList[0].FieldList.Add(new ViewModel.Vast.Field());
                            }

                            // Add the Column                                                
                            column = new ViewModel.Vast.Field();

                            column.ID = queryStringParamKey;
                            column.Value = queryString[queryStringParamKey];

                            request.FieldGroupList[0].FieldRowList[0].FieldList.Add(column);
                        }
                        else
                        {
                            column.Value = queryString[queryStringParamKey];
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }
        }

        /// <summary>
        /// Created:        07/17/2015
        /// Author:         David J. McKee
        /// Purpose:        Creates a basic request from the informaiton provided.
        /// </summary>        
        private IQ.Entities.VastDB.Entity CreateBasicRequest(IContext context,
                                                                        IConnectionHost connectionHost,
                                                                        string formId,
                                                                        string actionType,
                                                                        string primaryKey,
                                                                        string sysLoginId,
                                                                        out ViewModel.Vast.Entity request)
        {
            request = new ViewModel.Vast.Entity();
            request.PrimaryKey = primaryKey;
            request.FormID = formId;

            request.FormID = formId;
            request.ActionResponseType = actionType;
            EnsureDomainAndAccount(context, request, connectionHost, actionType);
            AppendQueryString(context, request, connectionHost);

            // Convert the View Model Entity Request into a Data Model Entity Request
            IQ.Entities.VastDB.Entity dataModelEntity = IQ.BUS.Vast.Common.Helpers.EntitiesHelper.BuildDataModelEntity(context, request, sysLoginId, formId);
            dataModelEntity.sysLoginID = sysLoginId;

            return dataModelEntity;
        }

        /// <summary>
        /// Created:        07/17/2015
        /// Author:         David J. McKee
        /// Purpose:        Common method to perform basic CRUB operations.
        /// </summary>        
        internal ViewModel.Vast.Entity ExecuteBasicAction(IContext context,
                                                                    string sysLoginId,
                                                                    string accountCode,
                                                                    string moduleId,
                                                                    string formId,
                                                                    string actionType,
                                                                    string id,
                                                                    string logSetting,
                                                                    IAssemblyLine<IQ.Entities.VastDB.Entity, IQ.Entities.VastDB.Entity> assemblyLine,
                                                                    ref IConnectionHost connectionHost)
        {
            ViewModel.Vast.Entity response = new ViewModel.Vast.Entity();

            DateTime startTime = DateTime.Now;

            try
            {
                // Required for Embedded Uploads
                if (connectionHost == null)
                {
                    connectionHost = context.Get<IConnectionHost>();
                }

                SetContextValues(context, accountCode, moduleId, logSetting, actionType);

                // Trace the input
                //TraceEntry(context, sysLoginId, formId, actionType, logSetting);

                // Create a request entity from basic information.
                ViewModel.Vast.Entity request;
                IQ.Entities.VastDB.Entity dataModelEntity = CreateBasicRequest(context, connectionHost, formId, actionType, id, sysLoginId, out request);

                // Execute the AssemblyLine                
                FlowTransport<IQ.Entities.VastDB.Entity> alContext = new FlowTransport<IQ.Entities.VastDB.Entity>(dataModelEntity, context);
                IQ.Entities.VastDB.Entity entity = assemblyLine.Execute(alContext);

                // Convert the New Response into a response view model.                
                IQ.BUS.Vast.Common.DataHelper.ViewModelEntityFromDbEntity(context, request, entity, response);
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }
            finally
            {
                // Exceptions here have been a problem in the past.
                try
                {
                    // Trace the output
                    TraceExit(context, actionType, formId, id, response.ActionResponseType, response.FormID, response.PrimaryKey, sysLoginId, logSetting, response.TransactionID, startTime);
                }
                catch (System.Exception ex2)
                {
                    Console.WriteLine(ex2.ToString());
                }
            }

            return response;
        }

        private static void TraceEntry(IContext context,
                                        string sysLoginId,
                                        string formId,
                                        string actionType,
                                        string logSetting)
        {
            try
            {
                string instanceGUID = context.GetByName(Context.INSTANCE_GUID).ToString();
                string databaseId = context.GetByName(Context.DATABASE_ID).ToString();

                string message = "BEGIN - " + Context.INSTANCE_GUID + ": " + instanceGUID + ", " +
                                    Context.DATABASE_ID + ": " + databaseId + ", " +
                                    Columns.COL_GUID_FORM_ID + ": " + formId + ": " +
                                    Columns.COL_GUID_ACTION_TYPE + ": " + actionType + ", " +
                                    Columns.COL_GUID_SYS_LOGIN_ID + ": " + sysLoginId + ", " +
                                    Columns.COL_GUID_LOG_SETTING + ": " + logSetting;
                //Columns.COL_GUID_TRANSACTION_ID + ": " + transactionId;

                context.Get<ITrace>().Emit(System.Diagnostics.TraceLevel.Info, message);
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }
        }

        private static void TraceExit(IContext context,
                                        string requestActionType,
                                        string requestFormId,
                                        string requestPrimaryKey,
                                        string responseActionType,
                                        string responseFormId,
                                        string responsePrimaryKey,
                                        string sysLoginId,
                                        string logSetting,
                                        string responseTransactionId,
                                        DateTime startTime)
        {
            try
            {
                string instanceGUID = context.GetByName(Context.INSTANCE_GUID).ToString();
                string databaseId = context.GetByName(Context.DATABASE_ID).ToString();

                TimeSpan totalTime = DateTime.Now.Subtract(startTime);

                string message = "Request" + Columns.COL_GUID_ACTION_TYPE + ": " + requestActionType + ", " +
                                    "Request" + Columns.COL_GUID_FORM_ID + ": " + requestFormId + ": " +
                                    "Request" + Columns.COL_GUID_PRIMARY_KEY + ": " + requestPrimaryKey + ": " +

                                    "Response" + Columns.COL_GUID_ACTION_TYPE + ": " + responseActionType + ", " +
                                    "Response" + Columns.COL_GUID_FORM_ID + ": " + responseFormId + ": " +
                                    "Response" + Columns.COL_GUID_PRIMARY_KEY + ": " + responsePrimaryKey + ": " +

                                    Context.INSTANCE_GUID + ": " + instanceGUID + ", " +
                                    Context.DATABASE_ID + ": " + databaseId + ", " +

                                    Columns.COL_GUID_SYS_LOGIN_ID + ": " + sysLoginId + ", " +
                                    Columns.COL_GUID_LOG_SETTING + ": " + logSetting + ", " +
                                    Columns.COL_GUID_TRANSACTION_ID + ": " + responseTransactionId + ", " +

                                    "Total Time: " + totalTime.TotalMilliseconds + "ms";

                context.Get<ITrace>().Emit(System.Diagnostics.TraceLevel.Info, message);
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }
        }

        /// <summary>
        /// Created:        07/17/2015
        /// Author:         David J. McKee
        /// Purpose:        Builds a single cache key from the entity
        /// </summary>
        /// <param name="context"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        private static string BuildCacheKey(IContext context, ViewModel.Vast.Entity request)
        {
            return "SubmissionDataKey-" + request.TransactionID;
        }

        /// <summary>
        /// Created:        07/17/2015
        /// Author:         David J. McKee
        /// Purpose:        Builds a session id to be used by the session manager.
        ///                 Since each transaction also has a unique id, we just use the instance guid for this.
        /// </summary>        
        private static string BuildIqSession(IContext context)
        {
            string instanceGUID = context.GetByName(Context.INSTANCE_GUID).ToString();

            return instanceGUID;
        }

        /// <summary>
        /// Created:        07/17/2015
        /// Author:         David J. McKee
        /// Purpose:        Buffers all data from an immediate submission post-back.
        /// </summary>        
        private static void BufferImmediateSubmission(IContext context, ViewModel.Vast.Entity request)
        {
            string immediateSubmissionDataKey = BuildCacheKey(context, request);

            string iqSessionId = BuildIqSession(context);

            ISessionManager sessionManager = context.Get<IServiceLocator>().Locate<ISessionManager>(context, iqSessionId);

            string entityJson = sessionManager.ReadSessionValue(immediateSubmissionDataKey, string.Empty);

            ViewModel.Vast.Entity baseEntity = null;

            if (string.IsNullOrWhiteSpace(entityJson))
            {
                baseEntity = new ViewModel.Vast.Entity();
            }
            else
            {
                baseEntity = (ViewModel.Vast.Entity)context.Get<IRequestHelper>().StringToObject<ViewModel.Vast.Entity>(entityJson, ResponseFormat.JSON);
            }

            foreach (var columnGroup in request.FieldGroupList)
            {
                if (baseEntity.FieldGroupList == null)
                {
                    baseEntity.FieldGroupList = new List<ViewModel.Vast.FieldGroup>();
                }

                baseEntity.FieldGroupList.Add(columnGroup);
            }

            if (context.GetByName(Context.CUSTOM_ASSEMBLY_LINE_NAME) != null)
            {
                baseEntity.CustomAssemblyLineName = context.GetByName(Context.CUSTOM_ASSEMBLY_LINE_NAME).ToString();
            }

            entityJson = context.Get<IResponseHelper>().ObjectToString(baseEntity, ResponseFormat.JSON);

            // Buffer the data based on the transaction id.
            sessionManager.WriteSessionValue(immediateSubmissionDataKey, entityJson);
        }

        /// <summary>
        /// Created:        07/17/2015
        /// Author:         David J. McKee
        /// Purpose:        Restores immedate submission values from the session.
        /// </summary>        
        private static void RestoreImmediateSubmission(IContext context, ViewModel.Vast.Entity request)
        {
            string immediateSubmissionDataKey = BuildCacheKey(context, request);

            string iqSessionId = BuildIqSession(context);

            ISessionManager sessionManager = context.Get<IServiceLocator>().Locate<ISessionManager>(context, iqSessionId);

            string entityJson = sessionManager.ReadSessionValue(immediateSubmissionDataKey, string.Empty);

            ViewModel.Vast.Entity baseEntity = null;

            if (string.IsNullOrWhiteSpace(entityJson) == false)
            {
                baseEntity = (ViewModel.Vast.Entity)context.Get<IRequestHelper>().StringToObject<ViewModel.Vast.Entity>(entityJson, ResponseFormat.JSON);
            }

            if (baseEntity != null)
            {
                foreach (var columnGroup in baseEntity.FieldGroupList)
                {
                    if (columnGroup != null)
                    {
                        if (request.FieldGroupList == null)
                        {
                            request.FieldGroupList = new List<ViewModel.Vast.FieldGroup>();
                        }

                        request.FieldGroupList.Add(columnGroup);
                    }
                }

                // Recover the CustomAssemblyLineName if not already set.
                if ((context.GetByName(Context.CUSTOM_ASSEMBLY_LINE_NAME) == null) ||
                    (string.IsNullOrWhiteSpace(context.GetByName(Context.CUSTOM_ASSEMBLY_LINE_NAME).ToString())))
                {
                    context.SetByName(Context.CUSTOM_ASSEMBLY_LINE_NAME, baseEntity.CustomAssemblyLineName);
                }
            }
        }

        /// <summary>
        /// Created:        07/17/2015
        /// Author:         David J. McKee
        /// Purpose:        Performs either a create or update on the given data.
        /// </summary>        
        private ViewModel.Vast.Entity ExecuteCreateOrUpdate(IContext context,
                                                                    string sysLoginId,
                                                                    string accountCode,
                                                                    string moduleId,
                                                                    string formId,
                                                                    IConnectionHost connectionHost,
                                                                    ViewModel.Vast.Entity request,
                                                                    string actionType,
                                                                    string logSetting,
                                                                    IAssemblyLine<IQ.Entities.VastDB.Entity, IQ.Entities.VastDB.Entity> assemblyLine)
        {
            IQ.ViewModel.Vast.Entity response = new IQ.ViewModel.Vast.Entity();

            DateTime startTime = DateTime.Now;

            try
            {
                SetContextValues(context, accountCode, moduleId, logSetting, actionType);

                if (request.ImmediateSubmission)
                {
                    BufferImmediateSubmission(context, request);
                }
                else
                {
                    // Trace the input
                    //TraceEntry(context, sysLoginId, formId, actionType, logSetting);

                    RestoreImmediateSubmission(context, request);

                    AppendQueryString(context, request, connectionHost);
                    request.ActionResponseType = actionType;
                    EnsureDomainAndAccount(context, request, connectionHost, actionType);

                    // Convert the View Model Entity Request into a Data Model Entity Request                    
                    IQ.Entities.VastDB.Entity dataModelEntity = IQ.BUS.Vast.Common.Helpers.EntitiesHelper.BuildDataModelEntity(context, request, sysLoginId, formId);

                    FlowTransport<IQ.Entities.VastDB.Entity> alContext = new FlowTransport<IQ.Entities.VastDB.Entity>(dataModelEntity, context);

                    IQ.Entities.VastDB.Entity dataModelCreateResponse = assemblyLine.Execute(alContext);

                    IQ.BUS.Vast.Common.DataHelper.ViewModelEntityFromDbEntity(alContext, request, dataModelCreateResponse, response);

                    // Trace the output
                    TraceExit(context, actionType, formId, request.PrimaryKey, response.ActionResponseType, response.FormID, response.PrimaryKey, sysLoginId, logSetting, response.TransactionID, startTime);
                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }

            return response;
        }
    }
}
