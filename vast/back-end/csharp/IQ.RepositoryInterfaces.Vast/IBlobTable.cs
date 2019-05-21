using System;
using System.IO;

using SAIL.Framework.Host;

namespace IQ.RepositoryInterfaces.Vast
{
    public interface IBlobTable
    {
        dynamic CreateStream(IContext context, string parentTableId, string parentRowPK, string tableId, Stream stream, string fileExtension);
        Stream ReadStream(IContext context, string parentTableId, string parentRowPK, string tableId, dynamic rowPK, ref string fileExtension);
        void UpdateStream(IContext context, string parentTableId, string parentRowPK, string tableId, dynamic rowPK, Stream stream, string fileExtension);
    }
}
