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
    public class Field
    {
        [DataMember]
        public string ID = string.Empty;                            // Field Name or ID

        [DataMember(EmitDefaultValue = false)]
        public string Value = string.Empty;                         // Field Value

        [DataMember(EmitDefaultValue = false)]
        public string SensitiveValue = string.Empty;                // Encrypted value used when a 'Sensitive' piece of information is sent back to the UI.

        // We do this this way so we can set Schema to Null before sending in the data from the client.
        [DataMember(EmitDefaultValue = false)]
        public FieldSchema Schema = new FieldSchema();
    }
}
