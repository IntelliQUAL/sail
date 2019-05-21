using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using IQ.ViewModel.VastDB.Enums;

namespace IQ.ViewModel.Vast
{
    public class FieldSchema
    {
        // This is repeated on the column also
        public string ID = string.Empty;                                                    // Field Identifier
        public string DisplayName = string.Empty;
        public string Description = string.Empty;                                           // Data dictionary entry. Not for end-user consumption. Use HelpText instead.
        public DateTime DateCreated = DateTime.UtcNow;
        public int OrdinalPosition = 0;
        // public bool HasDefaultValue = false;
        public string DefaultValue = string.Empty;
        public string DataType = string.Empty;
        public int? Min = null;
        public int? Max = null;
        public string Format = string.Empty;                                                // Used in conjunction with InputType to format the control.
        public bool SortDescending = false;                                                 // Sort data in descending order by default.
        public string InputType = DataInputType.TextBox.ToString();
        public List<FieldEnum> Enum = null;                                                 // List of Value / DisplayName items relative to DataInputType. Not valid for all types
        public bool Hidden = false;                                                         // Hidden from user view

        public string ForeignModuleID = string.Empty;
        public string ForeignTableID = string.Empty;                                        // Returned only when data must be pulled from a Foreign Table from the client. Examples include embedded documents and Multi-Select Grids
        public string ForeignTableSearchCriteria = string.Empty;

        public string GroupID = string.Empty;                                               // GroupID for the group that contains this column.
        public string Placeholder = string.Empty;                                           // Placeholder text usually shown within a textbox e.g.: placeholder="yoursitename"
        public string ErrorMessage = string.Empty;

        // End-User Help
        public string HelpText = string.Empty;                                              // External end-user help. Use Description for data dictionary.        
        public string HelpTitle = string.Empty;

        // Search Filters
        public bool FilterEnabled = false;                                                  // Is a filter enabled for this Field?
        public string FilterType = IQ.ViewModel.Vast.Enums.FilterType.NoFilter.ToString();
        public string FilterDefaultValue = string.Empty;                                    // Default Search criteria for this field.
        public string FilterValue = string.Empty;                                           // Current Filter Value.        

        // Grid System
        public FieldLayout Layout = new FieldLayout();
    }
}