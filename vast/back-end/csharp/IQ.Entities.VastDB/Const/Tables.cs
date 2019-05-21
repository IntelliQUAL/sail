using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace IQ.Entities.VastDB.Const
{
    public class Tables
    {
        // Master Database
        public const string TABLE_GUID_SYS_LOGIN = "sysLogin";                      // Contains a list of logins for a given account
        public const string TABLE_GUID_SYS_LOGIN_ACCOUNT = "sysLoginAccount";       // Provides a domain level map between email / username and account. This is so users don't need to enter an account.
        public const string TABLE_GUID_SYS_DATABASE = "sysDatabase";                // Contains a list of databases for a given account.
        public const string TABLE_GUID_SYS_LOGIN_DATABASE = "sysLoginDatabase";     // Contains a list of databases for which a given login has access.

        // MasterRepository
        public const string IQ_CLOUD_TABLE_ID_ACCOUNT = "Account";                                                  // List of all valid Accounts within a given domain
        public const string TABLE_GUID_ACCOUNT_TYPE = "AccountType";                                                // List of valid account types for the domain.
        public const string TABLE_GUID_BILLING_ACCOUNT = "BillingAccount";                                          // Contains zero or more ways for a customer to pay 
                                                                                                                    //  (in Priority order). Usually contained within the 
                                                                                                                    //  Master Repository for a given account.

        // Database Specific
        public const string TABLE_GUID_SYS_TABLE_SCHEMA = "sysTableSchema";
        public const string TABLE_GUID_SYS_TABLE_AUTH = "sysTableAuth";
        public const string TABLE_GUID_SYS_OPERATION = "sysOperation";
        public const string TABLE_GUID_SYS_TABLE_OPERATION = "sysTableOperation";
        public const string TABLE_GUID_SYS_TABLE_OPERATION_SETTING = "sysTableOperationSetting";                    // Must be same format as sysCustomALOperationSetting
        public const string TABLE_GUID_SYS_OPERATION_SETTING_VALUE = "sysOperationSettingValue";                    // Future: Used to display setting options to the end-user
        public const string TABLE_GUID_SYS_MENU = "sysMenu";
        public const string TABLE_GUID_SYS_BRAND = "sysBrand";
        public const string TABLE_GUID_SYS_CUSTOM_ASSEMBLY_LINE = "sysCustomAssemblyLine";
        public const string TABLE_GUID_SYS_CUSTOM_ASSEBLY_LINE_OPERATION = "sysCustomALOperation";
        public const string TABLE_GUID_SYS_CUSTOM_ASSEBLY_LINE_OPERATION_SETTING = "sysCustomALOperationSetting";   // Must be same format as sysTableOperationSetting
        public const string TABLE_GUID_SYS_BACKGROUND_PROCESS = "sysBackgroundProcess";                             // List of background processes for a given module.          
        public const string TABLE_GUID_SYS_LOG = "sysLog";
        public const string TABLE_GUID_SYS_SETTING = "sysSetting";                                                  // Domain wide system settings
        public const string TABLE_GUID_FEATURE = "sysFeature";                                                      // List of features within the application. May be shown to users when new features are added.
        public const string TABLE_GUID_SYS_CLIENT_ACTION = "sysClientAction";                                       // Provides additional actions to the end user.
        public const string TABLE_GUID_SYS_STANDARD_OPERATION = "sysStandardOperation";                             // Lists all standard operations.
    }
}
