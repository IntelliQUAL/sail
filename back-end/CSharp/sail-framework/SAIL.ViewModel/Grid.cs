using System;
using System.Text;
using System.Collections.Generic;

using SAIL.Framework.Host;

namespace SAIL.ViewModel.Generic
{
    public class Grid : IServiceResponse
    {
        private string _correlationId = string.Empty;

        public bool success = false;
        public string errorCode = string.Empty;
        public string errorText = string.Empty;
        public string userMessage = string.Empty;

        public string entity { get; set; }      // Table name / collection name / etc.

        public List<string> columns = null;
        public List<string[]> rows = null;

        public Dictionary<string, string> ReadRow(string[] row)
        {
            Dictionary<string, string> columnData = new Dictionary<string, string>();

            int valueIndex = 0;

            foreach (string key in this.columns)
            {
                try
                {
                    columnData[key] = row[valueIndex];

                    valueIndex++;
                }
                catch { }
            }

            return columnData;
        }

        public void AddRow(Row entity)
        {
            List<string> data = new List<string>();

            foreach (string column in this.columns)
            {
                if (entity.fields.ContainsKey(column))
                {
                    data.Add(entity.fields[column].ToString());
                }
                else
                {
                    data.Add(null);
                }
            }

            if (this.rows == null)
            {
                this.rows = new List<string[]>();
            }

            this.rows.Add(data.ToArray());
        }

        // IServiceResponse
        bool IServiceResponse.Success
        {
            get
            {
                return success;
            }
            set
            {
                success = value;
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
