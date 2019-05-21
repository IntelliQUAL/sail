using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using IQ.Entities.VastMetaDB;

namespace IQ.Entities.VastDB.Helpers
{
    internal static class TableHelper
    {
        public static Table LoadTableFromColumnIdValuePair(string tableId, Dictionary<string, string> columnIdValuePair, Table table)
        {
            if (table == null)
            {
                table = new Entities.VastMetaDB.Table();
                table.ID = tableId;
            }

            if (table.ColumnList == null)
            {
                table.ColumnList = new List<Entities.VastMetaDB.Column>();
            }

            foreach (string columnName in columnIdValuePair.Keys)
            {
                if (table.GetColumn(columnName) == null)
                {
                    int ordinalPosition = table.MaxOrdinalPosition + 1;
                    table.ColumnList.Add(new Column(ordinalPosition, columnName));
                }
            }

            return table;
        }
    }
}
