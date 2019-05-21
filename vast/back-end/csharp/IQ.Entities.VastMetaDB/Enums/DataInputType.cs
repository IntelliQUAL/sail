using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQ.Entities.VastMetaDB
{
    public enum DataInputType
    {
        Text = 10,                      // Read-only Label                
        CheckBox = 20,
        RadioButtons = 30,              // Data Pulled from Enum column or ForeignTableID
        CheckBoxList = 40,              // Data Pulled from Enum column or ForeignTableID
        DropDown = 50,                  // Data Pulled from Enum column or ForeignTableID
        MaskedTextBox = 60,             // Password or masked value
        MaskedTextBoxWithConfirm = 70,  // Password or masked value with Confirmation
        TextBox = 80,                   // 
        MemoBox = 90,
        DatePicker = 100,
        DateTimePicker = 3000,
        TimePicker = 3100,
        Number = 3001,                  // Use 'Pattern' field for Money e.g. ^\d+(\.|\,)\d{2}$
        Spinner = 3002,                 // SPINNER up/down number input
        SelectList = 3003,
        URL = 3004,
        UserPicker = 3005,
        // Advanced
        Map = 3006,
        Lookup = 3007,                  // Lookup from another table. Data from ForeignTableID
        LookupWithAdd = 3014,           // Lookup from another table with an add button. Data from ForeignTableID
        LookupWithAddDetail = 3024,     // Lookup from another table with an Add button plus detail about the row selected.
        Signature = 5100,               // Sign using a touch-screen or mouse       http://thomasjbradley.ca/lab/signature-pad/accept/
        Hidden = 5200,                  // Not visible to the end-user.
        FileUpload = 6000,              // Single File Upload        
        StarRating = 8000,              // star-rating e.g. 1 to 5 stars
        FormPicker = 9000,              // Select from a list of existing forms. 
        MultiSelectGrid = 10000         // Displays a full grid containing a select box which equates to a list of IDs within a single field.
    }
}
