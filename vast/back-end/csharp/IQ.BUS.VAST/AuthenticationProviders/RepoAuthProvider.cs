using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SAIL.Framework.Host;
using SAIL.Framework.Host.Bootstrap;

using IQ.ViewModel.Vast;
using IQ.Entities.VastDB.Const;
using IQ.RepositoryInterfaces.Vast;

namespace IQ.BUS.VAST.AuthenticationProviders
{
    /// <summary>
    /// Created:        10/4/2016
    /// Author:         David J. McKee
    /// Purpose:        Authenticates a given user against a repository
    /// Important:      The default repository is Sqlite but any repository may be used.
    /// </summary>
    public class RepoAuthProvider : IAuthenticationProvider
    {
        public static object LoadAuthenticationRepo(IContext context, object domainName)
        {
            const string KEY_STANDARD_REPO = "VastStandardAuthenticationRepo";
            const string DEFAULT_STANDARD_REPO = "IQ.Repository.Vast.SQLite.EntityRepository";

            IAppConfig appConfig = context.Get<IAppConfigFactory>().GetAppConfig(IQ.Entities.VastDB.Const.Context.CONTEXT_VALUE_APP_CODE);

            // Get the default.
            string standardRepo = appConfig.ReadSetting(KEY_STANDARD_REPO, DEFAULT_STANDARD_REPO);

            // Allow for override
            standardRepo = appConfig.ReadSetting(KEY_STANDARD_REPO + domainName, standardRepo);

            IStandardTable standardTableRepo = (IStandardTable)context.Get<IBinding>().LoadViaFullName(context, standardRepo);

            return standardTableRepo;
        }

        bool IAuthenticationProvider.AuthenticateToken(IContext context, IAuthToken authToken)
        {
            return false;
        }

        bool IAuthenticationProvider.AuthenticateCredentials(IContext context, IAuthCredentials authCredentials, IAuthToken authTokenOutput)
        {
            AuthResponse authResponse = new AuthResponse();

            try
            {
                // Authenticate the user based on the instance.
                IQ.BUS.Vast.Entities entities = new IQ.BUS.Vast.Entities();
                IQ.Entities.VastDB.EntitySearch searchCriteria = new IQ.Entities.VastDB.EntitySearch();

                bool authenticateViaTokenOnly = false;

                string password = authCredentials.Credentials["Password"];
                string username = authCredentials.Credentials["Username"];

                if (string.IsNullOrWhiteSpace(password))
                {
                    string authToken = context.Get<IConnectionHost>().RequestAny(IQ.ViewModel.Vast.Const.QueryString.AUTH_TOKEN);
                    string logSetting;
                    string sysLoginId = IQ.BUS.Vast.BaseClasses.REST.ParseSysLoginID(context, authToken, out logSetting);

                    // Search by sysLoginId
                    searchCriteria.Where = new IQ.Entities.VastMetaDB.SqlEsque.Where(new IQ.Entities.VastMetaDB.SqlEsque.Predicate(Columns.COL_GUID_SYS_LOGIN_ID, IQ.Entities.VastMetaDB.Enums.OperatorType.EqualTo, sysLoginId));

                    authenticateViaTokenOnly = true;
                }
                else
                { 
                    searchCriteria.Where = new IQ.Entities.VastMetaDB.SqlEsque.Where(new IQ.Entities.VastMetaDB.SqlEsque.Predicate(Columns.COL_GUID_USER_NAME, IQ.Entities.VastMetaDB.Enums.OperatorType.EqualTo, username));
                }

                IOperation<IQ.Entities.VastDB.EntitySearch, IQ.Entities.VastDB.SearchResponse> searchRepo =
                    context.Get<IServiceLocator>().LocateByName<IOperation<IQ.Entities.VastDB.EntitySearch, IQ.Entities.VastDB.SearchResponse>>(context, "IQ.OPS.Search.SearchRepo");

                FlowTransport<IQ.Entities.VastDB.EntitySearch> searchContext = new FlowTransport<IQ.Entities.VastDB.EntitySearch>(searchCriteria, context);

                string domainName = searchContext[IQ.Entities.VastDB.Const.Context.DOMAIN_NAME].ToString();

                searchContext[IQ.Entities.VastDB.Const.Context.INTERFACE_STANDARD_TABLE] = LoadAuthenticationRepo(context, domainName);

                IQ.Entities.VastDB.SearchResponse userSearchResponse = new IQ.Entities.VastDB.SearchResponse();

                searchCriteria.TableID = Tables.TABLE_GUID_SYS_LOGIN;

                searchContext[IQ.Entities.VastDB.Const.Context.DATABASE_ID] = Databases.CAT_GUID_MASTER;

                searchRepo.Execute(searchContext, null, ref userSearchResponse);

                authResponse.Result.Success = false;

                if ((userSearchResponse.RowList != null) &&
                    (userSearchResponse.RowList.Count > 0))
                {
                    SAIL.Framework.Host.IMD5 md5 = context.Get<SAIL.Framework.Host.IMD5>();

                    string passwordHash = md5.ComputeHash(password.Trim());
                    string defaultDatabaseName = string.Empty;
                    string sysLoginPk = string.Empty;

                    // Check the Password
                    foreach (string[] userEntity in userSearchResponse.RowList)
                    {
                        bool authenticted = false;

                        if (((userSearchResponse.FieldValueByID(userEntity, Columns.COL_GUID_USER_NAME) == username)) &&
                            ((
                                (userSearchResponse.FieldValueByID(userEntity, Columns.COL_GUID_PASSWORD_HASH) == passwordHash) ||
                                (
                                    (userSearchResponse.FieldValueByID(userEntity, Columns.COL_GUID_PASSWORD_HASH) == password) &&
                                    ((userSearchResponse.FieldValueByID(userEntity, Columns.COL_GUID_PASSWORD_HASH).EndsWith("=") == false)
                                )))))
                        {
                            authenticted = true;
                        }
                        else if (authenticateViaTokenOnly)
                        {
                            authenticted = true;
                        }

                        if (authenticted)
                        {
                            if (userSearchResponse.FieldValueByID(userEntity, Columns.COL_GUID_PASSWORD_HASH) == password)
                            {
                                // The password is clear text and needs to be encrypted.
                                // TODO: Set the PasswordHash for the given password.
                            }

                            // Find the default Catalog for the given user
                            authTokenOutput.SetClaim("DefaultModuleID", userSearchResponse.FieldValueByID(userEntity, Columns.COL_GUID_DEFAULT_DATABASE_GUID).Trim());
                            authTokenOutput.SetClaim("DefaultFormID", userSearchResponse.FieldValueByID(userEntity, Columns.COL_GUID_DEFAULT_TABLE_ID).Trim());
                            authTokenOutput.SetClaim("DefaultActionType", userSearchResponse.FieldValueByID(userEntity, Columns.COL_GUID_DEFAULT_ACTION_TYPE).Trim());
                            authTokenOutput.SetClaim("FirstName", userSearchResponse.FieldValueByID(userEntity, Columns.COL_GUID_FIRST_NAME).Trim());
                            authTokenOutput.SetClaim("LastName", userSearchResponse.FieldValueByID(userEntity, Columns.COL_GUID_LAST_NAME).Trim());

                            sysLoginPk = userSearchResponse.FieldValueByID(userEntity, Columns.COL_GUID_SYS_LOGIN_ID);

                            authTokenOutput.SetClaim("SysLoginPk", sysLoginPk);
 
                            string logSetting = userSearchResponse.FieldValueByID(userEntity, Columns.COL_GUID_LOG_SETTING).Trim();

                            authTokenOutput.SetClaim("LogSetting", logSetting);

                            authResponse.AuthToken = IQ.BUS.Vast.Common.DataHelper.BuildAuthToken(context, passwordHash, sysLoginPk, logSetting);
                            authResponse.Result.Success = true;

                            authTokenOutput.SetClaim("AccessToken", authResponse.AuthToken);
                            break;
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }

            return authResponse.Result.Success;
        }
    }
}
