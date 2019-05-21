using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQ.Entities.VastDB.Const
{
    public class Actions
    {
        public const string ACTION_NEW = "New";
        public const string ACTION_CREATE = "Create";
        public const string ACTION_READ = "Read";
        public const string ACTION_UPDATE = "Update";
        public const string ACTION_DELETE = "Delete";
        public const string ACTION_SEARCH = "Search";
        public const string ACTION_LOAD_MODULE = "LoadModule";  // Loads the module returned. Used for menu updates or new account creation.
        public const string ACTION_LOG_OUT = "Logout";          // Used when the entire account needs to be reset such as when a users deletes their account.
        public const string ACTION_SPECIFIC = "Specific";       // Instructs the client to perform a specific action using all possible variables.
    }
}
