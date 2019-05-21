using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace IQ.Entities.VastMetaDB
{
    [Serializable]
    [DataContract(Namespace = "")]
    public class ColumnLayout
    {
        // Important: We have adapted the Boostrap grid system and adapted it for our own use.
        // Grid Classes
        // The Bootstrap grid system has four classes:
        // xs (for phones)
        // sm (for tablets)
        // md (for desktops)
        // lg (for larger desktops)
        // The classes above can be combined to create more dynamic and flexible layouts.

        // Tip: Each class scales up, so if you wish to set the same widths for xs and sm, you only need to specify xs. 
        // Grid System Rules
        // Some Bootstrap grid system rules:

        // Rows must be placed within a .container (fixed-width) or .container-fluid (full-width) for proper alignment and padding
        // Use rows to create horizontal groups of columns
        // Content should be placed within columns, and only columns may be immediate children of rows
        // Predefined classes like .row and .col-sm-4 are available for quickly making grid layouts
        // Columns create gutters (gaps between column content) via padding. That padding is offset in rows for the first and last column via negative margin on .rows
        // Grid columns are created by specifying the number of 12 available columns you wish to span. For example, three equal columns would use three .col-sm-4

        [DataMember]
        public string xs = string.Empty;   // (for phones)

        [DataMember]
        public string sm = string.Empty;   // (for tablets) 

        [DataMember]
        public string md = string.Empty;   // (for desktops)

        [DataMember]
        public string lg = string.Empty;   // (for larger desktops)
    }
}
