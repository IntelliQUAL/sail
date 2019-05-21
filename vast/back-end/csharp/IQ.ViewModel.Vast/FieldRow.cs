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
    public class FieldRow
    {
        [DataMember]
        public int OrdinalPosition = 0;

        [DataMember]
        public List<Field> FieldList = null;
    }
}
