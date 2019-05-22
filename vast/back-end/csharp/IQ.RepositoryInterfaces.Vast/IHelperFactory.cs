using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace IQ.RepositoryInterfaces.Vast
{
    /// <summary>
    /// Created:        06/15/2016
    /// Author:         David J. McKee
    /// Purose:         In this case "helpers" are common methods supplied by the assembly line project to the operations.
    /// </summary>

    public interface IHelperFactory
    {
        IHelper<TableDataModelType, ColumnDataModelType, EntityDataModelType, SearchRequestDataModelType, SearchResponseDataModelType> GetHelper<TableDataModelType, ColumnDataModelType, EntityDataModelType, SearchRequestDataModelType, SearchResponseDataModelType>();
    }
}
