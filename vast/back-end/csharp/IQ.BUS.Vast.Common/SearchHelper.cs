using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using SAIL.Framework.Host;

namespace IQ.BUS.Vast.Common
{
    public static class SearchHelper
    {
        /// <summary>
        /// Created:        07/17/2015
        /// Author:         David J. McKee
        /// Purpose:        Sets the paging information for the request
        /// </summary>        
        private static void SetPagingInfo(ViewModel.Vast.EntitySearch request, IQ.Entities.VastDB.EntitySearch dataModelEntity)
        {
            const int DEFAULT_PAGE = 1;
            const int MAX_ROW_COUNT = 100;
            const int DEFAULT_ROW_COUNT = 10;

            if (request == null)
            {
                dataModelEntity.Page = DEFAULT_PAGE;
                dataModelEntity.RowsPerPage = DEFAULT_ROW_COUNT;
            }
            else
            {
                if (request.Page == null)
                {
                    dataModelEntity.Page = DEFAULT_PAGE;
                }
                else
                {
                    dataModelEntity.Page = (int)request.Page;
                }

                if (request.RowsPerPage == null)
                {
                    dataModelEntity.RowsPerPage = DEFAULT_ROW_COUNT;
                }
                else
                {
                    dataModelEntity.RowsPerPage = (int)request.RowsPerPage;

                    if (dataModelEntity.RowsPerPage > MAX_ROW_COUNT)
                    {
                        dataModelEntity.RowsPerPage = MAX_ROW_COUNT;
                    }
                }
            }
        }

        /// <summary>
        /// Created:        07/17/2015
        /// Author:         David J. McKee
        /// Purpose:        Processes any inbound And or Or statements.
        /// </summary>        
        private static void AppendAndOr(IContext context,
                                        SAIL.ViewModel.SqlEsque.predicate viewModelPredicateParent,
                                        IQ.Entities.VastMetaDB.SqlEsque.PredicateParent entityPredicateParent,
                                        ref int placeholderIndex)
        {
            // And
            if (viewModelPredicateParent.and != null)
            {
                IQ.Entities.VastMetaDB.Enums.OperatorType andOperatorType = (IQ.Entities.VastMetaDB.Enums.OperatorType)Enum.Parse(typeof(IQ.Entities.VastMetaDB.Enums.OperatorType), viewModelPredicateParent.and.@operator.ToString());

                IQ.Entities.VastMetaDB.SqlEsque.Predicate predicate = new IQ.Entities.VastMetaDB.SqlEsque.Predicate(viewModelPredicateParent.and.field, andOperatorType, viewModelPredicateParent.and.value);

                placeholderIndex++;
                predicate.PlaceholderIndex = placeholderIndex;

                entityPredicateParent.And = new IQ.Entities.VastMetaDB.SqlEsque.AND(predicate);

                AppendAndOr(context, viewModelPredicateParent.and, entityPredicateParent.And, ref placeholderIndex);
            }

            // Or
            if (viewModelPredicateParent.or != null)
            {
                IQ.Entities.VastMetaDB.Enums.OperatorType andOperatorType = (IQ.Entities.VastMetaDB.Enums.OperatorType)Enum.Parse(typeof(IQ.Entities.VastMetaDB.Enums.OperatorType), viewModelPredicateParent.or.@operator.ToString());

                IQ.Entities.VastMetaDB.SqlEsque.Predicate predicate = new IQ.Entities.VastMetaDB.SqlEsque.Predicate(viewModelPredicateParent.or.field, andOperatorType, viewModelPredicateParent.or.value);

                placeholderIndex++;
                predicate.PlaceholderIndex = placeholderIndex;

                //entityPredicateParent.Or = new IQ.Entities.VastMetaDB.SqlEsque.OR(predicate);

                //AppendAndOr(context, viewModelPredicateParent.Or, entityPredicateParent.Or, ref placeholderIndex);
            }
        }

        /// <summary>
        /// Created:        07/17/2015
        /// Author:         David J. McKee
        /// Purpoe:         Processes any inbound where condition.
        /// </summary>        
        private static void ProcessWhereCriteria(IContext context, ViewModel.Vast.EntitySearch request, IQ.Entities.VastDB.EntitySearch dataModelEntity)
        {
            if (request.Where != null)
            {
                int placeholderIndex = 1;

                IQ.Entities.VastMetaDB.Enums.OperatorType operatorType = (IQ.Entities.VastMetaDB.Enums.OperatorType)Enum.Parse(typeof(IQ.Entities.VastMetaDB.Enums.OperatorType), request.Where.@operator.ToString());

                IQ.Entities.VastMetaDB.SqlEsque.Predicate predicate = new IQ.Entities.VastMetaDB.SqlEsque.Predicate(request.Where.field, operatorType, request.Where.value);
                predicate.PlaceholderIndex = placeholderIndex;

                dataModelEntity.Where = new IQ.Entities.VastMetaDB.SqlEsque.Where(predicate);

                AppendAndOr(context, request.Where, dataModelEntity.Where, ref placeholderIndex);
            }
        }

        /// <summary>
        /// Created:        07/17/2015
        /// Author:         David J. McKee
        /// Purpose:        Processes any inbound order-by criteria.
        /// </summary>        
        private static void ProcessOrderByCriteria(IContext context, ViewModel.Vast.EntitySearch request, IQ.Entities.VastDB.EntitySearch dataModelEntity)
        {
            try
            {
                if (request.OrderBy != null)
                {
                    if (dataModelEntity.OrderBy == null)
                    {
                        dataModelEntity.OrderBy = new IQ.Entities.VastMetaDB.SqlEsque.OrderBy();
                        dataModelEntity.OrderBy.SortColumn = request.OrderBy.sortColumn;
                        dataModelEntity.OrderBy.SortOrder = (IQ.Entities.VastMetaDB.Enums.SortOrder)Enum.Parse(typeof(IQ.Entities.VastMetaDB.Enums.SortOrder), request.OrderBy.sortOrder);
                    }
                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }
        }

        public static void ViewModelEntitySearchFromDbEntity(IContext context, string sysLoginId, string formId, ViewModel.Vast.EntitySearch request, IQ.Entities.VastDB.EntitySearch dataModelEntity)
        {
            dataModelEntity.TableID = formId;
            dataModelEntity.sysLoginID = sysLoginId;

            SetPagingInfo(request, dataModelEntity);

            ProcessWhereCriteria(context, request, dataModelEntity);

            ProcessOrderByCriteria(context, request, dataModelEntity);

            dataModelEntity.Distinct = request.Distinct;

            if (string.IsNullOrWhiteSpace(request.ReturnColumnList) == false)
            {
                string[] columnIdList = request.ReturnColumnList.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                dataModelEntity.ReturnColumnList = new List<string>(columnIdList);
            }
        }

        private static void AppendClientActionList(IContext context, IQ.Entities.VastDB.SearchResponse searchResponse, ViewModel.Vast.Grid result)
        {
            try
            {
                // Append Client-side Actions
                if ((searchResponse.ClientActionRowList != null) &&
                    (searchResponse.ClientActionRowList.Count > 0))
                {
                    result.ClientActionList = new List<ViewModel.Vast.ClientAction>();

                    foreach (string[] clientActionRow in searchResponse.ClientActionRowList)
                    {
                        Dictionary<string, string> clientActionRowData = searchResponse.ReadTableRow(searchResponse.ClientActionRowTable, clientActionRow);

                        ViewModel.Vast.ClientAction clientAction = new ViewModel.Vast.ClientAction();

                        clientAction.ActionType = clientActionRowData["ActionType"];
                        clientAction.CustomAssemblyLineName = clientActionRowData["CustomAssemblyLineName"];
                        clientAction.FilterFieldName = clientActionRowData["FilterFieldName"];
                        clientAction.NewDisplayName = clientActionRowData["NewDisplayName"];
                        clientAction.TableID = clientActionRowData["TableID"];
                        clientAction.ViewName = clientActionRowData["ViewName"];
                        clientAction.url = clientActionRowData[IQ.Entities.VastDB.Const.Columns.COL_GUID_URL];
                        clientAction.target = clientActionRowData["target"];

                        result.ClientActionList.Add(clientAction);
                    }
                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }
        }

        private static void AppendChildTableList(IContext context, IQ.Entities.VastDB.SearchResponse searchResponse, ViewModel.Vast.Grid result)
        {
            try
            {
                // Append Child Tables
                if ((searchResponse.ChildTableRowList != null) &&
                    (searchResponse.ChildTableRowList.Count > 0))
                {
                    result.ChildTableList = new List<ViewModel.Vast.ChildTable>();

                    int viewNameIndex = 1;

                    foreach (string[] childTableRow in searchResponse.ChildTableRowList)
                    {
                        Dictionary<string, string> childTableRowData = searchResponse.ReadTableRow(searchResponse.ChildTableRowTable, childTableRow);

                        ViewModel.Vast.ChildTable childTable = new ViewModel.Vast.ChildTable();

                        childTable.TableID = childTableRowData["TableID"];
                        childTable.ActionType = childTableRowData["ActionType"];
                        childTable.CustomAssemblyLineName = childTableRowData["CustomAssemblyLineName"];
                        childTable.FilterFieldName = childTableRowData["FilterFieldName"];
                        childTable.NewDisplayName = childTableRowData["NewDisplayName"];
                        childTable.ViewName = "ChildTable" + viewNameIndex;

                        viewNameIndex++;

                        result.ChildTableList.Add(childTable);
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
        /// Purpose:        Sets default paging values.
        /// </summary>        
        private static void SetPagingDefaultValues(ViewModel.Vast.EntitySearch request, ViewModel.Vast.Grid result)
        {
            if (request == null)
            {
                result.CurrentPage = 1;
                result.RowsPerPage = 10;
            }
            else
            {
                if (request.Page == null)
                {
                    result.CurrentPage = 1;
                    result.RowsPerPage = 10;
                }
                else
                {
                    result.CurrentPage = (int)request.Page;
                    result.RowsPerPage = (int)request.RowsPerPage;
                }
            }
        }

        /// <summary>
        /// Created:        07/17/2015
        /// Author:         David J. McKee
        /// Purpose:        Appends total pages to the response.
        /// </summary>        
        private static void AppendTotalPages(IQ.Entities.VastDB.SearchResponse searchResponse, ViewModel.Vast.Grid result)
        {
            if (searchResponse.TotalRows > 0)
            {
                result.TotalRows = searchResponse.TotalRows;
                result.TotalPages = (result.TotalRows / result.RowsPerPage);

                if ((result.TotalRows % result.RowsPerPage) > 0)
                {
                    result.TotalPages++;
                }
            }
        }

        /// <summary>
        /// Created:        07/17/2015
        /// Author:         David J. McKee
        /// Purpose:        Appends the table schema to the repsonse
        /// </summary>        
        private static void AppendTableSchema(IContext context,
                                                IQ.Entities.VastDB.SearchResponse searchResponse,
                                                ViewModel.Vast.Grid result)
        {
            try
            {
                if (searchResponse.Table != null)
                {
                    if (searchResponse.Table.ColumnList != null)
                    {
                        foreach (IQ.Entities.VastMetaDB.Column column in searchResponse.Table.ColumnList)
                        {
                            ViewModel.Vast.FieldSchema columnSchema = IQ.BUS.Vast.Common.DataHelper.ViewModelColumnSchemaFromDbColumnSchema(context, column.Schema);

                            if (column.Schema != null)
                            {
                                columnSchema.Hidden = !column.Schema.GridVisible;
                            }

                            result.FieldSchemaList.Add(columnSchema);
                        }
                    }

                    // Set the primary key column id
                    result.PrimaryFieldID = searchResponse.Table.PrimaryKeyColumnID;
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
        /// Purpose:        Append Row Data to the respone
        /// </summary>        
        private static void AppendRowData(IQ.Entities.VastDB.SearchResponse searchResponse, ViewModel.Vast.Grid result)
        {
            // Add Rows
            if (result.RowList == null)
            {
                result.RowList = new List<string[]>();
            }

            if ((searchResponse.RowList != null) &&
                (searchResponse.RowList.Count > 0))
            {
                foreach (string[] row in searchResponse.RowList)
                {
                    result.RowList.Add(row);
                }
            }
        }

        /// <summary>
        /// Created:        07/17/2015
        /// Author:         David J. McKee
        /// Purpose:        Maps the filter to the where clause.
        /// </summary>        
        private static void MapFilterToWhere(IContext context, SAIL.ViewModel.SqlEsque.predicate predicateParent, ViewModel.Vast.Grid result)
        {
            // Set the Filter Value
            IQ.ViewModel.Vast.FieldSchema fieldSchema = result.FieldByID(predicateParent.field);

            if (fieldSchema != null)
            {
                if (fieldSchema.FilterEnabled)
                {
                    fieldSchema.FilterValue = predicateParent.value;
                }
            }

            // Check the Next And
            if (predicateParent.and != null)
            {
                // Warning: Recurive call.
                MapFilterToWhere(context, predicateParent.and, result);
            }
        }

        /// <summary>
        /// Created:        06/30/2015
        /// Author:         David J. McKee
        /// Purpose:              
        /// </summary>        
        private static void MapFilterValueToWhereClause(IContext context, ViewModel.Vast.EntitySearch request, ViewModel.Vast.Grid result)
        {
            try
            {
                if (request.Where != null)
                {
                    MapFilterToWhere(context, request.Where, result);
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
        /// Purpose:        Sets the default filter values.
        /// </summary>        
        private static void SetDefaultFilterValues(IContext context, ViewModel.Vast.EntitySearch request, ViewModel.Vast.Grid result)
        {
            foreach (var column in result.FieldSchemaList)
            {
                if (column.FilterEnabled)
                {
                    if (string.IsNullOrWhiteSpace(column.FilterValue))
                    {
                        if (string.IsNullOrWhiteSpace(column.FilterDefaultValue) == false)
                        {
                            column.FilterValue = column.FilterDefaultValue;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Created:        02/12/2015
        /// Author:         David J. McKee
        /// Purpose:        Convers the data model to the view model.
        /// </summary>        
        public static ViewModel.Vast.Grid ViewModelGridFromDbEntity(IContext context,
                                                                        ViewModel.Vast.EntitySearch request,
                                                                        IQ.Entities.VastDB.SearchResponse searchResponse)
        {
            ViewModel.Vast.Grid result = new ViewModel.Vast.Grid();

            try
            {
                // Add Columns
                result.FieldSchemaList = new List<ViewModel.Vast.FieldSchema>();
                result.DisplayName = searchResponse.Table.DisplayName;
                result.IconName = searchResponse.Table.IconName;
                result.ShowFreeFormSearch = searchResponse.Table.ShowFreeFormSearch;

                result.AllowDragDropReorder = searchResponse.Table.AllowDragDropReorder;
                result.AllowNew = searchResponse.Table.AllowNew;
                result.AllowRead = searchResponse.Table.AllowRead;
                result.AllowDelete = searchResponse.Table.AllowDelete;

                result.ButtonText = searchResponse.Table.ButtonText;
                result.DefaultSortColumn = searchResponse.Table.DefaultSortColumn;
                result.DefaultSortOrder = searchResponse.Table.DefaultSortOrder.ToString();
                result.ViewType = searchResponse.Table.ViewType;

                AppendChildTableList(context, searchResponse, result);

                AppendClientActionList(context, searchResponse, result);

                SetPagingDefaultValues(request, result);

                AppendTotalPages(searchResponse, result);

                AppendTableSchema(context, searchResponse, result);

                AppendRowData(searchResponse, result);

                MapFilterValueToWhereClause(context, request, result);

                SetDefaultFilterValues(context, request, result);
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }

            return result;
        }
    }
}
