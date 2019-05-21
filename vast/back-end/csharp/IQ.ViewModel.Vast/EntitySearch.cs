using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.Serialization;

using SAIL.ViewModel.SqlEsque;

namespace IQ.ViewModel.Vast
{
    [Serializable]
    [DataContract(Namespace = "")]
    public class EntitySearch
    {
        [DataMember(EmitDefaultValue = false)]
        public int? Page = null;

        [DataMember(EmitDefaultValue = false)]
        public int? RowsPerPage = null;

        [DataMember]
        public where Where = null;

        [DataMember]
        public orderBy OrderBy = null;

        [DataMember]
        public bool Distinct = false;                   // Return distinct rows.

        [DataMember]
        public string ReturnColumnList = string.Empty;  // List of columns to return if 0 or null all columns are returned.

    }
}
