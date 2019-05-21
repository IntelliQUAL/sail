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
    public class FieldGroup
    {
        [DataMember]
        public string ID = string.Empty;            // GroupID

        [DataMember]
        public string DisplayName = string.Empty;

        [DataMember]
        public bool Submitted = false;              // Very important: This is used to get around the size limitations of jsonp

        [DataMember]
        public List<FieldRow> FieldRowList = null;

        [DataMember]
        public bool Hidden = false;                 // This group contains only hidden fields.
    }
}
