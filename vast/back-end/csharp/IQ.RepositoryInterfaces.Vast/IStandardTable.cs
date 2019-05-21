using System;

using SAIL.Framework.Host;

namespace IQ.RepositoryInterfaces.Vast
{
    public interface IStandardTable : ITableManager
    {
        void Create(IContext context, string instanceGUID, string databaseName, string parentTableId, string parentRowPK, string tableId, IQ.Entities.VastDB.Entity newEntity);
        void Delete(IContext context, string instanceGUID, string databaseName, string parentTableId, string parentTablePkColumnID, string parentRowPK, string tableId, string tablePkColumnID, dynamic tableColumnPk);
        IQ.Entities.VastDB.Entity New(IContext context, string instanceGUID, string databaseName, string parentTableId, string parentTablePkColumnID, string parentRowPK, string tableId);
        IQ.Entities.VastDB.Entity Read(IContext context, string instanceGUID, string databaseName, string parentTableId, string parentTablePkColumnID, string parentRowPK, string tableId, string tablePkColumnID, dynamic tableColumnPk);
        IQ.Entities.VastDB.SearchResponse Search(IContext context, string instanceGUID, string databaseName, string parentTableId, string parentRowPK, string tableId, IQ.Entities.VastDB.EntitySearch searchCriteria);
        void Update(IContext context, string instanceGUID, string databaseName, IQ.Entities.VastDB.Entity existingEntity);
    }
}
