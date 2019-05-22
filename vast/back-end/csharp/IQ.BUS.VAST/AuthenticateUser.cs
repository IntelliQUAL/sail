using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using SAIL.Framework.Host;
using SAIL.Framework.Host.Bootstrap;

using IQ.ViewModel.Vast;
using IQ.BUS.Vast.Helpers;
using IQ.Entities.VastDB.Const;
using IQ.BUS.Vast.Common.Types;
using IQ.BUS.Vast.Common.Helpers;
using IQ.BUS.VAST.AuthenticationProviders;

namespace IQ.BUS.Vast
{
    /// <summary>
    /// Created:        02/24/2015
    /// Author:         David J. McKee
    /// Purpose:
    /// Examples:
    ///                 XML Request:    http://localhost/PIPE.WebApiHost/v7/xml/IQ.BUS.Vast.AuthenticateUser/?e=ExampleRequest
    ///                 XML Response:   http://localhost/PIPE.WebApiHost/v7/xml/IQ.BUS.Vast.AuthenticateUser/?e=ExampleResponse
    ///                 JSON Request:   http://localhost/PIPE.WebApiHost/v7/json/IQ.BUS.Vast.AuthenticateUser/?e=ExampleRequest
    ///                 JSON Response:  http://localhost/PIPE.WebApiHost/v7/json/IQ.BUS.Vast.AuthenticateUser/?e=ExampleResponse
    ///                 SOAP Request:   http://localhost/PIPE.WebApiHost/v7/soap/IQ.BUS.Vast.AuthenticateUser/?e=ExampleRequest
    ///                 SOAP Response:  http://localhost/PIPE.WebApiHost/v7/soap/IQ.BUS.Vast.AuthenticateUser/?e=ExampleResponse
    ///                 SOAP WSDL:      http://localhost/PIPE.WebApiHost/v7/soap/IQ.BUS.Vast.AuthenticateUser/?e=WSDL
    ///                 XSD:            http://localhost/PIPE.WebApiHost/v7/soap/IQ.BUS.Vast.AuthenticateUser/?e=XSD
    ///                 
    /// Error Trace:                    http://localhost/PIPE.WebApiHost/v7/html/PIPE.BusinessProcesses.Tracing.TraceQueueDump/Error
    /// </summary>
    public class AuthenticateUser : SAIL.Framework.Host.BaseClasses.ActionBusinessProcessBase<AuthRequest, AuthResponse>
    {
        public override AuthResponse ProcessRequest(SAIL.Framework.Host.IContext context, AuthRequest request)
        {
            AuthResponse authResponse = new AuthResponse();

            try
            {
                authResponse.Result.Success = false;

                string instanceGUID = ViewModelToDataModel.BuildInstanceGUID(request.DomainName, request.AccountCode);

                context.Set(new WebsiteReader(instanceGUID, request.AccountStatus.ToString()));

                // Create a new Context to access the Master catalog
                context.SetByName(IQ.Entities.VastDB.Const.Context.INSTANCE_GUID, instanceGUID);
                context.SetByName(IQ.Entities.VastDB.Const.Context.INSTANCE_STATUS, request.AccountStatus);
                context.SetByName(IQ.Entities.VastDB.Const.Context.DATABASE_ID, IQ.Entities.VastDB.Const.Databases.CAT_GUID_MASTER);
                context.SetByName(IQ.Entities.VastDB.Const.Context.TABLE_ID, IQ.Entities.VastDB.Const.Tables.TABLE_GUID_SYS_LOGIN);
                context.SetByName(IQ.Entities.VastDB.Const.Context.DOMAIN_NAME, request.DomainName);

                // Authenticate the user using the domain specific authentication provider.
                IAuthenticationProvider authenticationProvider = LoadAuthenticationProvider(context, request.DomainName);

                UserCredentials userCredentials = new UserCredentials();
                AuthenticationToken authenticationToken = new AuthenticationToken();

                if (authenticationProvider != null)
                {
                    userCredentials.Username = request.Username;
                    userCredentials.Password = request.Password;
                    userCredentials.AccountCode = request.AccountCode;

                    authResponse.Result.Success = authenticationProvider.AuthenticateCredentials(context, userCredentials, authenticationToken);
                }

                // Append Module / Database Access List
                if (authResponse.Result.Success)
                {
                    authResponse.AccountID = userCredentials.AccountCode;
                    authResponse.AccountStatus = request.AccountStatus;
                    authResponse.AuthToken = authenticationToken.AccessToken;
                    authResponse.DefaultActionType = authenticationToken.DefaultActionType;
                    authResponse.DefaultFormID = authenticationToken.DefaultFormID;
                    authResponse.DefaultModuleID = authenticationToken.DefaultModuleID;
                    authResponse.DomainName = request.DomainName;
                    authResponse.FirstName = authenticationToken.FirstName;
                    authResponse.LastName = authenticationToken.LastName;
                    authResponse.HostUrl = _connectionHost.HostUrl;  // Future use for loadbalancing.

                    // Attempt ModuleList
                    AppendDatabaseAccessList(context, request, authResponse, authenticationToken.SysLoginPk, authenticationToken.LogSetting);

                    // Append GroupNameList
                    // Important: The user will have a seperate sysUserRoleList for each module.
                    // The group name list corresponds to the module id requested.  If no module was requested, then the default modules is used.
                    AppendGroupNameList(context, authResponse);

                    // Append AccountType
                    AppendAccountType(context, authResponse);
                }
                else
                {
                    authResponse.Result.ErrorCode = IQ.Entities.VastDB.Const.Errors.ERROR_CODE_AUTHENTICATION_FAILED;
                    authResponse.Result.ErrorText = IQ.Entities.VastDB.Const.Errors.ERROR_TEXT_AUTHENTICATION_FAILED;
                    authResponse.Result.UserMessage = IQ.Entities.VastDB.Const.Errors.USER_MESSAGE_AUTHENTICATION_FAILED;
                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }
            finally
            {
                // If the authentication fails, trace the output.
                if (string.IsNullOrWhiteSpace(authResponse.AuthToken))
                {
                    try
                    {
                        StringBuilder authFailTrace = new StringBuilder();
                        authFailTrace.Append("AuthenticateUser Failed for request.DomainName: " + request.DomainName +
                                                ", request.AccountCode: " + request.AccountCode +
                                                ", request.AccountStatus: " + request.AccountStatus +
                                                ", DB: " + IQ.Entities.VastDB.Const.Databases.CAT_GUID_MASTER +
                                                ", Table: " + IQ.Entities.VastDB.Const.Tables.TABLE_GUID_SYS_LOGIN +
                                                ", request.DomainName: " + request.DomainName +
                                                ", request.Username: " + request.Username +
                                                ", request.Password: " + request.Password +
                                                ", request.AccountCode: " + request.AccountCode);

                        context.Get<ITrace>().Emit(System.Diagnostics.TraceLevel.Warning, authFailTrace.ToString());
                    }
                    catch (System.Exception ex2)
                    {
                        context.Get<IExceptionHandler>().HandleException(context, ex2);
                    }
                }
            }

            return authResponse;
        }

        /// <summary>
        /// Created:        03/03/2017
        /// Author:         David J. McKee
        /// Purpose:        Searches the MasterRepository database for the single row in the Account table.      
        /// </summary>
        private void AppendAccountType(IContext context, AuthResponse authResponse)
        {
            try
            {
                IQ.Entities.VastDB.EntitySearch sysLoginDatabaseSearchCriteria = new IQ.Entities.VastDB.EntitySearch();

                sysLoginDatabaseSearchCriteria.Page = 1;
                sysLoginDatabaseSearchCriteria.RowsPerPage = 100;

                IOperation<IQ.Entities.VastDB.EntitySearch, IQ.Entities.VastDB.SearchResponse> searchRepo =
                    context.Get<IServiceLocator>().LocateByName<IOperation<IQ.Entities.VastDB.EntitySearch, IQ.Entities.VastDB.SearchResponse>>(context, "IQ.OPS.Search.SearchRepo");

                FlowTransport<IQ.Entities.VastDB.EntitySearch> searchContext = new FlowTransport<IQ.Entities.VastDB.EntitySearch>(sysLoginDatabaseSearchCriteria, context);

                searchContext[IQ.Entities.VastDB.Const.Context.TABLE_ID] = IQ.Entities.VastDB.Const.Tables.IQ_CLOUD_TABLE_ID_ACCOUNT;

                string domainName = searchContext[IQ.Entities.VastDB.Const.Context.DOMAIN_NAME].ToString();

                IQ.Entities.VastDB.SearchResponse accountSearchResponse = new IQ.Entities.VastDB.SearchResponse();

                sysLoginDatabaseSearchCriteria.TableID = IQ.Entities.VastDB.Const.Tables.IQ_CLOUD_TABLE_ID_ACCOUNT;

                searchContext[IQ.Entities.VastDB.Const.Context.DATABASE_ID] = IQ.Entities.VastDB.Const.Databases.IQ_CLOUD_DATABASE_GUID;

                searchContext[IQ.Entities.VastDB.Const.Context.INTERFACE_STANDARD_TABLE] = RepoAuthProvider.LoadAuthenticationRepo(context, domainName);

                searchRepo.Execute(searchContext, null, ref accountSearchResponse);

                foreach (string[] sysLoginDatabase in accountSearchResponse.RowList)
                {
                    Dictionary<string, string> accountRow = accountSearchResponse.ReadRow(sysLoginDatabase);

                    string accountType = accountRow[Columns.COL_GUID_ACCOUNT_TYPE].Trim();

                    if (string.IsNullOrWhiteSpace(accountType) == false)
                    {
                        authResponse.AccountType = accountType;
                        break;
                    }
                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }
        }

        /// <summary>
        /// Created:        03/03/2017
        /// Author:         David J. McKee
        /// Purpose:        Appends the GroupNameList to the AuthResponse      
        /// </summary>
        private void AppendGroupNameList(IContext context, AuthResponse authResponse)
        {
            try
            {
                string moduleId = ViewModelToDataModel.GetModuleFromQueryString(context, _connectionHost);

                if (string.IsNullOrWhiteSpace(moduleId))
                {
                    moduleId = authResponse.DefaultModuleID;
                }

                if (string.IsNullOrWhiteSpace(moduleId) == false)
                {
                    foreach (string[] moduleData in authResponse.ModuleList.RowList)
                    {
                        Dictionary<string, string> moduleRow = authResponse.ModuleList.ReadRow(moduleData);

                        string localModuleId = moduleRow[IQ.Entities.VastDB.Const.Columns.COL_GUID_NAME];

                        if (localModuleId == moduleId)
                        {
                            string userRoleList = moduleRow[IQ.Entities.VastDB.Const.Columns.COL_GUID_USER_ROLE_LIST];

                            if (string.IsNullOrWhiteSpace(userRoleList) == false)
                            {
                                authResponse.GroupNameList = userRoleList;
                            }

                            break;
                        }
                    }
                }
            }
            catch (System.Exception ex2)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex2);
            }
        }

        private IAuthenticationProvider LoadAuthenticationProvider(IContext context, string domainName)
        {
            IAuthenticationProvider authenticationProvider = null;

            try
            {
                IAppConfig appConfig = context.Get<IAppConfigFactory>().GetAppConfig(IQ.Entities.VastDB.Const.Context.CONTEXT_VALUE_APP_CODE);

                const string KEY_AUTHENTICATION_PROVIDER_PREFIX = "AuthenticationProviderFor";

                string authenticationProviderName = appConfig.ReadSetting(KEY_AUTHENTICATION_PROVIDER_PREFIX + domainName, typeof(RepoAuthProvider).FullName);

                authenticationProvider = context.Get<IServiceLocator>().LocateByName<IAuthenticationProvider>(context, authenticationProviderName);
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }

            return authenticationProvider;
        }

        /// <summary>
        /// Created:        10/04/2016
        /// Author:         David J. McKee
        /// Purpose:        Returns the list of databases / modules for which the user has access.
        /// Assumption:     For now the same repository is used to authenticate the user and return the database access list.
        /// </summary>        
        private static void AppendDatabaseAccessList(IContext context, AuthRequest request, AuthResponse authResponse, string sysLoginPk, string logSetting)
        {
            try
            {
                // Find all Databases
                context.SetByName(IQ.Entities.VastDB.Const.Context.TABLE_ID, Tables.TABLE_GUID_SYS_LOGIN_DATABASE);

                IQ.Entities.VastDB.EntitySearch sysLoginDatabaseSearchCriteria = new IQ.Entities.VastDB.EntitySearch();

                sysLoginDatabaseSearchCriteria.Page = 1;
                sysLoginDatabaseSearchCriteria.RowsPerPage = 100;

                sysLoginDatabaseSearchCriteria.Where =
                    new IQ.Entities.VastMetaDB.SqlEsque.Where(new IQ.Entities.VastMetaDB.SqlEsque.Predicate(Columns.COL_GUID_SYS_LOGIN_ID,
                                                        IQ.Entities.VastMetaDB.Enums.OperatorType.EqualTo, sysLoginPk));

                sysLoginDatabaseSearchCriteria.OrderBy = new IQ.Entities.VastMetaDB.SqlEsque.OrderBy();
                sysLoginDatabaseSearchCriteria.OrderBy.SortColumn = Columns.COL_GUID_LAST_ACCESSED;
                sysLoginDatabaseSearchCriteria.OrderBy.SortOrder = IQ.Entities.VastMetaDB.Enums.SortOrder.Desc;

                IOperation<IQ.Entities.VastDB.EntitySearch, IQ.Entities.VastDB.SearchResponse> searchRepo =
                    context.Get<IServiceLocator>().LocateByName<IOperation<IQ.Entities.VastDB.EntitySearch, IQ.Entities.VastDB.SearchResponse>>("IQ.OPS.Search.SearchRepo");

                FlowTransport<IQ.Entities.VastDB.EntitySearch> searchContext = new FlowTransport<IQ.Entities.VastDB.EntitySearch>(sysLoginDatabaseSearchCriteria, context);

                string domainName = searchContext[IQ.Entities.VastDB.Const.Context.DOMAIN_NAME].ToString();

                searchContext[IQ.Entities.VastDB.Const.Context.INTERFACE_STANDARD_TABLE] = RepoAuthProvider.LoadAuthenticationRepo(context, domainName);

                IQ.Entities.VastDB.SearchResponse sysLoginDatabaseList = new IQ.Entities.VastDB.SearchResponse();

                sysLoginDatabaseSearchCriteria.TableID = Tables.TABLE_GUID_SYS_LOGIN_DATABASE;

                searchContext[IQ.Entities.VastDB.Const.Context.DATABASE_ID] = Databases.CAT_GUID_MASTER;

                searchRepo.Execute(searchContext, ref sysLoginDatabaseList);

                // Find all Databases the user can access.
                // Filter for only databases the user can access.
                IQ.Entities.VastDB.SearchResponse databaseList = null;

                foreach (string[] sysLoginDatabase in sysLoginDatabaseList.RowList)
                {
                    Dictionary<string, string> sysLoginDatabaseRow = sysLoginDatabaseList.ReadRow(sysLoginDatabase);

                    string sysDatabaseID = sysLoginDatabaseRow[Columns.COL_GUID_SYS_DATABASE_ID];

                    string defaultTableIdSysLoginDatabase = "";
                    string defaultActionTypeSysLoginDatabase = "";

                    GetDefaultTableAndActionTypeFromDatabase(sysLoginDatabaseRow, ref defaultTableIdSysLoginDatabase, ref defaultActionTypeSysLoginDatabase);

                    IQ.Entities.VastDB.EntitySearch sysDatabaseSearchCriteria = new IQ.Entities.VastDB.EntitySearch();

                    sysDatabaseSearchCriteria.TableID = Tables.TABLE_GUID_SYS_DATABASE;

                    sysDatabaseSearchCriteria.Where =
                        new IQ.Entities.VastMetaDB.SqlEsque.Where(new IQ.Entities.VastMetaDB.SqlEsque.Predicate(Columns.COL_GUID_SYS_DATABASE_ID,
                                                            IQ.Entities.VastMetaDB.Enums.OperatorType.EqualTo, sysDatabaseID));

                    if (databaseList == null)
                    {
                        searchContext = new FlowTransport<IQ.Entities.VastDB.EntitySearch>(sysDatabaseSearchCriteria, searchContext);

                        databaseList = new IQ.Entities.VastDB.SearchResponse();

                        searchRepo.Execute(searchContext, ref databaseList);

                        SetDefaultTableAndActionType(context, databaseList, defaultTableIdSysLoginDatabase, defaultActionTypeSysLoginDatabase);

                        // Merge sysLoginDatabaseRow
                        Dictionary<string, string> firstModule = databaseList.ReadRow(databaseList.RowList[0]);

                        foreach (string key in sysLoginDatabaseRow.Keys)
                        {
                            if (firstModule.ContainsKey(key) == false)
                            {
                                firstModule.Add(key, sysLoginDatabaseRow[key]);
                            }
                        }

                        databaseList.LoadTableFromColumnIdValuePair(databaseList.Table.ID, firstModule);

                        databaseList.ReplaceRow(0, firstModule);
                    }
                    else
                    {
                        IQ.Entities.VastDB.SearchResponse singleDb = new IQ.Entities.VastDB.SearchResponse();

                        searchContext = new FlowTransport<IQ.Entities.VastDB.EntitySearch>(sysDatabaseSearchCriteria, searchContext);

                        searchRepo.Execute(searchContext, ref singleDb);

                        SetDefaultTableAndActionType(context, singleDb, defaultTableIdSysLoginDatabase, defaultActionTypeSysLoginDatabase);

                        foreach (string[] dbRow in singleDb.RowList)
                        {
                            Dictionary<string, string> dbDict = singleDb.ReadRow(dbRow);

                            foreach (string key in sysLoginDatabaseRow.Keys)
                            {
                                if (dbDict.ContainsKey(key) == false)
                                {
                                    dbDict.Add(key, sysLoginDatabaseRow[key]);
                                }
                            }

                            databaseList.AppendRow(dbDict);
                        }
                    }
                }

                if (databaseList != null)
                {
                    IQ.ViewModel.Vast.EntitySearch viewModelEntitySearch = new EntitySearch();
                    IQ.BUS.Vast.Common.SearchHelper.ViewModelEntitySearchFromDbEntity(context, sysLoginPk, databaseList.Table.ID, viewModelEntitySearch, searchContext.Payload);

                    authResponse.ModuleList = IQ.BUS.Vast.Common.SearchHelper.ViewModelGridFromDbEntity(context, viewModelEntitySearch, databaseList);
                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(ex);
            }
        }

        /// <summary>
        /// Created:        02/03/2016
        /// Author:         David J. McKee
        /// Purpose:        Ensures that a DefaultTableID, DefaultActionType
        /// Assumptions:    We are only dealing with one row at one time. sysLoginDatabase is the highest priority.
        /// </summary>        
        private static void SetDefaultTableAndActionType(IContext context, IQ.Entities.VastDB.SearchResponse databaseList,
                                                            string defaultTableIdSysLoginDatabase, string defaultActionTypeSysLoginDatabase)
        {
            try
            {
                foreach (string[] row in databaseList.RowList)
                {
                    Dictionary<string, string> rowData = databaseList.ReadRow(row);

                    string defaultTableId = defaultTableIdSysLoginDatabase;
                    string defaultActionType = defaultActionTypeSysLoginDatabase;

                    if (string.IsNullOrWhiteSpace(defaultTableId))
                    {
                        if (rowData.ContainsKey(Columns.COL_GUID_DEFAULT_TABLE_ID))
                        {
                            if (string.IsNullOrWhiteSpace(rowData[Columns.COL_GUID_DEFAULT_TABLE_ID]) == false)
                            {
                                defaultTableId = rowData[Columns.COL_GUID_DEFAULT_TABLE_ID];
                            }
                        }
                    }

                    if (string.IsNullOrWhiteSpace(defaultActionType))
                    {
                        if (rowData.ContainsKey(Columns.COL_GUID_DEFAULT_ACTION_TYPE))
                        {
                            if (string.IsNullOrWhiteSpace(rowData[Columns.COL_GUID_DEFAULT_ACTION_TYPE]) == false)
                            {
                                defaultActionType = rowData[Columns.COL_GUID_DEFAULT_ACTION_TYPE];
                            }
                        }
                    }

                    // Update COL_GUID_DEFAULT_TABLE_ID and COL_GUID_DEFAULT_ACTION_TYPE
                    IQ.Entities.VastMetaDB.Column defaultTableColumn = databaseList.FieldByID(Columns.COL_GUID_DEFAULT_TABLE_ID);

                    if (defaultTableColumn != null)
                    {
                        row[defaultTableColumn.Schema.OrdinalPosition] = defaultTableId;
                    }

                    IQ.Entities.VastMetaDB.Column defaultActionTypeColumn = databaseList.FieldByID(Columns.COL_GUID_DEFAULT_ACTION_TYPE);

                    if (defaultActionTypeColumn != null)
                    {
                        row[defaultActionTypeColumn.Schema.OrdinalPosition] = defaultActionType;
                    }
                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }
        }

        private static void GetDefaultTableAndActionTypeFromDatabase(Dictionary<string, string> sysLoginDatabaseRow, ref string defaultTableIdSysLoginDatabase, ref string defaultActionTypeSysLoginDatabase)
        {
            if (sysLoginDatabaseRow.ContainsKey(Columns.COL_GUID_DEFAULT_TABLE_ID))
            {
                defaultTableIdSysLoginDatabase = sysLoginDatabaseRow[Columns.COL_GUID_DEFAULT_TABLE_ID];
            }

            if (sysLoginDatabaseRow.ContainsKey(Columns.COL_GUID_DEFAULT_ACTION_TYPE))
            {
                defaultActionTypeSysLoginDatabase = sysLoginDatabaseRow[Columns.COL_GUID_DEFAULT_ACTION_TYPE];
            }
        }

        public override AuthRequest ExampleRequest
        {
            get
            {
                AuthRequest authRequest = new AuthRequest();

                authRequest.DomainName = "mydomain.com";
                authRequest.AccountCode = "MyAccount";
                authRequest.AccountStatus = IQ.ViewModel.Vast.Enums.InstanceStatus.QA.ToString();
                authRequest.Username = "MyUsername";
                authRequest.Password = "MyPassword";

                return authRequest;
            }
        }

        public override AuthResponse ExampleResponse
        {
            get { return new AuthResponse(); }
        }
    }
}

