using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using SAIL.Framework.Host;

namespace SAIL.Infrastructure.TypeConversion
{
    public class MapperFactory : IMapperFactory
    {
        IMapper<TSource, TDest> IMapperFactory.Get<TSource, TDest>(IContext context, string fullName)
        {
            return (IMapper<TSource, TDest>)context.Get<IBinding>().LoadViaFullName(context, fullName);
        }
    }
}
