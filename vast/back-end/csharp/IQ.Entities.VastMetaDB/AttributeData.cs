using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace IQ.Entities.VastMetaDB
{
    public class AttributeData
    {
        public List<Column> _columnList = null;                  // Contains all meta information about the RowList

        public List<Column> ColumnList
        {
            get
            {
                if (_columnList == null)
                {
                    _columnList = new List<Column>();
                    _columnList.Add(new Column(0, "Key"));
                    _columnList.Add(new Column(1, "Value"));
                }

                return _columnList;
            }
        }

        public List<string[]> RowList = new List<string[]>();
    }
}
