using System;
using System.Text;
using System.Collections.Generic;

namespace SAIL.ViewModel.SqlEsque
{
    public class Query
    {
        public int? page = null;                        // Page to return. If null 1 is assumed.
        public int? rowsPerPage = null;                 // Number of rows to return per page.
        public bool distinct = false;                   // Return distinct rows?
        public string entity { get; set; }              // Table name / collection name / etc.
        public string returnColumns = string.Empty;     // Pipe delimited list of columns to return if 0 or null all columns are returned.
        public where where = null;                      // Request filter
        public orderBy orderBy = null;                  // Order by
    }
}
