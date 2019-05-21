using System;
using System.Runtime.Serialization;

namespace SAIL.Framework.Host.Types
{
    [Serializable]
    [DataContract(Namespace = "")]
    public class Result
    {
        private bool _success = true;

        [DataMember]
        public bool Success
        {
            get { return _success; }
            set { _success = value; }
        }

        private string _errorCode = string.Empty;

        [DataMember]
        public string ErrorCode
        {
            get { return _errorCode; }
            set { _errorCode = value; }
        }

        private ErrorMessages _errorMessages;

        [DataMember]
        public ErrorMessages ErrorMessages
        {
            get { return _errorMessages; }
            set { _errorMessages = value; }
        }

        private string _errorText = string.Empty;

        [DataMember]
        public string ErrorText
        {
            get { return _errorText; }
            set { _errorText = value; }
        }

        private string _userMessage = string.Empty;

        [DataMember]
        public string UserMessage
        {
            get { return _userMessage; }
            set { _userMessage = value; }
        }
    }
}
