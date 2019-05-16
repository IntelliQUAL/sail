using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SAIL.Framework.Host.Consts
{
    public class Context
    {
        public const string CORRELATION_ID = "CorrelationId";
        public const string TABLE_OR_COLLECTION_NAME = "TableOrCollectionName";
        //public const string TABLE_OR_COLLECTION_NAME_PRIMARY_KEY_FIELD = "TableOrCollectionNamePrimaryKeyField";
        public const string PARENT_TABLE_OR_COLLECTION_NAME = "ParentTableOrCollectionName";
        public const string PARENT_TABLE_OR_COLLECTION_NAME_PRIMARY_KEY = "ParentTableOrCollectionNamePrimaryKey";
        public const string PARENT_TABLE_OR_COLLECTION_CREATE_DATE_UTC = "ParentTableOrCollectionCreateDateUTC";
        public const string HTTP_STATUS_CODE = "HttpStatusCode";
    }
}
