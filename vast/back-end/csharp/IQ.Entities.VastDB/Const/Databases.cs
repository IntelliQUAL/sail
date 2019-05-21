using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQ.Entities.VastDB.Const
{
    public class Databases
    {
        // The master catelog (database) records all the system-level information for an Instance. 
        // This includes instance-wide metadata such as logon accounts, endpoints, 
        // linked servers, and system configuration settings. 
        // Also, master is the Catalog that records the existence of all other Catalogs and the location of those Catalog files and 
        // records the initialization information for the system. Therefore, the system cannot start if the master database is unavailable.
        public const string CAT_GUID_MASTER = "master";                     // contains the list of logins and databases within each account.
        public const string IQ_CLOUD_DATABASE_GUID = "MasterRepository";    // Master Module. Contains the list of accounts  
        public const string DOMAIN_CONTROL_PANEL = "DomainControlPanel";    // Database used to administer a given domain.
    }
}
