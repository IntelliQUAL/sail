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
    public class Entity : ViewModelResultBase
    {
        // ******************************************
        // ***** STOP: DON'T ADD ANY MORE FIELDS ****
        // ***** INSTEAD ADD to AttributeList    ****
        // ***** AND AN ASSOCIATIVE ARRAY        ****
        // ***** LIKE WE DO WITH sysBrand        ****
        // ******************************************
        [DataMember(EmitDefaultValue = false)]
        public string DomainName = null;

        [DataMember(EmitDefaultValue = false)]
        public string AccountCode = null;

        [DataMember(EmitDefaultValue = false)]
        public string AccountStatus = null;

        [DataMember(EmitDefaultValue = false)]
        public string ModuleID = null;

        [DataMember(EmitDefaultValue = false)]
        public string FormID = null;

        [DataMember(EmitDefaultValue = false)]
        public string PrimaryKey = null;

        [DataMember]
        public bool ImmediateSubmission = false;

        [DataMember]
        public string DisplayName = string.Empty;           // Usually the Table.DisplayName

        [DataMember]
        public List<FieldGroup> FieldGroupList = null;    // TODO: Change to FieldGroupList

        [DataMember(EmitDefaultValue = false)]
        public List<ChildTable> ChildTableList = null;

        [DataMember(EmitDefaultValue = false)]
        public List<ClientAction> ClientActionList = null;

        [DataMember]
        public string TransactionID = string.Empty;

        // Indicates how to handle the response e.g. Handle like a "New", "Create", "Read", "Update", "Delete" or "Search".
        // This will usually match the original request but is not required to.
        [DataMember]
        public string ActionResponseType = string.Empty;

        [DataMember]
        public string SpecificActionResponseType = string.Empty;    // Used in conjunction with ActionResponseType = IQ.Entities.VastDB.Const.Actions.ACTION_SPECIFIC

        [DataMember]
        public string ButtonText = string.Empty;

        [DataMember]
        public string AuthToken = string.Empty;

        [DataMember(EmitDefaultValue = false)]
        public string CustomAssemblyLineName = null;                // At present this is used for session persistance and may be used in conjunction with ActionResponseType = IQ.Entities.VastDB.Const.Actions.ACTION_SPECIFIC

        [DataMember]
        public string ViewType = string.Empty;                      // Equates to a known Type / Name within the UI for example: "crud", "dashboard", etc.

        [DataMember]
        public string ViewName = "Primary";                         // Equates to a given view name e.g. "Primary", "Child1", "Child2" etc.

        [DataMember(EmitDefaultValue = false)]
        public EntitySearch SearchInfo = null;                      // Used in conjunction with ActionResponseType = IQ.Entities.VastDB.Const.Actions.ACTION_SPECIFIC to force a search from the client.

        // ******************************************
        // ***** STOP: DON'T ADD ANY MORE FIELDS ****
        // ***** INSTEAD ADD to AttributeList    ****
        // ***** AND AN ASSOCIATIVE ARRAY        ****
        // ***** LIKE WE DO WITH sysBrand        ****
        // ******************************************
        [DataMember(EmitDefaultValue = false)]
        public AttributeData Attributes = null;                  // Critical: These values are copied to Entity.Attributes.* as associative array in javascript.

        public Field FieldByGuid(string id)
        {
            Field column = null;

            if (this.FieldGroupList != null)
            {
                foreach (FieldGroup fieldGroup in FieldGroupList)
                {
                    if (fieldGroup.FieldRowList != null)
                    {
                        foreach (FieldRow fieldRow in fieldGroup.FieldRowList)
                        {
                            if (fieldRow.FieldList != null)
                            {
                                foreach (Field localField in fieldRow.FieldList)
                                {
                                    if (localField.ID == id)
                                    {
                                        column = localField;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return column;
        }

        public string FieldValueByID(string id)
        {
            string columnValue = string.Empty;

            Field column = FieldByGuid(id);

            if (column != null)
            {
                columnValue = column.Value;
            }

            return columnValue;
        }
    }
}