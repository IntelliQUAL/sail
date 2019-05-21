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
    public class ClientAction : ChildTable
    {
        [DataMember]
        public string url = string.Empty;           // Full URL, created on the server-side for the given action

        [DataMember]
        public string target = string.Empty;        // Same as href target

        [DataMember]
        public string InputType = string.Empty;     // button, link, image, reset
    }
}
