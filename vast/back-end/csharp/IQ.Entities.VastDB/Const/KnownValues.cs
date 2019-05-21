using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace IQ.Entities.VastDB.Const
{
    public class KnownValues
    {
        public const string KNOWN_VALUE_ME = "{{Me}}";                                          // Replaced with "sysLoginID"
        public const string KNOWN_VALUE_INSTANCE_STATUS = "InstanceStatus";                     // QA = "QA", UAT = "UAT, Prod = "". 
        public const string KNOWN_VALUE_INSTANCE_STATUS_W_SLASH = "InstanceStatusWithSlash";    // QA = "QA/", UAT = "UAT/, Prod = "". 
        public const string KNOWN_VALUE_SYSTEM_USER = "SYSTEM";                                 // System User ID used for system only operations such as creating Accounts.  This account is only authorized to do very specific actions.
        public const string KNOWN_VALUE_PUBLIC_USER = "PUBLIC";                                 // Public User ID used for public access only as online forms not requiring authentication.  This account is only authorized to do very specific actions.

        public const string KNOWN_VALUE_SYSTEM_ROLE_SUPER_ADMIN = "sysSuperAdmin";              // Should only be IQ employees. Can do everything including edit users for all domains.
        public const string KNOWN_VALUE_SYSTEM_ROLE_DOMAIN_ADMIN = "sysDomainAdmin";            // Should only be an employee of the given domain. Can do everything including edit users
        public const string KNOWN_VALUE_SYSTEM_ROLE_ACCOUNT_ADMIN = "sysAccountAdmin";          // Should only be an employee of the given account. Can do everything including edit users
        public const string KNOWN_VALUE_SYSTEM_ROLE_SYSTEM_DESIGNER = "sysSystemDesigner";      // Can create / edit forms, fields, and menus. An end-user may be a System Designer for a CRM.

        public const string MENU_STYLE_HORIZONTAL = "HNav";
        public const string BLANK_VALUE = "ASCII0x20";                                          // Used to send a blank where a blank is normally not allowed. 
        public const string UTC_TIME_FORMAT = "u";
    }
}
