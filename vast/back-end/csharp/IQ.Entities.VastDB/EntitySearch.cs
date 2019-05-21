using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace IQ.Entities.VastDB
{
    [Serializable]
    public class EntitySearch
    {
        public int Page = 0;
        public int RowsPerPage = 0;
        public IQ.Entities.VastMetaDB.SqlEsque.Where Where = null;
        public IQ.Entities.VastMetaDB.SqlEsque.OrderBy OrderBy = null;
        public string TableID = string.Empty;                                       // Same as TableName
        public bool Distinct = false;                                               // Return distinct rows.
        public List<string> ReturnColumnList = new List<string>();                  // List of columns to return if 0 or null all columns are returned.
        public string sysLoginID = string.Empty;                                    // Primary Key from the sysLogin table within the Master database

        public string TableName
        {
            get
            {
                // TableName is the same as TableID but TableName may be a better property than TableID
                return this.TableID;
            }
        }
    }
}
