using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using IQ.BUS.Vast.Common.Helpers;
using IQ.RepositoryInterfaces.Vast;

using SAIL.Framework.Host;

namespace IQ.BUS.Vast.Common
{
    public class CommonConcerns
    {
        private static OperationConfigFactory _operationConfigFactory = new OperationConfigFactory();

        private static System.Collections.Hashtable _standardRepoList = new System.Collections.Hashtable();
        private static IBlobTable _binaryRepo = null;
        private static IStandardTable _metaDataRepo = null;

        public static void InitAssemblyLineContext(IContext context, string actionType, out string customAssemblyLineName, out string operationNameList)
        {
            context.SetByName(IQ.Entities.VastDB.Const.Context.EXIT_ASSEMBLY_LINE, false);

            IConnectionHost connectionHost = context.Get<IConnectionHost>();

            string domainName = ViewModelToDataModel.GetDomainFromQueryString(context, connectionHost);

            // Init / Ensure the Context            
            InitContext(context, domainName);

            // Allow for Operation Specific Configuration
            context.Set(_operationConfigFactory);

            context.SetByName(Consts.Context.CONTEXT_KEY_ACTION_TYPE, actionType);

            // Custom Assembly Line Name
            if (context.GetByName(IQ.Entities.VastDB.Const.Context.CUSTOM_ASSEMBLY_LINE_NAME) == null)
            {
                customAssemblyLineName = string.Empty;
            }
            else
            {
                customAssemblyLineName = context.GetByName(IQ.Entities.VastDB.Const.Context.CUSTOM_ASSEMBLY_LINE_NAME).ToString();
            }

            // Allow for a list of operations
            if (context.GetByName(IQ.Entities.VastDB.Const.Context.OPERATION_NAME_LIST) == null)
            {
                operationNameList = string.Empty;
            }
            else
            {
                operationNameList = context.GetByName(IQ.Entities.VastDB.Const.Context.OPERATION_NAME_LIST).ToString();
            }
        }

        public static void InitAssemblyLineContext(IContext context,
                                                    string actionType,
                                                    out string instanceGuid,
                                                    out string moduleId,
                                                    out string customAssemblyLineName,
                                                    out string transactionId,
                                                    out string operationNameList)
        {
            instanceGuid = string.Empty;
            moduleId = string.Empty;
            customAssemblyLineName = string.Empty;
            transactionId = Guid.NewGuid().ToString();
            operationNameList = string.Empty;

            try
            {
                InitAssemblyLineContext(context, actionType, out customAssemblyLineName, out operationNameList);

                instanceGuid = context.GetByName(IQ.Entities.VastDB.Const.Context.INSTANCE_GUID).ToString();
                moduleId = context.GetByName(IQ.Entities.VastDB.Const.Context.DATABASE_ID).ToString();
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }
        }

        public static void InitRequestAndResponse(string tableId, string actionType, IQ.Entities.VastDB.Entity request, IQ.Entities.VastDB.Entity response)
        {
            // Set the unique identifier for the Table within the Database
            response.ActionResponseType = actionType;
            response.TableID = tableId;
            response.Table = new IQ.Entities.VastMetaDB.Table();
            response.Table.ID = tableId;
            response.PrimaryKey = request.PrimaryKey;

            if (request.Table == null)
            {
                request.TableID = tableId;
                request.Table = new IQ.Entities.VastMetaDB.Table();
                request.Table.ID = request.TableID;
            }
        }

        public static IStandardTable InitStandardTableRepo(IContext context, string domainName)
        {
            IStandardTable result = null;

            lock (_standardRepoList.SyncRoot)
            {
                if (_standardRepoList.ContainsKey(domainName) == false)
                {
                    const string KEY_STANDARD_REPO = "VastStandardRepo";
                    const string DEFAULT_STANDARD_REPO = "IQ.Repository.Vast.SQLite.EntityRepository";

                    IAppConfig appConfig = context.Get<IAppConfigFactory>().GetAppConfig(IQ.Entities.VastDB.Const.Context.CONTEXT_VALUE_APP_CODE);

                    // Get the default.
                    string standardRepo = appConfig.ReadSetting(KEY_STANDARD_REPO, DEFAULT_STANDARD_REPO);

                    // Allow for override
                    standardRepo = appConfig.ReadSetting(KEY_STANDARD_REPO + domainName, standardRepo);

                    // Init the SQL Server Based Repository                                    
                    _standardRepoList[domainName] = (IStandardTable)context.Get<IBinding>().LoadViaFullName(context, standardRepo);
                }

                result = (IStandardTable)_standardRepoList[domainName];
            }

            return result;
        }

        private static IBlobTable InitBlobTableRepo(IContext context)
        {
            if (_binaryRepo == null)
            {
                // Init the File Based Repository
                _binaryRepo = (IBlobTable)context.Get<IBinding>().LoadViaFullName(context, "IQ.Repository.Vast.FileBased.EntityRepository");
            }

            return _binaryRepo;
        }

        public static void InitContext(IContext context, string domainName)
        {
            try
            {
                // Add Standard Table Repo
                IStandardTable standardTableRepo = InitStandardTableRepo(context, domainName);
                context.SetByName(IQ.Entities.VastDB.Const.Context.INTERFACE_STANDARD_TABLE, standardTableRepo);

                // Add Blob Table Repo
                IBlobTable blobTableRepo = InitBlobTableRepo(context);
                context.SetByName(IQ.Entities.VastDB.Const.Context.INTERFACE_BLOB, blobTableRepo);

                // Add Metadata Repo INTERFACE_STANDARD_TABLE_OR_BLOB_METADATA
                IStandardTable metadataRepo = InitMetaDataRepo(context);
                context.SetByName(IQ.Entities.VastDB.Const.Context.INTERFACE_STANDARD_TABLE_OR_BLOB_METADATA, metadataRepo);
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }
        }

        private static IStandardTable InitMetaDataRepo(IContext context)
        {
            if (_metaDataRepo == null)
            {
                // Init the SQLite Based Repository                                
                _metaDataRepo = (IStandardTable)context.Get<IBinding>().LoadViaFullName(context, "IQ.Repository.Vast.SQLite.EntityRepository");
            }

            return _metaDataRepo;
        }
    }
}
