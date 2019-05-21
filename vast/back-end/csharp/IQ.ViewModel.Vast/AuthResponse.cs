using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using SAIL.Framework.Host.BaseClasses;

using IQ.ViewModel.Vast.Enums;

namespace IQ.ViewModel.Vast
{
    public class AuthResponse : ViewModelResultBase
    {
        public string DomainName = string.Empty;                        // Example: logisticsbackoffice.com
        public string AccountID = string.Empty;                         // Example: WOL
        public string AccountStatus = InstanceStatus.Prod.ToString();
        public string AuthToken = string.Empty;
        public string HostUrl = string.Empty;                           // Example: https://uat-api.inteliqual.com
        public string DefaultModuleID = string.Empty;
        public IQ.ViewModel.Vast.Grid ModuleList;
        public string DefaultFormID = string.Empty;
        public string DefaultActionType = string.Empty;
        public string FirstName = string.Empty;
        public string LastName = string.Empty;
        public string GroupNameList = string.Empty;
        public string AccountType = string.Empty;
    }
}
