using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.Serialization;

using SAIL.Framework.Host.BaseClasses;

namespace IQ.ViewModel.Vast
{
    [Serializable]
    [DataContract(Namespace = "")]
    public class Grid : ViewModelResultBase
    {
        // ******************************************
        // ***** STOP: DON'T ADD ANY MORE FIELDS ****
        // ***** INSTEAD ADD to AttributeList    ****
        // ***** AND AN ASSOCIATIVE ARRAY        ****
        // ***** LIKE WE DO WITH sysBrand        ****
        // ******************************************
        [DataMember]
        public string DisplayName = string.Empty;                           // Usually the Table.DisplayName

        [DataMember]
        public string IconName = string.Empty;                              // Glyphicon or filename

        [DataMember]
        public List<FieldSchema> FieldSchemaList = null;

        [DataMember]
        public List<string[]> RowList = null;

        [DataMember]
        public int CurrentPage = 1;

        [DataMember]
        public int TotalPages = 1;

        [DataMember]
        public int RowsPerPage = 1;

        [DataMember]
        public int TotalRows = 1;

        [DataMember]
        public string PrimaryFieldID = string.Empty;    // Assumption: Each Form must have a single PrimaryFieldID

        [DataMember]
        public bool ShowFreeFormSearch = true;

        [DataMember]
        public string DefaultSortColumn = string.Empty;

        [DataMember]
        public string DefaultSortOrder = string.Empty;

        [DataMember]
        public string ViewType = string.Empty;          // Equates to a known Type / Name within the UI for example: "grid-flip-scroll", "grid-responsive", "BarChar", "LineChart", etc.

        [DataMember]
        public string ViewName = "Primary";             // Equates to a given view name e.g. "Primary", "Child1", "Child2" etc.

        [DataMember(EmitDefaultValue = false)]
        public List<ChildTable> ChildTableList = null;

        [DataMember(EmitDefaultValue = false)]
        public List<ClientAction> ClientActionList = null;

        [DataMember]
        public bool AllowDragDropReorder = false;

        [DataMember]
        public bool AllowNew = true;                    // Is the end-user allowed to create a new record?

        [DataMember]
        public bool AllowRead = true;                   // Is the end-user allowed read any existing record. This normally results in an 'Edit' link on each individual row.

        [DataMember]
        public bool AllowDelete = true;                 // Is the end-user allowed to delete any existing record. This normally results in a 'Delete' link on each individual row.

        [DataMember]
        public string ButtonText = "Add New";           // Primary Button

        // ******************************************
        // ***** STOP: DON'T ADD ANY MORE FIELDS ****
        // ***** INSTEAD ADD to AttributeList    ****
        // ***** AND AN ASSOCIATIVE ARRAY        ****
        // ***** LIKE WE DO WITH sysBrand        ****
        // ******************************************
        [DataMember(EmitDefaultValue = false)]
        public AttributeData Attributes = null;                  // Critical: These values are copied to Entity.Attributes.* as associative array in javascript.

        public FieldSchema FieldByID(string Id)
        {
            FieldSchema column = null;

            foreach (FieldSchema localField in this.FieldSchemaList)
            {
                if (localField.ID == Id)
                {
                    column = localField;
                    break;
                }
            }

            return column;
        }

        public string FieldValueByID(string[] row, string Id)
        {
            string columnValue = string.Empty;

            FieldSchema column = FieldByID(Id);

            if (column != null)
            {
                columnValue = row[column.OrdinalPosition];
            }

            return columnValue;
        }

        internal void AddEntity(Entity entity)
        {
            List<string> data = new List<string>();

            foreach (IQ.ViewModel.Vast.FieldGroup colGroup in entity.FieldGroupList)
            {
                if (colGroup.FieldRowList != null)
                {
                    foreach (FieldRow columnRow in colGroup.FieldRowList)
                    {
                        if (columnRow.FieldList != null)
                        {
                            foreach (IQ.ViewModel.Vast.Field col in columnRow.FieldList)
                            {
                                if (FieldByID(col.ID) == null)
                                {
                                    this.FieldSchemaList.Add(col.Schema);
                                }

                                data.Add(col.Value);
                            }
                        }
                    }
                }
            }

            if (this.RowList == null)
            {
                this.RowList = new List<string[]>();
            }

            this.RowList.Add(data.ToArray());
        }

        public Dictionary<string, string> ReadRow(string[] row)
        {
            Dictionary<string, string> columnData = new Dictionary<string, string>();

            foreach (FieldSchema column in this.FieldSchemaList)
            {
                try
                {
                    string value = row[FieldByID(column.ID).OrdinalPosition];

                    columnData.Add(column.ID, value);
                }
                catch { }
            }

            return columnData;
        }

        /// <summary>
        /// Created:        08/31/2015
        /// Author:         David J. McKee
        /// Purpose:        Appends an entire row in one show
        /// Critical:       The Column Schema ordinal position must start at 0!
        /// </summary>
        public void AppendRow(Dictionary<string, string> row)
        {
            string[] newRow = new string[this.FieldSchemaList.Count];

            foreach (string key in row.Keys)
            {
                FieldSchema col = this.FieldByID(key);

                if (col != null)
                {
                    newRow[col.OrdinalPosition] = row[key];
                }
            }

            if (this.RowList == null)
            {
                this.RowList = new List<string[]>();
            }

            this.RowList.Add(newRow);
        }
    }
}
