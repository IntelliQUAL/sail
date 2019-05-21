using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace IQ.Entities.VastDB.Const
{
    public class Context
    {
        public const string EXIT_ASSEMBLY_LINE = "ExitAssemblyLine";

        // Repositories
        public const string INTERFACE_STANDARD_TABLE = "IStandardTable";
        public const string INTERFACE_BLOB = "IBlobTable";
        public const string INTERFACE_STANDARD_TABLE_OR_BLOB_METADATA = "IStandardTable or IBlobTable (Metadata)";

        // Input from API
        public const string ACCOUNT_CODE = "AccountCode";
        public const string DOMAIN_NAME = "DomainName";
        public const string INSTANCE_GUID = "InstanceGUID";                             // Globally unique identifier for an instance of a cloud environment.    
        public const string INSTANCE_STATUS = "InstanceStatus";                         // Blank / Default = Prod                                       
        public const string DATABASE_ID = "DatabaseID";                                 // Unique identifier for a Database within an instance                    
        public const string TABLE_ID = "TableID";                                       // Unique Table ID within a database.       
        public const string PRIMARY_KEY_COLUMN_ID = "PrimaryKeyColumnID";               // ColumnID which is the primary key.            
        public const string PARENT_TABLE_ID = "ParentTableGUID";                        // Unique Parent Table ID within a database.                    
        public const string PARENT_TABLE_PRIMARY_KEY = "ParentTablePrimaryKey";         // Primary Key of Parent Table                                          
        public const string SYS_LOGIN_ID = "sysLoginID";
        public const string PASSWORD_HASH = "PasswordHash";
        public const string LOG_SETTING = "LogSetting";                                 // SEE: COL_GUID_LOG_SETTING for details

        public const string CUSTOM_ASSEMBLY_LINE_NAME = "CustomAsseblyLineName";        // Custom Assembly Line
        public const string OPERATION_NAME_LIST = "OperationNameList";                  // comma delimited list of operation names.

        public const string CONFIG_SOURCE = "OperationConfigSourceTableID";
        public const string CONFIG_SOURCE_KEY = "OperationConfigSourceTableColumnID";
        public const string CONFIG_SOURCE_ID = "ConfigSourceID";                        // This should ONLY be used for context serialization.
        public const string CONFIG_SOURCE_INSTANCE_GUID = "ConfigSourceInstanceGUID";
        public const string CONFIG_SOURCE_DATABASE_NAME = "ConfigSourceDatabaseName";

        public const string CONTEXT_KEY_ACTION_TYPE = "ActionType";
        public const string FORMAT_SETTING_PREFIX = "FormatSettingPrefix";              // Used to prefix setting names for if / then operations.
        public const string CONTEXT_AUTH_TOKEN = "AuthToken";
        public const string CONTEXT_VALUE_APP_CODE = "VAST";
    }
}
