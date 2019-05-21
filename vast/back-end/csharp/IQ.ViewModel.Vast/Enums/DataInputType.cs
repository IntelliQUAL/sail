using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace IQ.ViewModel.VastDB.Enums
{
    public enum DataInputType
    {
        Text = 10,                      // Read-only Label                
        CheckBox = 20,
        RadioButtons = 30,
        CheckBoxList = 40,
        DropDown = 50,
        MaskedTextBox = 60,             // Password or hidden value with reveal option
        MaskedTextBoxWithConfirm = 70,  // Password or hidden value with Confirm
        TextBox = 80,
        DatePicker = 100,
        DateTimePicker = 3000,
        Number = 3001,                  // Use 'Pattern' field for Money e.g. ^\d+(\.|\,)\d{2}$
        Spinner = 3002,                 // SPINNER up/down number input
        SelectList = 3001,              // Based on Enum column within *ColumnSchema table
        URL = 3002,
        UserPicker = 3002,
        // Advanced
        Map = 3003,
        Lookup = 3004,                  // Lookup from another table.
        LookupWithAdd = 3005,           // Lookup from another table with an Add button
        LookupWithAddDetail = 3006,     // Lookup from another table with an Add button plus detail about the row selected.
        Hidden = 3007,
        Initial = 5000,                 // Initial using a touch-screen or mouse    http://thomasjbradley.ca/lab/signature-pad/accept/                
        StarRating = 7000,              // star-rating e.g. 1 to 5 stars
        EmbeddedDoc = 8000,             // Signature, FileUpload, MemoBox
        MultiSelectGrid = 9999          // Displays a full grid containing a select box which equates to a list of IDs within a single field.
    }
}
