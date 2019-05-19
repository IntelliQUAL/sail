using System;
using System.Collections.Generic;

using SAIL.Framework.Host;

namespace SAIL.ViewModel.Generic
{
    public class Row : IServiceResponse
    {
        private bool Success = true;
        public string errorCode = string.Empty;
        public string errorText = string.Empty;
        public string userMessage = string.Empty;
        private string _correlationId = string.Empty;

        public string id { get; set; }                  // Primary key / UUID / GUID of row
        public string parentId { get; set; }            // Primary key / UUID / GUID of row when parentEntity is supplied.
        public string entity { get; set; }              // Entity, table or collection name / etc.
        public string parentEntity { get; set; }        // Entity, table or collection name / etc. of the parent entity.
        public string parentCreateDateUtc { get; set; } // Time stamp when the parent entity was created.
        public Dictionary<string, object> fields { get; set; }

        // IServiceResponse
        bool IServiceResponse.Success
        {
            get
            {
                return this.Success;
            }
            set
            {
                this.Success = value;
            }
        }

        string IServiceResponse.ErrorCode
        {
            get
            {
                return errorCode;
            }
            set
            {
                errorCode = value;
            }
        }

        string IServiceResponse.ErrorText
        {
            get
            {
                return errorText;
            }
            set
            {
                errorText = value;
            }
        }

        string IServiceResponse.UserMessage
        {
            get
            {
                return userMessage;
            }
            set
            {
                userMessage = value;
            }
        }

        string IServiceResponse.CorrelationId
        {
            get
            {
                return _correlationId;
            }
            set
            {
                _correlationId = value;
            }
        }
    }
}
