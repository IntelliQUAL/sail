using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using IQ.Entities.VastMetaDB.Enums;
using System.Runtime.Serialization;

namespace IQ.Entities.VastMetaDB.SqlEsque
{
    [Serializable]
    [DataContract(Namespace = "")]
    public class Predicate
    {
        private int _placeholderIndex = 0;
        private string _columnName = string.Empty;
        private string _suffix = string.Empty;
        private string _prefix = string.Empty;

        [DataMember]
        public string ColumnName
        {
            get { return _columnName; }
            set { _columnName = value; }
        }

        private OperatorType _operatorType = OperatorType.EqualTo;

        [DataMember]
        public OperatorType OperatorType
        {
            get { return _operatorType; }
            set { _operatorType = value; }
        }

        private dynamic _value = null;

        [DataMember(EmitDefaultValue = false)]
        public dynamic Value
        {
            get { return _value; }
            set { _value = value; }
        }

        public Predicate(string columnName, OperatorType operatorType, dynamic value)
        {
            _columnName = columnName;
            _operatorType = operatorType;
            _value = value;
        }

        public string PlaceholderName
        {
            get
            {
                return _columnName + _placeholderIndex;
            }
        }

        public int PlaceholderIndex
        {
            set { _placeholderIndex = value; }
            get { return _placeholderIndex; }
        }

        [DataMember]
        public string Suffix
        {
            get { return _suffix; }
            set { _suffix = value; }
        }

        [DataMember]
        public string Prefix
        {
            get { return _prefix; }
            set { _prefix = value; }
        }
    }
}
