using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using IQ.Entities.VastMetaDB.Enums;

namespace IQ.Entities.VastMetaDB
{
    public class ColumnSchema
    {
        public string ID = string.Empty;                                    // Same as Column.ID
        public string DisplayName = string.Empty;                           // Name displayed to the end-user for input
        public string Description = string.Empty;                           // Internal Description of the given field.
        public string HelpTitle = string.Empty;                             // Title for the Help Link
        public string HelpText = string.Empty;                              // External or "User" help information further describing the current input field.
        public DateTime DateCreated = DateTime.UtcNow;
        public string TableID = null;                                     // This is used to support features such as Pivit Tables
        public int OrdinalPosition = 0;
        public bool HasDefaultValue = false;
        public string DefaultValue = string.Empty;
        public ColumnDataType DataType = ColumnDataType.Unknown;
        public bool ValidateMinMax = false;
        public int? Min = null;
        public int? Max = null;
        public DataFormat Format = DataFormat.Unknown;
        public bool SortDescending = false;                                 // Sort data in descending order by default.
        public DataInputType InputType = DataInputType.TextBox;
        public string Enum = string.Empty;                                  // comma delimited list of acceptable values relative to DataInputType
        public string ForeignModuleID = string.Empty;
        public string ForeignTableID = string.Empty;                        // Used when a given value must be pulled from another table. 
        public string ForeignTableSearchCriteria = string.Empty;
        public string ConfigurationWarning = string.Empty;                  // Warning text indicating that there is a problem with the schema data.
        public bool Required = false;
        public string DisplaySequence = "0";                                // "1" or "1.1" or "1.2" or "2" or "2.2" (all 1s on same line and all 2s on same line)
        public string Pattern = string.Empty;
        public string GroupID = string.Empty;
        public bool TrustAsHtml = false;                                    // Render the value as html on the given page.
        public InternalVisibility Internal = InternalVisibility.External;   // This is an internal column and should not be returned via the external service interface.
        public bool GridVisible = true;                                     // Visible within the search response grid display
        public string Placeholder = string.Empty;                           // Placeholder text usually shown within a textbox e.g.: placeholder="yoursitename"
        public string ErrorMessage = string.Empty;                          // Any field level error i.e. Validation or consistency e.g. uniqueness.

        // Filter Items
        public bool FilterEnabled = false;
        public FilterType FilterType = FilterType.FreeFormSearch;
        public string FilterDefaultValue = string.Empty;                    // Default Search criteria for this field.
        public bool Unique = false;
        public string UniqueErrorMessage = string.Empty;

        // Grid System
        public ColumnLayout Layout = new ColumnLayout();
    }
}
