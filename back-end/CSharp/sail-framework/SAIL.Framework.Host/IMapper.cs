using System;
using System.Text;
using System.Collections.Generic;

using SAIL.Framework.Host;

namespace SAIL.Framework.Host
{
    public interface IMapper<TSource, TDest>
    {
        void Map(IContext context, TSource source, TDest dest);
    }
}
