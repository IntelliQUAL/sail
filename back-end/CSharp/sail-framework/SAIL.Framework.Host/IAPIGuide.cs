using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using SAIL.Framework.Host.Enums;

namespace SAIL.Framework.Host
{
    public interface IAPIGuide
    {
        string ExampleRequest(IContext context, ResponseFormat responseFormat);
        string ExampleResponse(IContext context, ResponseFormat responseFormat);
        string DocumentationHtml(IContext context, ResponseFormat responseFormat);
    }
}
