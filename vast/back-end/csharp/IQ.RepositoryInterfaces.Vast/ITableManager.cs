using System;
using SAIL.Framework.Host;

namespace IQ.RepositoryInterfaces.Vast
{
    public interface ITableManager
    {
        bool DropTable(IContext context, string instanceGUID, string databaseName, string tableId);
        bool EnsureColumn(IContext context, string instanceGUID, string databaseName, string tableId, IQ.Entities.VastMetaDB.ColumnSchema columnSchema);
        bool EnsureTable(IContext context, string instanceGUID, string databaseName, IQ.Entities.VastMetaDB.Table table);
    }
}
