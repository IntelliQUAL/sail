using System.Collections.Generic;

namespace SAIL.Framework.Host.Types
{
    public class ErrorMessages
    {
        public ErrorMessages()
        {
            ErrorMessageList = new List<ErrorMessage>();
        }

        public List<ErrorMessage> ErrorMessageList { get; set; }

        public void AddCode(string errorCode)
        {
            ErrorMessageList.Add(new ErrorMessage(errorCode));
        }
    }
}
