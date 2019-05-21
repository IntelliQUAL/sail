using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace VastMetaDB.BaseClasses
{
    public class CommonBase
    {
        public string ID = string.Empty;
        public string Name = string.Empty;
        public string DisplayName = string.Empty;
        public string Description = string.Empty;
        public DateTime DateCreated = DateTime.UtcNow;
        public string ColumnGroupIdList = string.Empty; // List of column groups for this table.
    }
}
