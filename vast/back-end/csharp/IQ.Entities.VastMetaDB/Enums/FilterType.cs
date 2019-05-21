using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQ.Entities.VastMetaDB.Enums
{
    public enum FilterType
    {
        NoFilter = 0,           // No option to filter this field
        Lookup = 1,             // Lookup from another table (based on *ColumnSchema table)
        FreeFormSearch = 2,     // Plain text search
        MultiSelectList = 3,
        SingleSelectList = 4,
        DateTimeRange = 5,
        DateRange = 6,
        TimeRange = 7
    }
}
