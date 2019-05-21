using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQ.Entities.VastMetaDB.Enums
{
    /// <summary>
    /// This is a common data type which can be used accross databases.
    /// </summary>
    public enum ColumnDataType
    {
        Unknown = 0,
        String = 1,         // TEXT
        ForeignKey = 2,     // Could be an integer, text, or even a date/time but it contains a primary key from another table 
        PrimaryKey = 3,
        Date = 4,
        DateTime = 5,
        Binary = 6,         // Binary / BLOB data.  TableDataType must be Blob to use this datatype.  Only one Binary field may exist per row.
        Number = 7,         // INTEGER, REAL, NUMERIC
        Boolean = 8,        // True / False
        Hash = 9,           // The actual value is not stored. Instead the value is hashed.
        Encrypted = 10      // The value is stored encrypted.
    }
}
