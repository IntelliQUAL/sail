using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace IQ.Entities.VastDB.Const
{
    public class QueryString
    {
        public const string DOMAIN_NAME = "d";
        public const string ACCOUNT_CODE = "a";                     // Unique identifier for an instance of cloud data within a domain name.
        public const string ACCOUNT_STATUS = "s";                   // Account Status
        public const string MODULE_ID = "m";                        // Module ID
        public const string FORM_ID = "f";                          // FormID
        public const string PARENT_FORM_ID = "p";                   // Parent FormID
        public const string PARENT_FORM_PRIMARY_KEY = "k";          // Parent Form Primary Key
        public const string CUSTOM_DATA_PIPELINE = "c";             // Custom Data Pipeline           
        public const string ACTION_TYPE = "t";                      // New, Create, Read, Update, Delete, or Search
        public const string AUTH_TOKEN = "n";                       // Authentication Token
        public const string QS_PRIMARY_KEY = "pk";                  // Used to pass the primary key on the query string
        public const string INPUT_TYPE = "InputType";               // Used by Embedded documents for a single field.
        public const string FOREIGN_TABLE_ID = "ForeignTableID";    // Used for Embedded Controls which require another table for upload.
        public const string PRIMARY_KEY = "PrimaryKey";             // Used for Embbeded post-back PrimaryKey for updates
        public const string FIELD_ID = "ColumnID";                  // TODO: Change to 'FieldID' or something shorter. Used for Embbeded post-back ColumnID from the main Table.
        public const string FIELD_VALUE = "ColumnValue";            // TODO: Change to 'FieldValue' or something shorter Used for Embbeded post-back ColumnValue into the main Table.
        public const string FORMAT = "Format";                      // Specifies the format for an Input Type such as for Embedded Documents
        public const string EMBEDDED_URL = "eurl";                  // Embedded URL
    }
}
