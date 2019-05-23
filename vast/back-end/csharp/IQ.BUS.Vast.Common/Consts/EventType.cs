using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace IQ.BUS.Vast.Common.Consts
{
    public enum EventType
    {
        Critical,       //      a fatal error or application crash
        Error,          //      a recoverable error
        Warning,        //      a noncritical problem
        Information,    //      an informational message
        Verbose,        //      a debugging trace message
        Start,          //      starting of a logical operation
        Stop            //      stopping of a logical operation
    }
}
