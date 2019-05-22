using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using IQ.BUS.VAST.Helpers;
using IQ.RepositoryInterfaces.Vast;

namespace IQ.BUS.Vast.Helpers
{
    class HelperFactory : IHelperFactory
    {
        IHelper<TableDataModelType, ColumnDataModelType, EntityDataModelType, SearchRequestDataModelType, SearchResponseDataModelType> IHelperFactory.GetHelper<TableDataModelType, ColumnDataModelType, EntityDataModelType, SearchRequestDataModelType, SearchResponseDataModelType>()
        {
            dynamic helper = new Helper();

            return helper;
        }
    }
}
