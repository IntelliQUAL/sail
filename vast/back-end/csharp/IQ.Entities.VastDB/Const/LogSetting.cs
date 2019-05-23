using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQ.Entities.VastDB.Const
{
    public class LogSetting
    {
        public const string OFF = "Off";                   // nothing
        public const string ERROR = "Error";               // errors only
        public const string WARNING = "Warning";           // errors and warnings
        public const string INFO = "Information";          // errors, warnings, informational messages
        public const string VERBOSE = "Verbose";           // “Information” plus additional debugging trace information including API requests and responses in XML format
        public const string ACTIVITY = "ActivityTracing";  // start and stop events only
        public const string ALL = "All";                   // “Verbose” plus “ActivityTracing”
    }
}
