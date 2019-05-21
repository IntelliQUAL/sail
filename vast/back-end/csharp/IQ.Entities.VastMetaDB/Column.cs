using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.Serialization;

using VastMetaDB.BaseClasses;

namespace IQ.Entities.VastMetaDB
{
    [Serializable]
    [DataContract(Namespace = "")]
    public class Column
    {
        /// <summary>
        /// Created:        04/16/2015
        /// Author:         ordinalPosition and id are critical for a column. Therefore they are always required.
        /// </summary>
        /// <param name="ordinalPosition"></param>
        /// <param name="id"></param>
        public Column(int ordinalPosition,
                        string id)
        {
            this.ID = id;

            if (this.Schema == null)
            {
                this.Schema = new ColumnSchema();
            }

            this.Schema.ID = id;
            this.Schema.OrdinalPosition = ordinalPosition;
        }

        [DataMember]
        public string ID = string.Empty;                // Same as ColumnSchema.ID

        [DataMember]
        public string Value = string.Empty;             // Column Value

        [DataMember]
        public string SensitiveValue = string.Empty;    // Encrypted value used when a 'Sensitive' piece of information is sent back to the UI.

        // We do this this way so we can set Schema to Null before sending in the data from the client.
        [DataMember]
        public ColumnSchema Schema = new ColumnSchema();
    }
}
