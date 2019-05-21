using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace IQ.Entities.VastMetaDB.Enums
{
    public enum OperatorType
    {
        EqualTo = 1,
        NotEqualTo = 2,
        GreaterThan = 3,
        GreaterThanOrEqualTo = 4,
        LessThan = 5,
        LessThanOrEqualTo = 6,
        In = 7,
        Between = 8,
        Like = 9,
        IsNull = 10,
        IsNotNull = 11
    }
}
