using System;

namespace SAIL.Framework.Host.Types
{
    public class ErrorMessage
    {
        public ErrorMessage() { }

        public ErrorMessage(string errorCode)
        {
            ErrorCode = errorCode;
        }

        public ErrorMessage(string errorCode, params object[] parameter)
        {
            ErrorCode = errorCode;
            Parameters = parameter;
        }

        public ErrorMessage(string errorCode, string userMessage, params object[] parameter)
        {
            ErrorCode = errorCode;
            UserMessage = string.Format(userMessage, parameter);
            Parameters = parameter;
        }

        public string ErrorCode { get; set; }
        public string UserMessage { get; set; }
        public object[] Parameters { get; set; }
    }
}
