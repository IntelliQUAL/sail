using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using SAIL.Framework.Host.Enums;

namespace SAIL.Framework.Host
{
    public interface IAPIGuideHost
    {
        // To be used within the service
        string ReadAPIGuideExampleRequest(IContext context, IService service, RESTAction restAction, ResponseFormat responseFormat);
        string ReadAPIGuideExampleResponse(IContext context, IService service, RESTAction restAction, ResponseFormat responseFormat);
        string ReadAPIGuideDocumentationHtml(IContext context, IService service, ResponseFormat responseFormat);

        // To be used within the host
        bool IsAPIGuideRequest(IContext context);
        string ReadAPIGuideResponseText(IContext context, IService service, ResponseFormat responseFormat);
    }
}
