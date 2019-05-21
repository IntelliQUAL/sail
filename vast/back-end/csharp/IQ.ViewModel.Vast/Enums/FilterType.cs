using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace IQ.ViewModel.Vast.Enums
{
    public enum FilterType
    {
        NoFilter = 0,           // No option to filter this field
        Lookup = 1,             // Lookup from another table (based on *ColumnSchema table)
        FreeFormSearch = 2,     // Plain text search
        DistinctList = 3,       // Distinct list of all values within the given table.
        Enum = 4,               // Simple List of Values (based on Enum column within *ColumnSchema table)
        MultiSelectList = 5,
        SingleSelectList = 6,
        DateTimeRange = 7,
        DateRange = 8
    }
}
