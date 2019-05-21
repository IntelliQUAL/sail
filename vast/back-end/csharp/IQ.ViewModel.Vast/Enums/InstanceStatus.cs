using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQ.ViewModel.Vast.Enums
{
    public enum InstanceStatus
    {
        Prod = 0,
        Dev = 1,        // Running on a development server
        QA = 2,         // QA environment
        UAT = 3,        // UAT environment
        HotFix = 4,     // Hot fix environment
        Stage = 5,      // Staging enviornment prior to prod push.
        Local = 6       // Everything running on the same machine. Usually a development box.
    }
}
