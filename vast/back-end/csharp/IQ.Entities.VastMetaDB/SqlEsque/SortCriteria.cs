using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IQ.Entities.VastMetaDB.Enums;

namespace IQ.Entities.VastMetaDB.SqlEsque
{
    [Serializable]
    public class SortCriteria
    {
        public string SortColumn = string.Empty;
        public SortOrder SortOrder = SortOrder.Natural;
    }
}
