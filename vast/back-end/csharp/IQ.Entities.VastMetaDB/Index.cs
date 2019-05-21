using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQ.Entities.VastMetaDB
{
    public class Index
    {
        public string TableGUID = string.Empty;
        public string ColumnGUID = string.Empty;
        public bool Unique = false; // Force unique constraint

    }
}
