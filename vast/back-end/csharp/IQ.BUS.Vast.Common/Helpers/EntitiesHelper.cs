using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using SAIL.Framework.Host;
using SAIL.Framework.Host.Bootstrap;
using SAIL.Framework.Host.BaseClasses;

namespace IQ.BUS.Vast.Common.Helpers
{
    public static class EntitiesHelper
    {
        // New, Create, Read, Update, Delete Only
        public static ViewModel.Vast.Entity ExecuteAssemblyLine(FlowTransport<Entities.VastDB.Entity> context,
                                                                    string instanceGUID,
                                                                    string databaseName,
                                                                    string formId,
                                                                    string actionType,
                                                                    string customAssemblyLineName,
                                                                    string accountCode)
        {
            IQ.ViewModel.Vast.Grid searchResponse;
            return ExecuteAssemblyLine(context, instanceGUID, databaseName, formId, actionType, customAssemblyLineName, accountCode, context.Payload, null, out searchResponse);
        }

        // Search Only
        public static ViewModel.Vast.Grid ExecuteAssemblyLine(FlowTransport<IQ.Entities.VastDB.EntitySearch> context,
                                                                    string instanceGUID,
                                                                    string databaseName,
                                                                    string formId,
                                                                    string actionType,
                                                                    string customAssemblyLineName,
                                                                    string accountCode)
        {
            IQ.ViewModel.Vast.Grid grid;
            ExecuteAssemblyLine(context, instanceGUID, databaseName, formId, actionType, customAssemblyLineName, accountCode, null, context.Payload, out grid);

            return grid;
        }

        private static ViewModel.Vast.Entity ExecuteAssemblyLine(IContext context,
                                                                    string instanceGUID,
                                                                    string databaseName,
                                                                    string formId,
                                                                    string actionType,
                                                                    string customAssemblyLineName,
                                                                    string accountCode,
                                                                    Entities.VastDB.Entity entityPayload,
                                                                    IQ.Entities.VastDB.EntitySearch entitySearchPayload,
                                                                    out IQ.ViewModel.Vast.Grid grid)
        {
            grid = new ViewModel.Vast.Grid();

            ViewModel.Vast.Entity response = new ViewModel.Vast.Entity();

            if (context.GetByName(IQ.Entities.VastDB.Const.Context.CUSTOM_ASSEMBLY_LINE_NAME) == null)
            {
                context.SetByName(IQ.Entities.VastDB.Const.Context.CUSTOM_ASSEMBLY_LINE_NAME, string.Empty);
            }

            if (context.GetByName(IQ.Entities.VastDB.Const.Context.OPERATION_NAME_LIST) == null)
            {
                context.SetByName(IQ.Entities.VastDB.Const.Context.OPERATION_NAME_LIST, string.Empty);
            }

            if (context.GetByName(IQ.Entities.VastDB.Const.Context.ACCOUNT_CODE) == null)
            {
                context.SetByName(IQ.Entities.VastDB.Const.Context.ACCOUNT_CODE, string.Empty);
            }

            // To prevent un bounded recursion, something must change.
            if ((customAssemblyLineName != context.GetByName(IQ.Entities.VastDB.Const.Context.CUSTOM_ASSEMBLY_LINE_NAME).ToString()) ||
                (instanceGUID != context.GetByName(IQ.Entities.VastDB.Const.Context.INSTANCE_GUID).ToString()) ||
                (databaseName != context.GetByName(IQ.Entities.VastDB.Const.Context.DATABASE_ID).ToString()) ||
                (formId != context.GetByName(IQ.Entities.VastDB.Const.Context.TABLE_ID).ToString()) ||
                (accountCode != context.GetByName(IQ.Entities.VastDB.Const.Context.ACCOUNT_CODE).ToString()))
            {
                context.SetByName(IQ.Entities.VastDB.Const.Context.CUSTOM_ASSEMBLY_LINE_NAME, customAssemblyLineName);
                context.SetByName(IQ.Entities.VastDB.Const.Context.INSTANCE_GUID, instanceGUID);
                context.SetByName(IQ.Entities.VastDB.Const.Context.DATABASE_ID, databaseName);
                context.SetByName(IQ.Entities.VastDB.Const.Context.TABLE_ID, formId);
                context.SetByName(IQ.Entities.VastDB.Const.Context.ACCOUNT_CODE, accountCode);

                RepositoryBusinessProcessBase<IQ.ViewModel.Vast.Entity,
                                                            IQ.ViewModel.Vast.Entity,
                                                            IQ.ViewModel.Vast.Entity,
                                                            IQ.ViewModel.Vast.Entity,
                                                            IQ.ViewModel.Vast.Entity,
                                                            IQ.ViewModel.Vast.Entity,
                                                            IQ.ViewModel.Vast.EntitySearch,
                                                            IQ.ViewModel.Vast.Grid> entities =
                context.Get<IServiceLocator>().LocateByName<RepositoryBusinessProcessBase<IQ.ViewModel.Vast.Entity,
                                                            IQ.ViewModel.Vast.Entity,
                                                            IQ.ViewModel.Vast.Entity,
                                                            IQ.ViewModel.Vast.Entity,
                                                            IQ.ViewModel.Vast.Entity,
                                                            IQ.ViewModel.Vast.Entity,
                                                            IQ.ViewModel.Vast.EntitySearch,
                                                            IQ.ViewModel.Vast.Grid>>(context, "IQ.BUS.Vast.Entities");

                if (actionType == IQ.Entities.VastDB.Const.Actions.ACTION_SEARCH)
                {
                    ViewModel.Vast.EntitySearch entitySearchViewModel = new ViewModel.Vast.EntitySearch();
                    IQ.Entities.VastDB.EntitySearch entitySearchDataModel = new Entities.VastDB.EntitySearch();

                    string sysLoginId = context.GetByName(IQ.Entities.VastDB.Const.Context.SYS_LOGIN_ID).ToString();

                    IQ.BUS.Vast.Common.SearchHelper.ViewModelEntitySearchFromDbEntity(context, sysLoginId, formId, entitySearchViewModel, entitySearchDataModel);

                    grid = entities.ProcessSearchRequest(context, entitySearchViewModel);
                }
                else
                {
                    string assemblyLineName = string.Empty;

                    IQ.ViewModel.Vast.Entity requestEntity = new ViewModel.Vast.Entity();

                    requestEntity.DomainName = context.GetByName(IQ.Entities.VastDB.Const.Context.DOMAIN_NAME).ToString();
                    requestEntity.AccountCode = accountCode;
                    requestEntity.FormID = formId;
                    requestEntity.PrimaryKey = entityPayload.PrimaryKey;

                    requestEntity.FieldGroupList = new List<ViewModel.Vast.FieldGroup>();
                    ViewModel.Vast.FieldGroup fieldGroup = new ViewModel.Vast.FieldGroup();
                    requestEntity.FieldGroupList.Add(fieldGroup);

                    fieldGroup.FieldRowList = new List<ViewModel.Vast.FieldRow>();

                    ViewModel.Vast.FieldRow fieldRow = new ViewModel.Vast.FieldRow();
                    fieldGroup.FieldRowList.Add(fieldRow);

                    fieldRow.FieldList = new List<ViewModel.Vast.Field>();

                    foreach (string key in entityPayload.ColumnIdValuePair.Keys)
                    {
                        ViewModel.Vast.Field field = new ViewModel.Vast.Field();
                        field.ID = key;
                        field.Value = entityPayload.ColumnIdValuePair[key];

                        fieldRow.FieldList.Add(field);
                    }

                    IQ.ViewModel.Vast.Entity responseEntity = new ViewModel.Vast.Entity();
                    IQ.BUS.Vast.Common.DataHelper.ViewModelEntityFromDbEntity(context, requestEntity, entityPayload, responseEntity);

                    responseEntity.FieldGroupList = requestEntity.FieldGroupList;

                    responseEntity.DomainName = requestEntity.DomainName;
                    responseEntity.AccountCode = accountCode;
                    responseEntity.FormID = formId;

                    switch (actionType)
                    {
                        case IQ.Entities.VastDB.Const.Actions.ACTION_CREATE:
                            response = entities.ProcessCreateRequest(context, responseEntity);
                            break;

                        case IQ.Entities.VastDB.Const.Actions.ACTION_READ:
                            response = entities.ProcessReadRequest(context, requestEntity.PrimaryKey);
                            break;

                        case IQ.Entities.VastDB.Const.Actions.ACTION_UPDATE:
                            response = entities.ProcessUpdateRequest(context, responseEntity);
                            break;

                        case IQ.Entities.VastDB.Const.Actions.ACTION_DELETE:
                            response = entities.ProcessDeleteRequest(context, requestEntity.PrimaryKey);
                            break;

                        default:
                            response = entities.ProcessNewRequest(context);
                            break;
                    }
                }
            }

            return response;
        }

        /// <summary>
        /// Created:        01/30/2015
        /// Author:         David J. McKee
        /// Purpose:        Converts the given ViewModel into a data entity.      
        /// </summary>        
        public static IQ.Entities.VastDB.Entity BuildDataModelEntity(IContext context,
                                                                        ViewModel.Vast.Entity request,
                                                                        string sysLoginId,
                                                                        string formId)
        {
            IQ.Entities.VastDB.Entity entity = new IQ.Entities.VastDB.Entity();

            try
            {
                entity.PrimaryKey = request.PrimaryKey;
                entity.sysLoginID = sysLoginId;

                if (request == null)
                {
                    // Assume Custom Pipeline
                    entity.TableID = formId;
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(request.FormID))
                    {
                        request.FormID = formId;
                    }

                    entity.TableID = request.FormID;
                    entity.PrimaryKey = request.PrimaryKey;

                    if (entity.ColumnIdValuePair == null)
                    {
                        entity.ColumnIdValuePair = new Dictionary<string, string>();
                    }

                    if (request.FieldGroupList != null)
                    {
                        foreach (var colGroup in request.FieldGroupList)
                        {
                            if (colGroup != null)
                            {
                                if (colGroup.FieldRowList != null)
                                {
                                    foreach (var colRow in colGroup.FieldRowList)
                                    {
                                        if (colRow.FieldList != null)
                                        {
                                            foreach (var col in colRow.FieldList)
                                            {
                                                entity.ColumnIdValuePair[col.ID] = col.Value;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }

            return entity;
        }
    }
}
