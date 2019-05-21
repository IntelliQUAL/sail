using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using SAIL.Framework.Host;
using SAIL.Framework.Host.Enums;

using IQ.ViewModel.Vast;
using IQ.Entities.VastDB.Const;
using IQ.RepositoryInterfaces.Vast;
using IQ.Entities.VastMetaDB.Enums;

namespace IQ.BUS.Vast.Common
{
    public static class DataHelper
    {
        /// <summary>
        /// Created:        06/26/2015
        /// Author:         David J. McKee
        /// Purpose:        Returns a list of field enums based on the given data-type
        /// </summary>        
        private static List<ViewModel.Vast.FieldEnum> ProcessEnum(IContext context, string dbEnum)
        {
            List<ViewModel.Vast.FieldEnum> result = null;

            try
            {
                if (string.IsNullOrWhiteSpace(dbEnum) == false)
                {
                    // Important: dbEnum is in the same format as sysTable.ColumnGroupList
                    Dictionary<string, string> enumList = IQ.BUS.Vast.Common.MetaDataHelper.ParseEncodedList(context, dbEnum);

                    if (enumList.Count > 0)
                    {
                        result = new List<ViewModel.Vast.FieldEnum>();

                        const string COMMA_PLACEHOLDER = "&#44;";

                        foreach (string key in enumList.Keys)
                        {
                            ViewModel.Vast.FieldEnum fieldEnum = new ViewModel.Vast.FieldEnum();

                            fieldEnum.Value = key;
                            fieldEnum.DisplayName = enumList[key].Replace(COMMA_PLACEHOLDER, ",");

                            result.Add(fieldEnum);
                        }
                    }
                }

            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }

            return result;
        }

        /// <summary>
        /// Created:        06/22/2015
        /// Author:         David J. McKee
        /// Purpose:        Converts a DB Column Schema into a View Model Column Schema.      
        /// </summary>        
        public static IQ.ViewModel.Vast.FieldSchema ViewModelColumnSchemaFromDbColumnSchema(IContext context,
                                                                                                IQ.Entities.VastMetaDB.ColumnSchema dbColumnSchema)
        {
            IQ.ViewModel.Vast.FieldSchema vmColumnSchema = new ViewModel.Vast.FieldSchema();

            try
            {
                // Critical Values
                vmColumnSchema.ID = dbColumnSchema.ID;
                vmColumnSchema.OrdinalPosition = dbColumnSchema.OrdinalPosition;
                vmColumnSchema.DisplayName = dbColumnSchema.DisplayName;

                switch (dbColumnSchema.InputType)
                {
                    case IQ.Entities.VastMetaDB.DataInputType.Hidden:
                        vmColumnSchema.Hidden = true;
                        vmColumnSchema.InputType = dbColumnSchema.InputType.ToString();
                        break;

                    case IQ.Entities.VastMetaDB.DataInputType.Lookup:
                    case IQ.Entities.VastMetaDB.DataInputType.LookupWithAdd:
                    case IQ.Entities.VastMetaDB.DataInputType.LookupWithAddDetail:
                    case IQ.Entities.VastMetaDB.DataInputType.Text:                         // Read-only
                    case IQ.Entities.VastMetaDB.DataInputType.Spinner:
                    case IQ.Entities.VastMetaDB.DataInputType.RadioButtons:
                    case IQ.Entities.VastMetaDB.DataInputType.DatePicker:
                    case IQ.Entities.VastMetaDB.DataInputType.TimePicker:
                    case IQ.Entities.VastMetaDB.DataInputType.StarRating:
                    case IQ.Entities.VastMetaDB.DataInputType.CheckBox:
                    case IQ.Entities.VastMetaDB.DataInputType.DropDown:
                    case IQ.Entities.VastMetaDB.DataInputType.MaskedTextBox:
                    case IQ.Entities.VastMetaDB.DataInputType.MultiSelectGrid:
                        vmColumnSchema.InputType = dbColumnSchema.InputType.ToString();
                        break;

                    // Switch to Embedded with Meta data
                    case IQ.Entities.VastMetaDB.DataInputType.Signature:
                    case IQ.Entities.VastMetaDB.DataInputType.FileUpload:
                    case IQ.Entities.VastMetaDB.DataInputType.MemoBox:
                        vmColumnSchema.InputType = IQ.ViewModel.VastDB.Enums.DataInputType.EmbeddedDoc.ToString();
                        vmColumnSchema.Format = dbColumnSchema.InputType.ToString();
                        break;

                    case IQ.Entities.VastMetaDB.DataInputType.UserPicker:
                    case IQ.Entities.VastMetaDB.DataInputType.FormPicker:
                        vmColumnSchema.InputType = IQ.Entities.VastMetaDB.DataInputType.DropDown.ToString();
                        break;

                    default:
                        vmColumnSchema.InputType = IQ.Entities.VastMetaDB.DataInputType.TextBox.ToString();
                        break;
                }

                vmColumnSchema.Enum = ProcessEnum(context, dbColumnSchema.Enum);

                if (dbColumnSchema.FilterEnabled)
                {
                    vmColumnSchema.FilterEnabled = true;

                    // Filter Type
                    switch (dbColumnSchema.FilterType)
                    {
                        case IQ.Entities.VastMetaDB.Enums.FilterType.DateRange:
                        case IQ.Entities.VastMetaDB.Enums.FilterType.DateTimeRange:
                        case IQ.Entities.VastMetaDB.Enums.FilterType.Lookup:
                        case IQ.Entities.VastMetaDB.Enums.FilterType.MultiSelectList:
                        case IQ.Entities.VastMetaDB.Enums.FilterType.SingleSelectList:
                        case IQ.Entities.VastMetaDB.Enums.FilterType.NoFilter:
                            vmColumnSchema.FilterType = dbColumnSchema.FilterType.ToString();
                            break;

                        default:
                            vmColumnSchema.FilterType = IQ.Entities.VastMetaDB.Enums.FilterType.FreeFormSearch.ToString();
                            break;
                    }

                    vmColumnSchema.FilterDefaultValue = dbColumnSchema.FilterDefaultValue;
                }
                else
                {
                    vmColumnSchema.FilterEnabled = false;
                    vmColumnSchema.FilterDefaultValue = string.Empty;
                    vmColumnSchema.FilterType = IQ.ViewModel.Vast.Enums.FilterType.NoFilter.ToString();
                }

                vmColumnSchema.DateCreated = dbColumnSchema.DateCreated;
                vmColumnSchema.DefaultValue = dbColumnSchema.DefaultValue;
                vmColumnSchema.Description = dbColumnSchema.Description;
                vmColumnSchema.HelpTitle = dbColumnSchema.HelpTitle;
                vmColumnSchema.HelpText = dbColumnSchema.HelpText;
                vmColumnSchema.ForeignModuleID = dbColumnSchema.ForeignModuleID;
                vmColumnSchema.ForeignTableID = dbColumnSchema.ForeignTableID;
                vmColumnSchema.ForeignTableSearchCriteria = dbColumnSchema.ForeignTableSearchCriteria;
                vmColumnSchema.GroupID = dbColumnSchema.GroupID;
                vmColumnSchema.Min = dbColumnSchema.Min;
                vmColumnSchema.Max = dbColumnSchema.Max;
                vmColumnSchema.DataType = dbColumnSchema.DataType.ToString();
                vmColumnSchema.Placeholder = dbColumnSchema.Placeholder;
                vmColumnSchema.ErrorMessage = dbColumnSchema.ErrorMessage;
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }

            return vmColumnSchema;
        }

        /// <summary>
        /// Created:        07/17/2015
        /// Author:         David J. McKee
        /// Purpose:        Ensures a column row exists.
        /// </summary>        
        private static ViewModel.Vast.FieldGroup EnsureColumnGroup(IContext context, ViewModel.Vast.Entity newResponse, string groupId)
        {
            ViewModel.Vast.FieldGroup columnGroup = null;

            foreach (ViewModel.Vast.FieldGroup colGroup in newResponse.FieldGroupList)
            {
                if (colGroup.ID == groupId)
                {
                    columnGroup = colGroup;
                    break;
                }
            }

            if (columnGroup == null)
            {
                columnGroup = new ViewModel.Vast.FieldGroup();
                columnGroup.ID = groupId;

                if (newResponse.FieldGroupList == null)
                {
                    newResponse.FieldGroupList = new List<ViewModel.Vast.FieldGroup>();
                }

                newResponse.FieldGroupList.Add(columnGroup);
            }

            return columnGroup;
        }

        /// <summary>
        /// Created:        06/19/2015
        /// Author:         David J. McKee
        /// Purpose:        Responsible for returning data which indicates how many columns should be displayed horizontally. 
        /// Important:      Values may come in as "3" (one item in row) or "3.1", "3.2", "3.3" which would indicates (three items in row), or "3" and "3" which would indicate three items in row.
        /// </summary>        
        private static ViewModel.Vast.FieldRow EnsureColumnRow(IContext context,
                                                                ViewModel.Vast.FieldGroup columnGroup,
                                                                string displaySequence,
                                                                out int colOrdinalPosition)
        {
            colOrdinalPosition = 0;
            ViewModel.Vast.FieldRow columnRow = null;

            int ordinalPosition = 0;

            double ordinalPositionDouble;
            if (double.TryParse(displaySequence, out ordinalPositionDouble))
            {
                string[] parts = displaySequence.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
                ordinalPosition = Convert.ToInt32(parts[0]);

                if (parts.Length > 1)
                {
                    colOrdinalPosition = Convert.ToInt32(parts[1]);
                }
            }

            if (columnGroup.FieldRowList != null)
            {
                foreach (ViewModel.Vast.FieldRow colRow in columnGroup.FieldRowList)
                {
                    if (colRow.OrdinalPosition == ordinalPosition)
                    {
                        columnRow = colRow;
                        break;
                    }
                }
            }

            if (columnRow == null)
            {
                columnRow = new ViewModel.Vast.FieldRow();
                columnRow.OrdinalPosition = ordinalPosition;

                if (columnGroup.FieldRowList == null)
                {
                    columnGroup.FieldRowList = new List<ViewModel.Vast.FieldRow>();
                }

                columnGroup.FieldRowList.Add(columnRow);
            }

            return columnRow;
        }

        /// <summary>
        /// Created:        04/13/2015
        /// Author:         David J. McKee
        /// Purpose:        Adds the given column to the appropriate column group.
        /// </summary>        
        private static void AddColumn(IContext context,
                                        ViewModel.Vast.Entity newResponse,
                                        ViewModel.Vast.Field viewModelColumn,
                                        IQ.Entities.VastMetaDB.Column dataModelColumn)
        {
            try
            {
                string groupId = dataModelColumn.Schema.GroupID;

                if (string.IsNullOrWhiteSpace(groupId))
                {
                    const string DEFAULT_GROUP_ID = "Default";

                    groupId = DEFAULT_GROUP_ID;
                }

                ViewModel.Vast.FieldGroup columnGroup = EnsureColumnGroup(context, newResponse, groupId);

                if (string.IsNullOrWhiteSpace(columnGroup.DisplayName))
                {
                    columnGroup.DisplayName = columnGroup.ID;
                }

                int ordinalPosition;
                ViewModel.Vast.FieldRow columnRow = EnsureColumnRow(context, columnGroup, dataModelColumn.Schema.DisplaySequence, out ordinalPosition);
                viewModelColumn.Schema.OrdinalPosition = ordinalPosition;

                if (columnRow.FieldList == null)
                {
                    columnRow.FieldList = new List<ViewModel.Vast.Field>();
                }

                columnRow.FieldList.Add(viewModelColumn);
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }
        }

        /// <summary>
        /// Created:        02/02/2015
        /// Author:         David J. McKee
        /// Purpose:        Copies all values from the IQ.Entities.VastDB.NewResponse to IQ.ViewModel.VastDB.Entity
        /// </summary>        
        private static void ViewModelFromDbTable(IContext context,
                                                        IQ.Entities.VastMetaDB.Table responseTable,
                                                        ViewModel.Vast.Entity newResponse,
                                                        ViewModel.Vast.Entity request)
        {
            try
            {
                if (responseTable != null)
                {
                    newResponse.DisplayName = responseTable.DisplayName;
                    newResponse.ButtonText = responseTable.ButtonText;
                    newResponse.FieldGroupList = new List<ViewModel.Vast.FieldGroup>();

                    if (string.IsNullOrWhiteSpace(responseTable.ColumnGroupIdList) == false)
                    {
                        Dictionary<string, string> groupList = IQ.BUS.Vast.Common.MetaDataHelper.ParseEncodedList(context, responseTable.ColumnGroupIdList);

                        foreach (string key in groupList.Keys)
                        {
                            IQ.ViewModel.Vast.FieldGroup newGroup = new IQ.ViewModel.Vast.FieldGroup();

                            newGroup.ID = key;
                            newGroup.DisplayName = groupList[key];

                            newResponse.FieldGroupList.Add(newGroup);
                        }
                    }

                    if (responseTable.ColumnList != null)
                    {
                        foreach (IQ.Entities.VastMetaDB.Column column in responseTable.ColumnList)
                        {
                            ViewModel.Vast.Field col = new ViewModel.Vast.Field();

                            col.ID = column.ID;

                            col.Schema = ViewModelColumnSchemaFromDbColumnSchema(context, column.Schema);

                            try
                            {
                                col.SensitiveValue = responseTable.GetColumn(column.ID).SensitiveValue;
                            }
                            catch (System.Exception ex2)
                            {
                                context.Get<IExceptionHandler>().HandleException(context, ex2);
                            }

                            AddColumn(context, newResponse, col, column);
                        }
                    }

                    newResponse.DomainName = request.DomainName;

                    if (string.IsNullOrWhiteSpace(responseTable.AccountCode))
                    {
                        newResponse.AccountCode = request.AccountCode;
                    }
                    else
                    {
                        newResponse.AccountCode = responseTable.AccountCode;
                    }

                    if (string.IsNullOrWhiteSpace(responseTable.DatabaseName))
                    {
                        newResponse.ModuleID = context.GetByName(Context.DATABASE_ID).ToString();
                    }
                    else
                    {
                        newResponse.ModuleID = responseTable.DatabaseName;
                    }

                    newResponse.FormID = responseTable.ID;
                    newResponse.PrimaryKey = Guid.NewGuid().ToString();
                    newResponse.TransactionID = Guid.NewGuid().ToString();    // Used to tie multiple submissions together.
                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }
        }

        public static string BuildAuthToken(IContext context,
                                            string passwordHash, string sysLoginPk, string logSetting)
        {
            string authToken = string.Empty;

            const int AUTH_TOKEN_TIMEOUT_HOURS = 16;

            ICsvIO csvIo = context.Get<ICsvIO>();

            DateTime authTokenTimeout = DateTime.UtcNow.AddHours(AUTH_TOKEN_TIMEOUT_HOURS);

            Dictionary<string, string> authTokenValues = new Dictionary<string, string>();
            authTokenValues.Add(Columns.COL_GUID_PASSWORD_HASH, passwordHash);
            authTokenValues.Add(Consts.Params.AUTH_TOKEN_TIMEOUT, authTokenTimeout.ToString("u")); // UTC format.
            authTokenValues.Add(Columns.COL_GUID_SYS_LOGIN_ID, sysLoginPk);
            authTokenValues.Add(Columns.COL_GUID_LOG_SETTING, logSetting);

            string authTokenData = csvIo.BuildCsvLine(authTokenValues);

            authToken = Helpers.AuthHelper.EncryptAuth(context, authTokenData);

            return authToken;
        }

        private static void EnsureAuthToken(IContext context, ViewModel.Vast.Entity newResponse)
        {
            if (string.IsNullOrWhiteSpace(newResponse.AuthToken))
            {
                // Rebuild it from the component parts.
                if ((context.GetByName(Context.PASSWORD_HASH) != null) &&
                    (context.GetByName(Context.SYS_LOGIN_ID) != null))
                {
                    string passwordHash = context.GetByName(Context.PASSWORD_HASH).ToString();
                    string sysLoginId = context.GetByName(Context.SYS_LOGIN_ID).ToString();
                    string logSetting = context.GetByName(Context.LOG_SETTING).ToString();

                    newResponse.AuthToken = BuildAuthToken(context, passwordHash, sysLoginId, logSetting);
                }
            }
        }

        // Copy all data from the database model to the view model.
        private static void CopyDbDataToViewModel(IContext context,
                                                    IQ.Entities.VastDB.Entity dbModelResponse,
                                                    ViewModel.Vast.Entity newResponse)
        {
            try
            {
                if (dbModelResponse.ColumnIdValuePair != null)
                {
                    foreach (string key in dbModelResponse.ColumnIdValuePair.Keys)
                    {
                        ViewModel.Vast.Field col = newResponse.FieldByGuid(key);

                        if (col != null)
                        {
                            col.Value = dbModelResponse.GetColumnValue(key);

                            try
                            {
                                Entities.VastMetaDB.Column column = dbModelResponse.Table.GetColumn(key);
                                col.SensitiveValue = column.SensitiveValue;
                            }
                            catch (System.Exception ex2)
                            {
                                context.Get<IExceptionHandler>().HandleException(context, ex2);
                            }
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
        /// Purpose:        Sets the column's default value.
        /// </summary>
        /// <param name="newResponse"></param>
        private static void SetDefaultValues(ViewModel.Vast.Entity newResponse)
        {
            if (newResponse.FieldGroupList != null)
            {
                foreach (var columnGroup in newResponse.FieldGroupList)
                {
                    if (columnGroup.FieldRowList != null)
                    {
                        foreach (var columnRowList in columnGroup.FieldRowList)
                        {
                            if (columnRowList.FieldList != null)
                            {
                                foreach (var column in columnRowList.FieldList)
                                {
                                    if (string.IsNullOrWhiteSpace(column.Value))
                                    {
                                        if (column.Schema != null)
                                        {
                                            column.Value = column.Schema.DefaultValue;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Created:        02/02/2015
        /// Author:         David J. McKee
        /// Purpose:        Copies all values from the IQ.Entities.VastDB.NewResponse to IQ.ViewModel.VastDB.Entity
        /// </summary>        
        public static void ViewModelEntityFromDbEntity(IContext context,
                                                        ViewModel.Vast.Entity request,
                                                        IQ.Entities.VastDB.Entity dbModelResponse,
                                                        ViewModel.Vast.Entity newResponse)
        {
            try
            {
                // Set the Default FormID
                if (string.IsNullOrWhiteSpace(dbModelResponse.TableID))
                {
                    dbModelResponse.TableID = request.FormID;
                }

                // Set the default ActionResponseType
                if (string.IsNullOrWhiteSpace(dbModelResponse.ActionResponseType))
                {
                    dbModelResponse.ActionResponseType = request.ActionResponseType;
                }

                newResponse.ViewType = dbModelResponse.Table.ViewType;

                // Start by Copying all table information.
                // This puts the ViewModel in the same state as it would be after a "New" request.
                ViewModelFromDbTable(context, dbModelResponse.Table, newResponse, request);

                // Next, copy all the data.
                CopyDbDataToViewModel(context, dbModelResponse, newResponse);

                // Set default values
                SetDefaultValues(newResponse);

                newResponse.PrimaryKey = dbModelResponse.PrimaryKey;
                newResponse.FormID = dbModelResponse.TableID;

                // Critical: The ActionType must be returned.
                newResponse.ActionResponseType = dbModelResponse.ActionResponseType;
                newResponse.SpecificActionResponseType = dbModelResponse.SpecificActionResponseType;

                // Only used on the client in conjunction with ActionResponseType = IQ.Entities.VastDB.Const.Actions.ACTION_SPECIFIC
                newResponse.CustomAssemblyLineName = dbModelResponse.CustomAssemblyLineName;

                SetSearchInfo(context, newResponse, dbModelResponse);

                if (string.IsNullOrWhiteSpace(newResponse.ButtonText))
                {
                    newResponse.ButtonText = "Save";
                }

                EnsureAuthToken(context, newResponse);

                EnsureGridSystem(context, dbModelResponse, newResponse);

                RemoveEmptyGroups(context, newResponse);

                IdentifyHiddenGroups(context, newResponse);

                SetTabIndex(context, newResponse);

                AppendAttributes(context, dbModelResponse, newResponse);
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }
        }

        private static void AppendAttributes(IContext context, Entities.VastDB.Entity dbModelResponse, Entity newResponse)
        {
            try
            {
                newResponse.Attributes = new AttributeData();

                newResponse.Attributes.FieldSchemaList = new List<FieldSchema>();

                FieldSchema key = new FieldSchema();
                key.OrdinalPosition = 0;
                key.ID = Columns.COL_GUID_KEY;

                newResponse.Attributes.FieldSchemaList.Add(key);

                FieldSchema value = new FieldSchema();
                value.OrdinalPosition = 1;
                value.ID = Columns.COL_GUID_VALUE;

                newResponse.Attributes.FieldSchemaList.Add(value);

                newResponse.Attributes.RowList = dbModelResponse.Table.AttributeData.RowList;

                // Check for ViewIFrameSrc because we need to switch on the UI
                const int KEY_COL = 0;
                const int VALUE_COL = 1;
                const string KEY_VIEW_IFRAME_SOURCE_URL = "ViewIFrameSrc";

                bool found = false;

                foreach (string[] row in newResponse.Attributes.RowList)
                {
                    string keyName = row[KEY_COL];

                    if (keyName == KEY_VIEW_IFRAME_SOURCE_URL)
                    {
                        found = true;
                    }
                }

                if (found == false)
                {
                    string[] viewIFrameSrc = new string[2];

                    viewIFrameSrc[KEY_COL] = KEY_VIEW_IFRAME_SOURCE_URL;
                    viewIFrameSrc[VALUE_COL] = string.Empty;

                    newResponse.Attributes.RowList.Add(viewIFrameSrc);
                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }
        }

        /// <summary>
        /// Created:        02/12/2016
        /// Author:         David J. McKee
        /// Purpose:        Allows a search request to be sent back to the UI for the next call.
        /// Important:      Only used on the client in conjunction with ActionResponseType = IQ.Entities.VastDB.Const.Actions.ACTION_SPECIFIC
        /// </summary>        
        private static void SetSearchInfo(IContext context, Entity newResponse, Entities.VastDB.Entity dbModelResponse)
        {
            try
            {
                if (dbModelResponse.SearchInfo != null)
                {
                    // Big assumption: The searialized 'Where' objects are compatible.
                    string whereJson = context.Get<IResponseHelper>().ObjectToString(dbModelResponse.SearchInfo, ResponseFormat.JSON);

                    // Warning: This is a bit of a hack but the two object models are very similar so we are making them match by replacing strings.
                    whereJson = whereJson.Replace("\"ColumnName\"", "\"FieldName\"");
                    whereJson = whereJson.Replace("\"OperatorType\": 1,", "\"OperatorType\": \"EqualTo\",");

                    newResponse.SearchInfo =
                        (IQ.ViewModel.Vast.EntitySearch)context.Get<IRequestHelper>().StringToObject<IQ.ViewModel.Vast.EntitySearch>(whereJson, ResponseFormat.JSON);
                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }
        }

        /// <summary>
        /// Created:        01/27/2016
        /// Author:         David J. McKee
        /// Purpose:        Sets the tab index from top to bottom, left to right
        /// Assumptions:    Specifies the tabbing order of the element (1 is first)
        ///                 This enhancement was due to a problem where we required two tabs to move to the next field.
        /// </summary>
        private static void SetTabIndex(IContext context, Entity newResponse)
        {
            try
            {
                int tabIndex = 1;   // should always start a 1

                if (newResponse.FieldGroupList != null)
                {
                    foreach (FieldGroup fieldGroup in newResponse.FieldGroupList)
                    {
                        SetTabIndexForGroup(fieldGroup, ref tabIndex);
                    }
                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }
        }

        private static void SetTabIndexForGroup(FieldGroup fieldGroup, ref int tabIndex)
        {
            if (fieldGroup.FieldRowList != null)
            {
                foreach (FieldRow fieldRow in fieldGroup.FieldRowList)
                {
                    if (fieldRow.FieldList != null)
                    {
                        SetTabIndexForRow(fieldRow, ref tabIndex);
                    }
                }
            }
        }

        private static void SetTabIndexForRow(FieldRow fieldRow, ref int tabIndex)
        {
            foreach (Field field in fieldRow.FieldList)
            {
                // Skip tabs for these items.
                if ((field.Schema.InputType == IQ.ViewModel.VastDB.Enums.DataInputType.Hidden.ToString()) ||
                    (field.Schema.InputType == IQ.ViewModel.VastDB.Enums.DataInputType.Text.ToString()))
                {
                    // Do nothing. This will cause the tabs to be skipped
                }
                else
                {
                    field.Schema.Layout.TabIndex = tabIndex.ToString();
                    tabIndex++;
                }
            }
        }

        /// <summary>
        /// Created:        01/24/2016
        /// Author:         David J. McKee
        /// Purpose:        Removes all FieldRows without fields. Removed all field groups without field rows.
        /// </summary>        
        private static void IdentifyHiddenGroups(IContext context, Entity newResponse)
        {
            try
            {
                if ((newResponse.FieldGroupList != null) &&
                    (newResponse.FieldGroupList.Count > 0))
                {
                    foreach (FieldGroup fieldGroup in newResponse.FieldGroupList)
                    {
                        bool visibleFieldFound = false;

                        if ((fieldGroup.FieldRowList != null) ||
                            (fieldGroup.FieldRowList.Count >= 0))
                        {
                            foreach (FieldRow fieldRow in fieldGroup.FieldRowList)
                            {
                                if ((fieldRow.FieldList != null) ||
                                    (fieldRow.FieldList.Count > 0))
                                {
                                    foreach (Field field in fieldRow.FieldList)
                                    {
                                        if (field.Schema.InputType != IQ.ViewModel.VastDB.Enums.DataInputType.Hidden.ToString())
                                        {
                                            visibleFieldFound = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        if (visibleFieldFound == false)
                        {
                            // This group only contains hidden fields.
                            fieldGroup.Hidden = true;
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
        /// Created:        01/24/2016
        /// Author:         David J. McKee
        /// Purpose:        Removes all FieldRows without fields. Removed all field groups without field rows.
        /// </summary>        
        private static void RemoveEmptyGroups(IContext context, Entity newResponse)
        {
            try
            {
                // Remove Unused FieldRows
                if (newResponse.FieldGroupList != null)
                {
                    foreach (FieldGroup fieldGroup in newResponse.FieldGroupList)
                    {
                        bool fieldRowRemoved = true;

                        while (fieldRowRemoved)
                        {
                            fieldRowRemoved = false;

                            if (fieldGroup.FieldRowList != null)
                            {
                                foreach (FieldRow fieldRow in fieldGroup.FieldRowList)
                                {
                                    if ((fieldRow.FieldList == null) ||
                                        (fieldRow.FieldList.Count == 0))
                                    {
                                        fieldGroup.FieldRowList.Remove(fieldRow);
                                        fieldRowRemoved = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }

                // Remove unused FieldGroups
                bool fieldGroupRemoved = true;

                while (fieldGroupRemoved)
                {
                    fieldGroupRemoved = false;

                    if (newResponse.FieldGroupList != null)
                    {
                        foreach (FieldGroup fieldGroup in newResponse.FieldGroupList)
                        {
                            if ((fieldGroup.FieldRowList == null) ||
                                (fieldGroup.FieldRowList.Count == 0))
                            {
                                newResponse.FieldGroupList.Remove(fieldGroup);
                                fieldGroupRemoved = true;
                                break;
                            }
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
        /// Created:        01/24/2016
        /// Author:         David J. McKee
        /// Purpose:        We use a 12 col grid system just like bootstap. This method ensures all layout values are valid.
        /// </summary>        
        private static void EnsureGridSystem(IContext context, Entities.VastDB.Entity dbModelResponse, ViewModel.Vast.Entity newResponse)
        {
            try
            {
                if (newResponse.FieldGroupList != null)
                {
                    foreach (FieldGroup fieldGroup in newResponse.FieldGroupList)
                    {
                        if (fieldGroup.FieldRowList != null)
                        {
                            foreach (FieldRow fieldRow in fieldGroup.FieldRowList)
                            {
                                if (fieldRow.FieldList.Count == 1)
                                {
                                    foreach (Field field in fieldRow.FieldList)
                                    {
                                        field.Schema.Layout.xs = "12";
                                        field.Schema.Layout.sm = "12";
                                        field.Schema.Layout.md = "12";
                                        field.Schema.Layout.lg = "12";
                                    }
                                }
                                else
                                {
                                    foreach (Field field in fieldRow.FieldList)
                                    {
                                        SetXsGridLayout(context, field, fieldRow, dbModelResponse);
                                        SetSmGridLayout(context, field, fieldRow, dbModelResponse);
                                        SetMdGridLayout(context, field, fieldRow, dbModelResponse);
                                        SetLgGridLayout(context, field, fieldRow, dbModelResponse);
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
        }

        private static void SetLgGridLayout(IContext context, Field field, FieldRow fieldRow, Entities.VastDB.Entity dbModelResponse)
        {

            try
            {
                if (string.IsNullOrWhiteSpace(field.Schema.Layout.lg))
                {
                    string columnSchemaDefinedValue = dbModelResponse.Table.GetColumn(field.ID).Schema.Layout.lg;

                    if (string.IsNullOrWhiteSpace(columnSchemaDefinedValue))
                    {
                        switch (fieldRow.FieldList.Count)
                        {
                            case 1:
                                field.Schema.Layout.lg = "12";
                                break;

                            case 2:
                                field.Schema.Layout.lg = "6";
                                break;

                            case 3:
                                field.Schema.Layout.lg = "4";
                                break;

                            case 4:
                                field.Schema.Layout.lg = "3";
                                break;

                            case 5:
                                field.Schema.Layout.lg = "2";
                                break;

                            case 6:
                                field.Schema.Layout.lg = "2";
                                break;

                            case 7:
                                field.Schema.Layout.lg = "1";
                                break;

                            case 8:
                                field.Schema.Layout.lg = "1";
                                break;

                            case 9:
                                field.Schema.Layout.lg = "1";
                                break;

                            case 10:
                                field.Schema.Layout.lg = "1";
                                break;

                            case 11:
                                field.Schema.Layout.lg = "1";
                                break;

                            default:
                                field.Schema.Layout.lg = "1";
                                break;
                        }
                    }
                    else
                    {
                        field.Schema.Layout.lg = columnSchemaDefinedValue;
                    }
                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }
        }

        private static void SetSmGridLayout(IContext context, Field field, FieldRow fieldRow, Entities.VastDB.Entity dbModelResponse)
        {

            try
            {
                if (string.IsNullOrWhiteSpace(field.Schema.Layout.sm))
                {
                    string columnSchemaDefinedValue = dbModelResponse.Table.GetColumn(field.ID).Schema.Layout.sm;

                    if (string.IsNullOrWhiteSpace(columnSchemaDefinedValue))
                    {
                        switch (fieldRow.FieldList.Count)
                        {
                            case 1:
                                field.Schema.Layout.sm = "12";
                                break;

                            case 2:
                                field.Schema.Layout.sm = "6";
                                break;

                            case 3:
                                field.Schema.Layout.sm = "4";
                                break;

                            case 4:
                                field.Schema.Layout.sm = "3";
                                break;

                            case 5:
                                field.Schema.Layout.sm = "3";
                                break;

                            case 6:
                                field.Schema.Layout.sm = "3";
                                break;

                            case 7:
                                field.Schema.Layout.sm = "3";
                                break;

                            case 8:
                                field.Schema.Layout.sm = "3";
                                break;

                            case 9:
                                field.Schema.Layout.sm = "3";
                                break;

                            case 10:
                                field.Schema.Layout.sm = "3";
                                break;

                            case 11:
                                field.Schema.Layout.sm = "3";
                                break;

                            default:
                                field.Schema.Layout.sm = "3";
                                break;

                        }
                    }
                    else
                    {
                        field.Schema.Layout.sm = columnSchemaDefinedValue;
                    }
                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }
        }

        private static void SetMdGridLayout(IContext context, Field field, FieldRow fieldRow, Entities.VastDB.Entity dbModelResponse)
        {

            try
            {
                if (string.IsNullOrWhiteSpace(field.Schema.Layout.md))
                {
                    string columnSchemaDefinedValue = dbModelResponse.Table.GetColumn(field.ID).Schema.Layout.md;

                    if (string.IsNullOrWhiteSpace(columnSchemaDefinedValue))
                    {
                        switch (fieldRow.FieldList.Count)
                        {
                            case 1:
                                field.Schema.Layout.md = "12";
                                break;

                            case 2:
                                field.Schema.Layout.md = "6";
                                break;

                            case 3:
                                field.Schema.Layout.md = "4";
                                break;

                            case 4:
                                field.Schema.Layout.md = "3";
                                break;

                            case 5:
                                field.Schema.Layout.md = "2";
                                break;

                            case 6:
                                field.Schema.Layout.md = "2";
                                break;

                            case 7:
                                field.Schema.Layout.md = "2";
                                break;

                            case 8:
                                field.Schema.Layout.md = "2";
                                break;

                            case 9:
                                field.Schema.Layout.md = "2";
                                break;

                            case 10:
                                field.Schema.Layout.md = "2";
                                break;

                            case 11:
                                field.Schema.Layout.md = "2";
                                break;

                            default:
                                field.Schema.Layout.md = "2";
                                break;
                        }
                    }
                    else
                    {
                        field.Schema.Layout.md = columnSchemaDefinedValue;
                    }
                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }
        }

        private static void SetXsGridLayout(IContext context, Field field, FieldRow fieldRow, Entities.VastDB.Entity dbModelResponse)
        {

            try
            {
                if (string.IsNullOrWhiteSpace(field.Schema.Layout.xs))
                {
                    string columnSchemaDefinedValue = dbModelResponse.Table.GetColumn(field.ID).Schema.Layout.xs;

                    if (string.IsNullOrWhiteSpace(columnSchemaDefinedValue))
                    {
                        switch (fieldRow.FieldList.Count)
                        {
                            case 1:
                                field.Schema.Layout.xs = "12";
                                break;

                            case 2:
                                field.Schema.Layout.xs = "6";
                                break;

                            case 3:
                                field.Schema.Layout.xs = "4";
                                break;

                            case 4:
                                field.Schema.Layout.xs = "4";
                                break;

                            case 5:
                                field.Schema.Layout.xs = "4";
                                break;

                            case 6:
                                field.Schema.Layout.xs = "4";
                                break;

                            case 7:
                                field.Schema.Layout.xs = "4";
                                break;

                            case 8:
                                field.Schema.Layout.xs = "4";
                                break;

                            case 9:
                                field.Schema.Layout.xs = "4";
                                break;

                            case 10:
                                field.Schema.Layout.xs = "4";
                                break;

                            case 11:
                                field.Schema.Layout.xs = "4";
                                break;

                            default:
                                field.Schema.Layout.xs = "4";
                                break;

                        }
                    }
                    else
                    {
                        field.Schema.Layout.xs = columnSchemaDefinedValue;
                    }
                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }
        }

        public static IQ.Entities.VastMetaDB.Column AppendColumn(string tableName,
                                                                    IQ.Entities.VastMetaDB.Table newTable,
                                                                    int ordinalPosition,
                                                                    string columnName,
                                                                    bool primaryKey)
        {
            IQ.Entities.VastMetaDB.Column primaryKeyColumn = new Entities.VastMetaDB.Column(ordinalPosition, columnName);
            //primaryKeyColumn.ID = newTable.PrimaryKeyColumnID;                                    // Set by constructor
            primaryKeyColumn.Schema = new Entities.VastMetaDB.ColumnSchema();

            if (primaryKey)
            {
                primaryKeyColumn.Schema.DataType = Entities.VastMetaDB.Enums.ColumnDataType.PrimaryKey;
            }
            else
            {
                primaryKeyColumn.Schema.DataType = Entities.VastMetaDB.Enums.ColumnDataType.String;
            }

            primaryKeyColumn.Schema.DateCreated = DateTime.Now;
            primaryKeyColumn.Schema.DefaultValue = null;
            primaryKeyColumn.Schema.Description = string.Empty;
            primaryKeyColumn.Schema.HelpTitle = string.Empty;
            primaryKeyColumn.Schema.HelpText = string.Empty;
            primaryKeyColumn.Schema.DisplayName = primaryKeyColumn.ID;
            primaryKeyColumn.Schema.DisplaySequence = primaryKeyColumn.Schema.OrdinalPosition.ToString();
            primaryKeyColumn.Schema.ForeignModuleID = string.Empty;
            primaryKeyColumn.Schema.ForeignTableID = string.Empty;
            primaryKeyColumn.Schema.ForeignTableSearchCriteria = string.Empty;
            primaryKeyColumn.Schema.Format = Entities.VastMetaDB.DataFormat.Unformatted;
            primaryKeyColumn.Schema.GroupID = string.Empty;
            primaryKeyColumn.Schema.HasDefaultValue = false;
            primaryKeyColumn.Schema.ID = primaryKeyColumn.ID;
            primaryKeyColumn.Schema.InputType = Entities.VastMetaDB.DataInputType.TextBox;

            if (primaryKey)
            {
                primaryKeyColumn.Schema.Internal = InternalVisibility.Internal;
            }

            primaryKeyColumn.Schema.Max = null;
            primaryKeyColumn.Schema.Min = null;
            //primaryKeyColumn.Schema.OrdinalPosition = 1;                                          // Set by constructor
            primaryKeyColumn.Schema.Pattern = string.Empty;
            primaryKeyColumn.Schema.Required = false;
            primaryKeyColumn.Schema.SortDescending = true;
            primaryKeyColumn.Schema.TableID = tableName;
            primaryKeyColumn.Schema.TrustAsHtml = false;
            primaryKeyColumn.Schema.ValidateMinMax = false;

            if (newTable != null)
            {
                if (newTable.ColumnList == null)
                {
                    newTable.ColumnList = new List<Entities.VastMetaDB.Column>();
                }

                newTable.ColumnList.Add(primaryKeyColumn);
            }

            return primaryKeyColumn;
        }

        /// <summary>
        /// Created:        08/14/2015
        /// Author:         David J. McKee
        /// Purpose:        Ensures a given menu exists based on the information provided.      
        /// </summary>        
        public static IQ.Entities.VastDB.Entity EnsureMenu(IContext context,
                                                            IStandardTable repo,
                                                            string instanceGUID,
                                                            string metabaseName,
                                                            IQ.Entities.VastDB.Entity parentMenu,
                                                            string displaySequence,
                                                            string displayName,
                                                            string formId,
                                                            string actionType,
                                                            string menuStyle,
                                                            string classText,
                                                            string moduleId,
                                                            string sysUserRoleList,
                                                            string accountCode)
        {
            IQ.Entities.VastDB.Entity result = null;

            try
            {
                // Search for an existing menu
                IQ.Entities.VastDB.EntitySearch searchCriteria = new IQ.Entities.VastDB.EntitySearch();

                searchCriteria.Where = new IQ.Entities.VastMetaDB.SqlEsque.Where(new IQ.Entities.VastMetaDB.SqlEsque.Predicate(Columns.COL_GUID_FORM_ID, IQ.Entities.VastMetaDB.Enums.OperatorType.EqualTo, formId));
                searchCriteria.Where.And = new IQ.Entities.VastMetaDB.SqlEsque.AND(new IQ.Entities.VastMetaDB.SqlEsque.Predicate(Columns.COL_GUID_ACTION_TYPE, IQ.Entities.VastMetaDB.Enums.OperatorType.EqualTo, actionType));
                searchCriteria.Page = 1;
                searchCriteria.RowsPerPage = 1;

                IQ.Entities.VastDB.SearchResponse searchResponse = repo.Search(context, instanceGUID, metabaseName, string.Empty, string.Empty, Tables.TABLE_GUID_SYS_MENU, searchCriteria);

                if (searchResponse.RowList.Count == 0)
                {
                    // Insert a new menu
                    result = new IQ.Entities.VastDB.Entity();

                    if (parentMenu != null)
                    {
                        string parentSysMenuId = parentMenu.PrimaryKey;

                        if (string.IsNullOrWhiteSpace(parentSysMenuId))
                        {
                            parentSysMenuId = parentMenu.GetColumnValue(Columns.COL_GUID_SYS_MENU_ID);
                        }

                        result.SetColumnValue(Columns.COL_GUID_PARENT_SYS_MENU_ID, parentSysMenuId);
                    }

                    result.SetColumnValue(Columns.COL_GUID_DISPLAY_SEQUENCE, displaySequence);
                    result.SetColumnValue(Columns.COL_GUID_DISPLAY_NAME, displayName);
                    result.SetColumnValue(Columns.COL_GUID_FORM_ID, formId);
                    result.SetColumnValue(Columns.COL_GUID_ACTION_TYPE, actionType);
                    result.SetColumnValue(Columns.COL_GUID_MENU_STYLE, menuStyle);
                    result.SetColumnValue(Columns.COL_GUID_CLASS_TEXT, classText);
                    result.SetColumnValue(Columns.COL_GUID_MODULE_ID, moduleId);
                    result.SetColumnValue(Columns.COL_GUID_USER_ROLE_LIST, sysUserRoleList);
                    result.SetColumnValue(Columns.COL_GUID_ACCOUNT_CODE, accountCode);

                    repo.Create(context, instanceGUID, metabaseName, string.Empty, string.Empty, Tables.TABLE_GUID_SYS_MENU, result);
                }
                else
                {
                    result = new IQ.Entities.VastDB.Entity();
                    result.ColumnIdValuePair = searchResponse.ReadRow(searchResponse.RowList[0]);
                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }

            return result;
        }

        public static void EnsureTierTwoSysTableSchema(IContext context, string instanceGUID, string metabaseName, string formName,
                                                            IQ.Entities.VastDB.Entity parentMenu,
                                                            IStandardTable repo, string accountCode)
        {
            // *ColumnSchema
            string columnSchemaTablename = formName + MetaDataHelper.SCHEMA_TABLE_SUFFIX;
            string menuAndSearchDisplayName = formName + " Fields";

            if (parentMenu != null)
            {
                IQ.Entities.VastDB.Entity editFields =
                    EnsureMenu(context, repo, instanceGUID, metabaseName, parentMenu, "1", menuAndSearchDisplayName,
                                columnSchemaTablename, Actions.ACTION_SEARCH,
                                KnownValues.MENU_STYLE_HORIZONTAL, string.Empty, metabaseName,
                                KnownValues.KNOWN_VALUE_SYSTEM_ROLE_SUPER_ADMIN, accountCode);
            }

            // Now ensure a sysTableSchema entry in the *MetabaseMetabase poining back to the Common editior
            string primaryKeyColumnId = columnSchemaTablename + "ID";

            IQ.BUS.Vast.Common.DataHelper.EnsureTierTwoMetabaseSysTableSchemaForCommonDesigner(context, instanceGUID, metabaseName, columnSchemaTablename, primaryKeyColumnId,
                                                                    "New Field", "Save Field", menuAndSearchDisplayName, "General", "New Field",
                                                                    "Common");

            // *ColumnVisibility
            string columnVisibilityTableName = formName + MetaDataHelper.COLUMN_VISIBILITY_TABLE_SUFFIX;

            if (parentMenu != null)
            {
                IQ.Entities.VastDB.Entity editVisibility =
                    EnsureMenu(context, repo, instanceGUID, metabaseName, parentMenu, "2", formName + " Field Visibility",
                                columnVisibilityTableName, Actions.ACTION_SEARCH,
                                KnownValues.MENU_STYLE_HORIZONTAL, string.Empty, metabaseName,
                                KnownValues.KNOWN_VALUE_SYSTEM_ROLE_SUPER_ADMIN, accountCode);
            }

            // Now ensure a sysTableSchema entry in the *MetabaseMetabase poining back to the Common editior
            string columnVisibilityPrimaryKeyColumnId = columnVisibilityTableName + "ID";

            IQ.BUS.Vast.Common.DataHelper.EnsureTierTwoMetabaseSysTableSchemaForCommonDesigner(context, instanceGUID, metabaseName, columnVisibilityTableName, columnVisibilityPrimaryKeyColumnId,
                                                                    "Show / Hide Field", "Save Field Visibility", "Field Visibility", "General", "Show / Hide Field",
                                                                    "Common");

            // Create the ColumnVisibility table
            IQ.BUS.Vast.Common.Helpers.SchemaHelper.EnsureColumnVisibilityTable(context, formName, repo, instanceGUID, metabaseName, repo);

            // Create the ColumnSchema table
            IQ.BUS.Vast.Common.Helpers.SchemaHelper.EnsureColumnSchemaTable(context, formName, repo, instanceGUID, metabaseName, repo);
        }

        public static void EnsureAndSetColumn(IContext context,
                                            string instanceGUID,
                                            string databaeName,
                                            IQ.Entities.VastDB.Entity entity,
                                            string columnName,
                                            string columnValue,
                                            IStandardTable repo)
        {
            if (entity.ColumnIdValuePair.ContainsKey(columnName) == false)
            {
                ITableManager tableManager = (ITableManager)repo;

                IQ.Entities.VastMetaDB.ColumnSchema columnSchema = new IQ.Entities.VastMetaDB.ColumnSchema();
                columnSchema.ID = columnName;

                tableManager.EnsureColumn(context, instanceGUID, databaeName, Tables.TABLE_GUID_SYS_TABLE_SCHEMA, columnSchema);
            }

            entity.SetColumnValue(columnName, columnValue);
        }

        public static void EnsureTierTwoMetabaseSysTableSchemaForCommonDesigner(IContext context,
                                                                                string instanceGUID,
                                                                                string parentMetabaseName,
                                                                                string formName,
                                                                                string primaryKeyColumnId,
                                                                                string newDisplayName,
                                                                                string updateDisplayName,
                                                                                string gridDisplayName,
                                                                                string columnGroupList,
                                                                                string newButtonText,
                                                                                string commonTableName)
        {
            try
            {
                const string COMMON_DATABASE_NAME = "Common";   // Metabase will automatically be added to this name at runtime.

                // Get the Metabase for the parent Metabase
                string metabaseName;
                IStandardTable repo = IQ.BUS.Vast.Common.MetaDataHelper.ReadMetabaseName(context, parentMetabaseName, out metabaseName);

                EnsureSysTableSchema(context, instanceGUID, metabaseName, repo);

                // Search
                IQ.Entities.VastDB.EntitySearch searchCriteria = new IQ.Entities.VastDB.EntitySearch();

                searchCriteria.Where = new IQ.Entities.VastMetaDB.SqlEsque.Where(new IQ.Entities.VastMetaDB.SqlEsque.Predicate(Columns.COL_GUID_ID, IQ.Entities.VastMetaDB.Enums.OperatorType.EqualTo, formName));
                searchCriteria.Page = 1;
                searchCriteria.RowsPerPage = 1;

                IQ.Entities.VastDB.SearchResponse searchResponse = repo.Search(context, instanceGUID, metabaseName, string.Empty, string.Empty, Tables.TABLE_GUID_SYS_TABLE_SCHEMA, searchCriteria);

                IQ.Entities.VastDB.Entity entity = null;

                if (searchResponse.RowList.Count == 0)
                {
                    // Insert
                    entity = repo.New(context, instanceGUID, metabaseName, string.Empty, string.Empty, string.Empty, Tables.TABLE_GUID_SYS_TABLE_SCHEMA);
                    entity.SetColumnValue(Columns.COL_GUID_ID, formName);
                }
                else
                {
                    // Read and Update
                    entity = repo.Read(context, instanceGUID, metabaseName, string.Empty, string.Empty, string.Empty, Tables.TABLE_GUID_SYS_TABLE_SCHEMA, Columns.COL_GUID_ID, formName);
                }

                entity.PrimaryKeyColumnID = Columns.COL_GUID_ID;
                entity.PrimaryKey = formName;

                EnsureAndSetColumn(context, instanceGUID, metabaseName, entity, Columns.COL_GUID_PRIMARY_KEY_COLUMN_ID, primaryKeyColumnId, repo);
                EnsureAndSetColumn(context, instanceGUID, metabaseName, entity, Columns.COL_GUID_NEW_DISPLAY_NAME, newDisplayName, repo);
                EnsureAndSetColumn(context, instanceGUID, metabaseName, entity, Columns.COL_GUID_UPDATE_DISPLAY_NAME, updateDisplayName, repo);
                EnsureAndSetColumn(context, instanceGUID, metabaseName, entity, Columns.COL_GUID_GRID_DISPLAY_NAME, gridDisplayName, repo);
                EnsureAndSetColumn(context, instanceGUID, metabaseName, entity, Columns.COL_GUID_COLUMN_GROUP_ID_LIST, columnGroupList, repo);
                EnsureAndSetColumn(context, instanceGUID, metabaseName, entity, Columns.COL_GUID_NEW_BUTTON_TEXT, newButtonText, repo);
                EnsureAndSetColumn(context, instanceGUID, metabaseName, entity, Columns.COL_GUID_SHOW_FREE_FORM_SEARCH, false.ToString(), repo);

                // Schema
                EnsureAndSetColumn(context, instanceGUID, metabaseName, entity, Columns.COL_GUID_COLUMN_SYS_TABLE_SCHEMA_INSTANCE_GUID, Consts.Root.IQ_CLOUD_DOMAIN, repo);
                EnsureAndSetColumn(context, instanceGUID, metabaseName, entity, Columns.COL_GUID_COLUMN_SYS_TABLE_SCHEMA_DATABASE_NAME, COMMON_DATABASE_NAME, repo);
                EnsureAndSetColumn(context, instanceGUID, metabaseName, entity, Columns.COL_GUID_COLUMN_SYS_TABLE_SCHEMA_TABLE_NAME, commonTableName, repo);

                // Visibility
                EnsureAndSetColumn(context, instanceGUID, metabaseName, entity, Columns.COL_GUID_COLUMN_VISIBILITY_INSTANCE_GUID, Consts.Root.IQ_CLOUD_DOMAIN, repo);
                EnsureAndSetColumn(context, instanceGUID, metabaseName, entity, Columns.COL_GUID_COLUMN_VISIBILITY_DATABASE_NAME, COMMON_DATABASE_NAME, repo);
                EnsureAndSetColumn(context, instanceGUID, metabaseName, entity, Columns.COL_GUID_COLUMN_VISIBILITY_TABLE_NAME, commonTableName, repo);

                EnsureAndSetColumn(context, instanceGUID, metabaseName, entity, Columns.COL_GUID_TABLE_DATA_TYPE, IQ.Entities.VastMetaDB.TableDataType.Standard.ToString(), repo);
                EnsureAndSetColumn(context, instanceGUID, metabaseName, entity, Columns.COL_GUID_TABLE_DATA_TYPE_CODE_COLUMN_ID, string.Empty, repo);
                EnsureAndSetColumn(context, instanceGUID, metabaseName, entity, Columns.COL_GUID_DATA_DISPLAY_NAME_FORMAT, string.Empty, repo);

                // Add AssemblyLineModes
                EnsureAndSetColumn(context, instanceGUID, metabaseName, entity, Columns.COL_GUID_ASSEMBLY_LINE_MODE_PREFIX + Actions.ACTION_NEW, AssemblyLineMode.Replace.ToString(), repo);
                EnsureAndSetColumn(context, instanceGUID, metabaseName, entity, Columns.COL_GUID_ASSEMBLY_LINE_MODE_PREFIX + Actions.ACTION_CREATE, AssemblyLineMode.Replace.ToString(), repo);
                EnsureAndSetColumn(context, instanceGUID, metabaseName, entity, Columns.COL_GUID_ASSEMBLY_LINE_MODE_PREFIX + Actions.ACTION_READ, AssemblyLineMode.Replace.ToString(), repo);
                EnsureAndSetColumn(context, instanceGUID, metabaseName, entity, Columns.COL_GUID_ASSEMBLY_LINE_MODE_PREFIX + Actions.ACTION_UPDATE, AssemblyLineMode.Replace.ToString(), repo);
                EnsureAndSetColumn(context, instanceGUID, metabaseName, entity, Columns.COL_GUID_ASSEMBLY_LINE_MODE_PREFIX + Actions.ACTION_DELETE, AssemblyLineMode.Replace.ToString(), repo);
                EnsureAndSetColumn(context, instanceGUID, metabaseName, entity, Columns.COL_GUID_ASSEMBLY_LINE_MODE_PREFIX + Actions.ACTION_SEARCH, AssemblyLineMode.Replace.ToString(), repo);

                // Restrictions
                EnsureAndSetColumn(context, instanceGUID, metabaseName, entity, Columns.COL_GUID_ALLOW_DRAG_DROP_REORDER, false.ToString(), repo);
                EnsureAndSetColumn(context, instanceGUID, metabaseName, entity, Columns.COL_GUID_ALLOW_NEW, true.ToString(), repo);
                EnsureAndSetColumn(context, instanceGUID, metabaseName, entity, Columns.COL_GUID_ALLOW_READ, true.ToString(), repo);
                EnsureAndSetColumn(context, instanceGUID, metabaseName, entity, Columns.COL_GUID_ALLOW_DELETE, true.ToString(), repo);

                if (entity.ColumnIdValuePair.ContainsKey("sysTableSchemaID"))
                {
                    entity.ColumnIdValuePair.Remove("sysTableSchemaID");
                }

                if (searchResponse.RowList.Count == 0)
                {
                    // Insert
                    repo.Create(context, instanceGUID, metabaseName, string.Empty, string.Empty, Tables.TABLE_GUID_SYS_TABLE_SCHEMA, entity);
                }
                else
                {
                    // Update
                    repo.Update(context, instanceGUID, metabaseName, entity);
                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }
        }

        public static void EnsureSysTableSchema(IContext context, string instanceGUID, string metabaseName, IStandardTable repo)
        {
            // Ensure the table exists.
            ITableManager tableManager = (ITableManager)repo;

            IQ.Entities.VastMetaDB.Table table = new IQ.Entities.VastMetaDB.Table();
            table.ID = Tables.TABLE_GUID_SYS_TABLE_SCHEMA;
            table.PrimaryKeyColumnID = table.ID + "ID";
            table.ColumnList = new List<Entities.VastMetaDB.Column>();
            tableManager.EnsureTable(context, instanceGUID, metabaseName, table);
            IQ.Entities.VastMetaDB.ColumnSchema columnSchema = new IQ.Entities.VastMetaDB.ColumnSchema();
            columnSchema.ID = Columns.COL_GUID_ID;
            tableManager.EnsureColumn(context, instanceGUID, metabaseName, table.ID, columnSchema);

            EnsureColumn(context, instanceGUID, metabaseName, table, Columns.COL_GUID_PRIMARY_KEY_COLUMN_ID, tableManager);
            EnsureColumn(context, instanceGUID, metabaseName, table, Columns.COL_GUID_NEW_DISPLAY_NAME, tableManager);
            EnsureColumn(context, instanceGUID, metabaseName, table, Columns.COL_GUID_UPDATE_DISPLAY_NAME, tableManager);
            EnsureColumn(context, instanceGUID, metabaseName, table, Columns.COL_GUID_GRID_DISPLAY_NAME, tableManager);
            EnsureColumn(context, instanceGUID, metabaseName, table, Columns.COL_GUID_COLUMN_GROUP_ID_LIST, tableManager);
            EnsureColumn(context, instanceGUID, metabaseName, table, Columns.COL_GUID_NEW_BUTTON_TEXT, tableManager);
            EnsureColumn(context, instanceGUID, metabaseName, table, Columns.COL_GUID_UPDATE_BUTTON_TEXT, tableManager);
            EnsureColumn(context, instanceGUID, metabaseName, table, Columns.COL_GUID_SHOW_FREE_FORM_SEARCH, tableManager);
            EnsureColumn(context, instanceGUID, metabaseName, table, Columns.COL_GUID_PARENT_TABLE_ID, tableManager);

            // Schema
            EnsureColumn(context, instanceGUID, metabaseName, table, Columns.COL_GUID_COLUMN_SYS_TABLE_SCHEMA_INSTANCE_GUID, tableManager);
            EnsureColumn(context, instanceGUID, metabaseName, table, Columns.COL_GUID_COLUMN_SYS_TABLE_SCHEMA_DATABASE_NAME, tableManager);
            EnsureColumn(context, instanceGUID, metabaseName, table, Columns.COL_GUID_COLUMN_SYS_TABLE_SCHEMA_TABLE_NAME, tableManager);

            // Visibility
            EnsureColumn(context, instanceGUID, metabaseName, table, Columns.COL_GUID_COLUMN_VISIBILITY_INSTANCE_GUID, tableManager);
            EnsureColumn(context, instanceGUID, metabaseName, table, Columns.COL_GUID_COLUMN_VISIBILITY_DATABASE_NAME, tableManager);
            EnsureColumn(context, instanceGUID, metabaseName, table, Columns.COL_GUID_COLUMN_VISIBILITY_TABLE_NAME, tableManager);

            EnsureColumn(context, instanceGUID, metabaseName, table, Columns.COL_GUID_TABLE_DATA_TYPE, tableManager);
            EnsureColumn(context, instanceGUID, metabaseName, table, Columns.COL_GUID_TABLE_DATA_TYPE_CODE_COLUMN_ID, tableManager);
            EnsureColumn(context, instanceGUID, metabaseName, table, Columns.COL_GUID_DATA_DISPLAY_NAME_FORMAT, tableManager);

            // Add AssemblyLineModes
            EnsureColumn(context, instanceGUID, metabaseName, table, Columns.COL_GUID_ASSEMBLY_LINE_MODE_PREFIX + Actions.ACTION_NEW, tableManager);
            EnsureColumn(context, instanceGUID, metabaseName, table, Columns.COL_GUID_ASSEMBLY_LINE_MODE_PREFIX + Actions.ACTION_CREATE, tableManager);
            EnsureColumn(context, instanceGUID, metabaseName, table, Columns.COL_GUID_ASSEMBLY_LINE_MODE_PREFIX + Actions.ACTION_READ, tableManager);
            EnsureColumn(context, instanceGUID, metabaseName, table, Columns.COL_GUID_ASSEMBLY_LINE_MODE_PREFIX + Actions.ACTION_UPDATE, tableManager);
            EnsureColumn(context, instanceGUID, metabaseName, table, Columns.COL_GUID_ASSEMBLY_LINE_MODE_PREFIX + Actions.ACTION_DELETE, tableManager);
            EnsureColumn(context, instanceGUID, metabaseName, table, Columns.COL_GUID_ASSEMBLY_LINE_MODE_PREFIX + Actions.ACTION_SEARCH, tableManager);

            // Restrictions
            EnsureColumn(context, instanceGUID, metabaseName, table, Columns.COL_GUID_ALLOW_DRAG_DROP_REORDER, tableManager);
            EnsureColumn(context, instanceGUID, metabaseName, table, Columns.COL_GUID_ALLOW_NEW, tableManager);
            EnsureColumn(context, instanceGUID, metabaseName, table, Columns.COL_GUID_ALLOW_READ, tableManager);
            EnsureColumn(context, instanceGUID, metabaseName, table, Columns.COL_GUID_ALLOW_DELETE, tableManager);

            // HTML
            EnsureColumn(context, instanceGUID, metabaseName, table, Columns.COL_HTML_CLASS, tableManager);
            EnsureColumn(context, instanceGUID, metabaseName, table, Columns.COL_HTML_STYLE, tableManager);
            EnsureColumn(context, instanceGUID, metabaseName, table, Columns.COL_SINGLE_BUTTON, tableManager);
        }

        private static void EnsureColumn(IContext context, string instanceGUID, string metabaseName, Entities.VastMetaDB.Table table, string columnId, ITableManager tableManager)
        {
            try
            {
                IQ.Entities.VastMetaDB.ColumnSchema columnSchema = new IQ.Entities.VastMetaDB.ColumnSchema();
                columnSchema.ID = columnId;

                tableManager.EnsureColumn(context, instanceGUID, metabaseName, table.ID, columnSchema);

            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }
        }

        public static void EnsureSysTableOperationSetting(IContext context, string instanceGUID, string metabaseName, IStandardTable repo)
        {
            try
            {
                // Ensure the table exists.
                ITableManager tableManager = (ITableManager)repo;

                IQ.Entities.VastMetaDB.Table table = new IQ.Entities.VastMetaDB.Table();
                table.ID = Tables.TABLE_GUID_SYS_TABLE_OPERATION_SETTING;
                table.PrimaryKeyColumnID = table.ID + "ID";

                table.ColumnList = new List<Entities.VastMetaDB.Column>();
                tableManager.EnsureTable(context, instanceGUID, metabaseName, table);

                // Ensure Columns
                EnsureColumn(context, instanceGUID, metabaseName, table, Columns.COL_GUID_SYS_TABLE_OPERATION_ID, tableManager);
                EnsureColumn(context, instanceGUID, metabaseName, table, Columns.COL_GUID_SETTING_KEY, tableManager);
                EnsureColumn(context, instanceGUID, metabaseName, table, Columns.COL_GUID_SETTING_VALUE, tableManager);
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }
        }

        public static void EnsureSysClientAction(IContext context, string instanceGUID, string metabaseName, IStandardTable repo)
        {
            try
            {
                // Ensure the table exists.
                ITableManager tableManager = (ITableManager)repo;

                IQ.Entities.VastMetaDB.Table table = new IQ.Entities.VastMetaDB.Table();
                table.ID = Tables.TABLE_GUID_SYS_CLIENT_ACTION;
                table.PrimaryKeyColumnID = table.ID + "ID";

                table.ColumnList = new List<Entities.VastMetaDB.Column>();
                tableManager.EnsureTable(context, instanceGUID, metabaseName, table);

                // Ensure Columns
                EnsureColumn(context, instanceGUID, metabaseName, table, table.PrimaryKeyColumnID, tableManager);
                EnsureColumn(context, instanceGUID, metabaseName, table, Columns.COL_CONFIG_SOURCE_KEY, tableManager);
                EnsureColumn(context, instanceGUID, metabaseName, table, Columns.COL_CONFIG_SOURCE_ID, tableManager);
                EnsureColumn(context, instanceGUID, metabaseName, table, Columns.COL_GUID_URL, tableManager);
                EnsureColumn(context, instanceGUID, metabaseName, table, Columns.COL_GUID_TARGET, tableManager);
                EnsureColumn(context, instanceGUID, metabaseName, table, Columns.COL_GUID_ACTION_TYPE, tableManager);
                EnsureColumn(context, instanceGUID, metabaseName, table, Columns.COL_GUID_CUSTOM_ASSEMBLY_LINE_NAME, tableManager);
                EnsureColumn(context, instanceGUID, metabaseName, table, Columns.COL_FILTER_FIELD_NAME_PREFIX, tableManager);
                EnsureColumn(context, instanceGUID, metabaseName, table, Columns.COL_GUID_NEW_DISPLAY_NAME, tableManager);
                EnsureColumn(context, instanceGUID, metabaseName, table, Columns.COL_GUID_TABLE_ID, tableManager);
                //EnsureColumn(context, instanceGUID, metabaseName, table, Columns.COL_GUID_VIEW_NAME, tableManager);
                //EnsureColumn(context, instanceGUID, metabaseName, table, Columns.COL_GUID_RESPONSE_FORMAT, tableManager);
                //EnsureColumn(context, instanceGUID, metabaseName, table, Columns.COL_GUID_INPUT_TYPE, tableManager);
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }
        }

        public static void EnsureSysTableAuth(IContext context, string instanceGUID, string metabaseName, IStandardTable repo)
        {
            try
            {
                // Ensure the table exists.
                ITableManager tableManager = (ITableManager)repo;

                IQ.Entities.VastMetaDB.Table table = new IQ.Entities.VastMetaDB.Table();
                table.ID = Tables.TABLE_GUID_SYS_TABLE_AUTH;
                table.PrimaryKeyColumnID = table.ID + "ID";

                table.ColumnList = new List<Entities.VastMetaDB.Column>();
                tableManager.EnsureTable(context, instanceGUID, metabaseName, table);

                // Ensure Columns
                EnsureColumn(context, instanceGUID, metabaseName, table, table.PrimaryKeyColumnID, tableManager);
                EnsureColumn(context, instanceGUID, metabaseName, table, Columns.COL_GUID_ID, tableManager);
                EnsureColumn(context, instanceGUID, metabaseName, table, Actions.ACTION_NEW, tableManager);
                EnsureColumn(context, instanceGUID, metabaseName, table, Actions.ACTION_CREATE, tableManager);
                EnsureColumn(context, instanceGUID, metabaseName, table, Actions.ACTION_READ, tableManager);
                EnsureColumn(context, instanceGUID, metabaseName, table, Actions.ACTION_UPDATE, tableManager);
                EnsureColumn(context, instanceGUID, metabaseName, table, Actions.ACTION_DELETE, tableManager);
                EnsureColumn(context, instanceGUID, metabaseName, table, Actions.ACTION_SEARCH, tableManager);
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }
        }

        /// <summary>
        /// Created:        08/12/2015
        /// Author:         David J. McKee
        /// Purpose:        Returns the value of a given column.
        /// Important:      The default value is used when the column does not exist or when the value is empty.
        /// </summary>        
        public static string SafeReadString(Dictionary<string, string> columnGUIDValuePair, string key, string defaultValue)
        {
            string result = defaultValue;

            if (columnGUIDValuePair != null)
            {
                if (columnGUIDValuePair.ContainsKey(key))
                {
                    string colValue = columnGUIDValuePair[key].ToString();

                    if (string.IsNullOrWhiteSpace(colValue) == false)
                    {
                        result = colValue;
                    }
                }
            }

            return result;
        }

        public static string SafeReadString(Dictionary<string, string> columnGUIDValuePair, string key)
        {
            return SafeReadString(columnGUIDValuePair, key, string.Empty);
        }
    }
}
