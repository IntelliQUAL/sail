using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace IQ.Entities.VastMetaDB
{
    public enum TableDataType
    {
        Standard = 1,
        Blob = 2,       // Supports Streams
        Type = 3        // e.g. Enum data. Data in this table is expectd to exist for the application to function.
    }
}
