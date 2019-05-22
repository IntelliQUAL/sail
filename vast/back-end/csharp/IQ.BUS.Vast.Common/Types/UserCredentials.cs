using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.Serialization;

using SAIL.Framework.Host;

namespace IQ.BUS.Vast.Common.Types
{
    [Serializable]
    [DataContract(Namespace = "")]
    public class UserCredentials : IAuthCredentials
    {
        private string _password = string.Empty;

        [DataMember(Order = 1)]
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
            }
        }

        private string _username = string.Empty;

        [DataMember(Order = 2)]
        public string Username
        {
            get
            {
                return _username;
            }
            set
            {
                _username = value;
            }
        }

        private string _apiKey = string.Empty;

        [DataMember(Order = 3)]
        public string ApiKey
        {
            get { return _apiKey; }
            set { _apiKey = value; }
        }

        private string _partnerCode = string.Empty;

        [DataMember(Order = 4)]
        public string PartnerCode
        {
            get { return _partnerCode; }
            set { _partnerCode = value; }
        }

        private string _accountCode = string.Empty;

        [DataMember(Order = 5)]
        public string AccountCode
        {
            get { return _accountCode; }
            set { _accountCode = value; }
        }

        Dictionary<string, string> IAuthCredentials.Credentials
        {
            get
            {
                Dictionary<string, string> credentials = new Dictionary<string, string>();

                return credentials;
            }
        }
    }
}
