using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using IQ.ViewModel.Vast.Enums;

namespace IQ.ViewModel.Vast
{
    public class AuthRequest
    {
        public string DomainName = string.Empty;                        // Example: logisticsbackoffice.com
        public string AccountCode = string.Empty;                       // Example: WOL
        public string AccountStatus = InstanceStatus.Prod.ToString();
        public string Username = string.Empty;                          // Login username for the given Instance
        public string Password = string.Empty;                          // Password for the given Instance User        
    }
}
