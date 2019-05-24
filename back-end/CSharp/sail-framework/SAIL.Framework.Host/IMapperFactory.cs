using System;
using System.Text;
using System.Collections.Generic;

using SAIL.Framework.Host;

namespace SAIL.Framework.Host
{
    public interface IMapperFactory
    {
        IMapper<TSource, TDest> Get<TSource, TDest>(IContext context, string fullName);                 // Load the mapper implementation by exact name
    }
}
