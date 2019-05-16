using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAIL.Framework.Host.Enums
{
    public enum TestResult
    {
        Success = 0,
        Failure = 2,
        RequestResponseFormatNotSupported = 3,
        FailureNoUnitTestImplemented = 4,
        InternalUnitTestException = 5,
        UnitTestDisabled = 6,           // Enabled = "N"
        FailureButTreatAsWarning = 7    // TreatFailureAsWarning = "Y"
    }
}
