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
    public class FieldEnum
    {
        [DataMember]
        public string DisplayName = string.Empty;

        [DataMember]
        public string Value = string.Empty;
    }
}
