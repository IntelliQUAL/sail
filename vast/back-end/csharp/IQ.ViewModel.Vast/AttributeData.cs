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
    public class AttributeData
    {
        [DataMember]
        public List<FieldSchema> FieldSchemaList = null;

        [DataMember]
        public List<string[]> RowList = null;
    }
}
