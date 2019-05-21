using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQ.Entities.VastDB.Const
{
    public class Columns
    {
        // Billing Account
        public const string COL_GUID_BILLING_ACCOUNT_PRIMARY_KEY = "BillingAccountID";
        public const string COL_GUID_BILLING_ADDRESS_LINE_1 = "BillingAddressLine1";
        public const string COL_GUID_BILLING_ADDRESS_LINE_2 = "BillingAddressLine2";
        public const string COL_GUID_BILLING_CITY = "BillingCity";
        public const string COL_GUID_BILLING_STATE = "BillingState";
        public const string COL_GUID_BILLING_POSTAL_CODE = "BillingPostalCode";
        public const string COL_GUID_BILLING_CC_NUMBER = "CreditCardNumber";
        public const string COL_GUID_BILLING_CC_TYPE = "CreditCardType";
        public const string COL_GUID_BILLING_NAME_ON_CARD = "NameOnCard";
        public const string COL_GUID_BILLING_EXP_MONTH = "ExpMonth";
        public const string COL_GUID_BILLING_EXP_YEAR = "ExpYear";
        public const string COL_GUID_BILLING_CARD_SECURITY_CODE = "CardSecurityCode";

        public const string COL_GUID_ID = "ID";
        public const string COL_GUID_NAME = "Name";

        public const string COL_GUID_USER_NAME = "Username";
        public const string COL_GUID_EMAIL = "Email";
        public const string COL_GUID_PASSWORD = "Password";
        public const string COL_GUID_PASSWORD_HASH = "PwdHash";
        public const string COL_GUID_DATABASE_GUID = "GUID";

        public const string COL_GUID_DEFAULT_DATABASE_GUID = "DefaultDatabaseGuid"; // This is contained within the sysLogin table in and is a GUID in case the database name changes.
        public const string COL_GUID_INTERNAL_PRIMARY_KEY = "InternalPK";
        public const string COL_GUID_DEFAULT_TABLE_ID = "DefaultTableID";
        public const string COL_GUID_ACTION_TYPE = "ActionType";
        public const string COL_GUID_DEFAULT_ACTION_TYPE = "DefaultActionType";
        public const string COL_GUID_MENU_STYLE = "MenuStyle";
        public const string COL_GUID_CLASS_TEXT = "ClassText";

        public const string COL_GUID_NEW_DISPLAY_NAME = "NewDisplayName";
        public const string COL_GUID_NEW_BUTTON_TEXT = "NewButtonText";

        public const string COL_GUID_CREATE_DISPLAY_NAME = "CreateDisplayName";
        public const string COL_GUID_CREATE_BUTTON_TEXT = "CreateButtonText";

        public const string COL_GUID_UPDATE_DISPLAY_NAME = "UpdateDisplayName";
        public const string COL_GUID_UPDATE_BUTTON_TEXT = "UpdateButtonText";

        public const string COL_GUID_GRID_DISPLAY_NAME = "GridDisplayName";
        public const string COL_GUID_ICON_NAME = "IconName";

        public const string COL_GUID_PARENT_TABLE_ID = "ParentTableID";
        public const string COL_GUID_ACTION = "Action";
        public const string COL_GUID_ACTION_LIST = "ActionList";                                            // A list of actions
        public const string COL_GUID_OPERATION_ID_LIST = "OperationLineIdList";
        public const string COL_GUID_COLUMN_GROUP_ID_LIST = "ColumnGroupIdList";
        public const string COL_GUID_PRIMARY_KEY_COLUMN_ID = "PrimaryKeyColumnID";
        public const string PRIMARY_KEY_PLACEHOLDER = "{{PrimaryKeyColumnID}}";
        public const string COL_GUID_OPERATION_ID = "sysOperationID";
        public const string COL_GUID_FULL_NAME = "FullName";
        public const string COL_GUID_KEY = "Key";
        public const string COL_GUID_VALUE = "Value";
        public const string COL_GUID_SETTING_KEY = "SettingKey";
        public const string COL_GUID_SETTING_VALUE = "SettingValue";
        public const string COL_GUID_TABLE_ID = "TableID";
        public const string COL_GUID_SEQUENCE = "Sequence";
        public const string COL_GUID_SYS_CUSTOM_ASSEMBLY_LINE_ID = "sysCustomAssemblyLineID";
        public const string COL_GUID_SYS_CUSTOM_ASSEMBLY_LINE_OPERATION_ID = "sysCustomALOperationID";
        public const string COL_GUID_SYS_TABLE_OPERATION_ID = "sysTableOperationID";
        public const string COL_GUID_DEFAULT_SORT_COLUMN = "DefaultSortColumn";
        public const string COL_GUID_DEFAULT_SORT_ORDER = "DefaultSortOrder";
        public const string COL_GUID_ALLOW_DRAG_DROP_REORDER = "AllowDragDropReorder";
        public const string COL_GUID_ALLOW_NEW = "AllowNew";
        public const string COL_GUID_ALLOW_READ = "AllowRead";
        public const string COL_GUID_ALLOW_DELETE = "AllowDelete";
        public const string COL_GUID_DATA_DISPLAY_NAME_FORMAT = "DataDisplayNameFormat";
        public const string COL_GUID_SHOW_FREE_FORM_SEARCH = "ShowFreeFormSearch";
        public const string COL_GUID_ASYNC = "Async";
        public const string COL_GUID_URL = "URL";
        public const string COL_GUID_TARGET = "target";

        // Common *ColumnVisibility
        public const string COL_GUID_COLUMN_VISIBILITY_INSTANCE_GUID = "ColumnVisibilityInstanceGUID";
        public const string COL_GUID_COLUMN_VISIBILITY_DATABASE_NAME = "ColumnVisibilityDatabaseName";
        public const string COL_GUID_COLUMN_VISIBILITY_TABLE_NAME = "ColumnVisibilityTableName";

        // Common *ColumnSchema
        public const string COL_GUID_COLUMN_SYS_TABLE_SCHEMA_INSTANCE_GUID = "sysTableSchemaInstanceGUID";
        public const string COL_GUID_COLUMN_SYS_TABLE_SCHEMA_DATABASE_NAME = "sysTableSchemaDatabaseName";
        public const string COL_GUID_COLUMN_SYS_TABLE_SCHEMA_TABLE_NAME = "sysTableSchemaTableName";

        // Common sysTableOperation
        public const string COL_GUID_COLUMN_SYS_TABLE_OPERATION_INSTANCE_GUID = "sysTableOperationInstanceGUID";
        public const string COL_GUID_COLUMN_SYS_TABLE_OPERATION_DATABASE_NAME = "sysTableOperationDatabaseName";
        public const string COL_GUID_COLUMN_SYS_TABLE_OPERATION_TABLE_NAME = "sysTableOperationTableName";
        public const string COL_GUID_COLUMN_SYS_TABLE_OPERATION_URL = "sysTableOperationUrl";                       // Pulls the operation list and operation settings from a url.

        public const string COL_GUID_TABLE_DATA_TYPE = "TableDataType";                                             // sysTableSchema: Indicates the TableDataType of the given table.
        public const string COL_GUID_TABLE_DATA_TYPE_CODE_COLUMN_ID = "TableDataTypeCodeColumnID";                  // Column which contains the ColumnID which is the unique value for the given type.
        public const string COL_GUID_ASSEMBLY_LINE_MODE_PREFIX = "AssemblyLineMode";                                // SEE: Enum AssemblyLineMode used as {Action}AssemblyLineMode
        public const string COL_GUID_SYS_LOGIN_ID = "sysLoginID";
        public const string COL_GUID_SYS_LOGIN_DATABASE_ID = "sysLoginDatabaseID";
        public const string COL_GUID_SYS_DATABASE_ID = "sysDatabaseID";
        public const string COL_GUID_PARENT_SYS_DATABASE_ID = "ParentSysDatabaseID";
        public const string COL_GUID_LOG_SETTING = "LogSetting";

        // Setting  What is logged:
        // Off              : nothing
        // Error            : errors only
        // Warning          : errors and warnings
        // Information      : errors, warnings, informational messages
        // Verbose          : “Information” plus additional debugging trace information including API requests and responses in XML format
        // ActivityTracing  : start and stop events only
        // All              : “Verbose” plus “ActivityTracing”

        public const string COL_GUID_LAST_ACCESSED = "LastAccessed";
        public const string COL_GUID_CREATED_BY = "CreatedBy";
        public const string COL_GUID_CREATED_DATE = "CreatedDate";
        public const string COL_GUID_MODIFIED_BY = "ModifiedBy";
        public const string COL_GUID_MODIFIED_DATE = "ModifiedDate";

        public const string COL_GUID_DOMAIN_NAME = "DomainName";        // Used in Master Repository
        public const string COL_GUID_ACCOUNT_ID = "AccountID";          // Used in Master Repository
        public const string COL_GUID_ACCOUNT_CODE = "AccountCode";      // Used in Master Repository
        public const string COL_GUID_ACCOUNT_TYPE = "AccountType";      // Used on initial account creation.
        public const string COL_GUID_FIRST_NAME = "FirstName";
        public const string COL_GUID_LAST_NAME = "LastName";
        public const string COL_GUID_PHONE = "Phone";
        public const string COL_GUID_TERMS = "TsAndCs";
        public const string COL_GUID_MODULE_NAME = "ModuleName";        // Used in Master database of each account
        public const string COL_GUID_MODULE_ID = "ModuleID";            // Used in sysMenu
        public const string COL_GUID_FORM_ID = "FormID";                // Used in Master database or each Module
        public const string COL_GUID_FIELD_ID = "FieldID";
        public const string COL_GUID_PRIMARY_KEY = "PrimaryKey";
        public const string COL_GUID_TYPE = "Type";
        public const string COL_GUID_COMPANY_NAME = "CompanyName";

        public const string COL_GUID_DESC = "Description";
        public const string COL_GUID_NOTE = "Note";                             // Used when a description is already present and an additional field is required.
        public const string COL_GUID_SYS_MENU_ID = "sysMenuID";
        public const string COL_GUID_PARENT_SYS_MENU_ID = "ParentSysMenuID";
        public const string COL_GUID_DISPLAY_SEQUENCE = "DisplaySequence";
        public const string COL_GUID_DISPLAY_NAME = "DisplayName";
        public const string COL_GUID_USER_ROLE_LIST = "sysUserRoleList";
        public const string COL_GUID_TRANSACTION_ID = "TransactionID";

        public const string COL_GUID_TABLE_ASSEMBLY_LINE = "TableAssemblyLine";
        public const string COL_GUID_CUSTOM_ASSEMBLY_LINE = "CustomAssemblyLine";
        public const string COL_GUID_CUSTOM_ASSEMBLY_LINE_NAME = "CustomAssemblyLineName";
        public const string COL_GUID_MESSAGE = "Message";
        public const string COL_GUID_INSTANCE_GUID = "InstanceGuid";
        public const string COL_GUID_START_TIME = "StartTime";
        public const string COL_GUID_END_TIME = "EndTime";
        public const string COL_GUID_NEXT_RUN_TIME = "NextRunTime";
        public const string COL_GUID_LAST_START_RUN_TIME = "LastStartRunTime";
        public const string COL_GUID_LAST_STOP_RUN_TIME = "LastStopRunTime";
        public const string COL_GUID_LAST_RUN_RESULT = "LastRunResult";
        public const string COL_GUID_STATUS = "Status";
        public const string COL_GUID_TOTAL_MILLISECONDS = "TotalMilliseonds";
        public const string COL_GUID_CONTEXT_CONTENTS = "ContextContents";
        public const string COL_REQUEST = "Request";
        public const string COL_RESPONSE = "Response";
        public const string COL_CONFIG_SOURCE_ID = "ConfigSourceID";
        public const string COL_CONFIG_SOURCE_KEY = "ConfigSourceKey";

        public const string COL_GUID_INTERNAL = "Internal";

        public const string COL_SCHEMA_DEFAULT_VALUE = "DefaultValue";
        public const string COL_SCHMEA_HAS_DEFAULT_VALUE = "HasDefaultValue";
        public const string COL_SCHMEA_HELP_TITLE = "HelpTitle";
        public const string COL_SCHMEA_HELP_TEXT = "HelpText";
        public const string COL_SCHEMA_SORT_DESC = "SortDescending";                                    // Default sort order when clicking on a grid column heading.
        public const string COL_SCHEMA_REQUIRED = "Required";
        public const string COL_SCHEMA_FOREIGN_MODULE_ID = "ForeignModuleID";                           // optional - Any module within the given account.
        public const string COL_SCHEMA_FOREIGN_TABLE_ID = "ForeignTableID";
        public const string COL_SCHEMA_FOREIGN_TABLE_SEARCH_CRITERIA = "ForeignTableSearchCriteria";    // Search critiera used when searching the foreign table.
        public const string COL_SCHEMA_PATTERN = "Pattern";
        public const string COL_SCHEMA_GROUP_ID = "GroupID";
        public const string COL_SCHEMA_FILTER_ENABLED = "FilterEnabled";
        public const string COL_SCHEMA_FILTER_DEFAULT_VALUE = "FilterDefaultValue";
        public const string COL_SCHEMA_ENUM = "Enum";
        public const string COL_PLACEHOLDER = "Placeholder";
        public const string COL_COLUMN_DATA_TYPE = "ColumnDataType";
        public const string COL_INPUT_TYPE = "InputType";
        public const string COL_FORMAT = "Format";                              // Same as "DataFormat". "DataFormat" is preferred over "Format".
        public const string COL_DATA_FORMAT = "DataFormat";                     // Used in conjunction with InputType to format the control. "DataFormat" is preferred over "Format".
        public const string COL_VALIDATE_MIN_MAX = "ValidateMinMax";
        public const string COL_MAX = "Max";
        public const string COL_MIN = "Min";
        public const string COL_UNIQUE = "Unique";
        public const string COL_UNIQUE_ERROR_MESSAGE = "UniqueErrorMessage";
        public const string COL_FILTER = "Filter";                              // Used for background process events.
        public const string COL_RESPONSE_FORMAT = "ResponseFormat";

        public const string COL_HTML_STYLE = "HtmlStyle";                       // Relative to the element e.g. Form or field.
        public const string COL_HTML_CLASS = "HtmlClass";                       // Relative to the element e.g. Form or field.
        public const string COL_SINGLE_BUTTON = "SingleButton";                 // True = only shown 1 button at bottom, False (default) show button at top and bottom.

        // Grid System
        public const string COL_XS_GRID_COLS = "XsGridCols";
        public const string COL_SM_GRID_COLS = "SmGridCols";
        public const string COL_MD_GRID_COLS = "MdGridCols";
        public const string COL_LG_GRID_COLS = "LgGridCols";

        // Filter Values
        public const string COL_FILTER_FIELD_NAME_PREFIX = "FilterFieldName";
        public const string COL_FILTER_FIELD_VALUE_PREFIX = "FilterFieldValue";

        public const string COL_FILTER_FIELD_NAME_1 = COL_FILTER_FIELD_NAME_PREFIX + "1";
        public const string COL_FILTER_FIELD_VALUE_1 = COL_FILTER_FIELD_VALUE_PREFIX + "1";

        public const string COL_FILTER_FIELD_NAME_2 = COL_FILTER_FIELD_NAME_PREFIX + "2";       // New on 5/27/2016
        public const string COL_FILTER_FIELD_VALUE_2 = COL_FILTER_FIELD_VALUE_PREFIX + "2";     // New on 5/27/2016

        public const string COL_QUERY_STRING = "QueryString";                                   // New on 5/27/2016
    }
}
