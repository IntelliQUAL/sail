using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IQ.ViewModel.Vast
{
    [Serializable]
    [DataContract(Namespace = "")]
    public class ChildTable
    {
        [DataMember]
        public string TableID = string.Empty;

        [DataMember]
        public string NewDisplayName = string.Empty;

        [DataMember]
        public string ActionType = string.Empty;

        [DataMember(EmitDefaultValue = false)]
        public string CustomAssemblyLineName = null;

        [DataMember]
        public string FilterFieldName = string.Empty;   // Generally equates the the field name in the child table who's name maps back to the parent table's primary key.

        [DataMember]
        public string ViewName = string.Empty;          // Unique identifier for a client-side view related to this child table.
    }
}
