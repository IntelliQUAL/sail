using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace IQ.Entities.VastMetaDB.Enums
{
    public enum AssemblyLineMode
    {
        Replace = 0,        // Default: Replace all operations with thoses listed in sysTableOperation
        Append = 1,         // Add all standard operations and then append table assenbly line operations.
        Prepend = 2,        // Add table assenbly line operations before standard processing.
    }
}
