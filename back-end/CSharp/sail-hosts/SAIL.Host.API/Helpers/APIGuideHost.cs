using System;
using System.Linq;
using System.Web;
using System.Collections.Generic;

using SAIL.Framework.Host;
using SAIL.Framework.Host.Enums;

namespace SAIL.Host.API.Helpers
{
    public class APIGuideHost : IAPIGuideHost
    {
        private const string QUERY_STRING_EXAMPLE = "e";
        private const string EXAMPLE_REQUEST = "examplerequest";
        private const string EXAMPLE_RESPONSE = "exampleresponse";
        private const string DOCKS = "docs";

        bool IAPIGuideHost.IsAPIGuideRequest(IContext context)
        {
            bool isAPIGuideRequest = false;

            string example = context.Get<IConnectionHost>().RequestQueryString(QUERY_STRING_EXAMPLE);

            if (string.IsNullOrWhiteSpace(example) == false)
            {
                example = example.ToLower().Trim();
            }

            switch (example)
            {
                case EXAMPLE_REQUEST:
                case EXAMPLE_RESPONSE:
                case DOCKS:
                    isAPIGuideRequest = true;
                    break;

                default:
                    isAPIGuideRequest = false;
                    break;
            }

            return isAPIGuideRequest;
        }

        private string ReadAPIGuideByType(IContext context, IService service, string guideType, RESTAction restAction, ResponseFormat responseFormat)
        {
            string exampleRequest = string.Empty;

            const string SUB_PATH = "APIGuide";

            IPathIO pathIo = context.Get<IPathIO>();
            IFileIO fileIo = context.Get<IFileIO>();
            IBinding binding = context.Get<IBinding>();

            List<string> servicePathList = binding.ReadServicePathList(context);

            string fileName = string.Empty;

            if (guideType == "Docs")
            {
                fileName = service.GetType().FullName + "-" + guideType + "." + responseFormat.ToString();
            }
            else
            {
                fileName = service.GetType().FullName + "-" + guideType + "-" + restAction + "." + responseFormat.ToString();
            }

            foreach (string servicePath in servicePathList)
            {
                string childPath = pathIo.Combine(context, servicePath, SUB_PATH);

                List<string> fileList = pathIo.GetFilesSortedByCreateDate(context, childPath, true);

                foreach (string filePathname in fileList)
                {
                    if (fileName.ToLower() == fileIo.FileNameOnly(context, filePathname).ToLower())
                    {
                        exampleRequest = fileIo.ReadTextFileContents(context, filePathname);
                        break;
                    }
                }
            }

            return exampleRequest;
        }

        string IAPIGuideHost.ReadAPIGuideDocumentationHtml(IContext context, IService service, ResponseFormat responseFormat)
        {
            return ReadAPIGuideByType(context, service, "Docs", RESTAction.SearchAll, responseFormat);
        }

        string IAPIGuideHost.ReadAPIGuideExampleRequest(IContext context, IService service, RESTAction restAction, ResponseFormat responseFormat)
        {
            return ReadAPIGuideByType(context, service, "ExampleRequest", restAction, responseFormat);
        }

        string IAPIGuideHost.ReadAPIGuideExampleResponse(IContext context, IService service, RESTAction restAction, ResponseFormat responseFormat)
        {
            return ReadAPIGuideByType(context, service, "ExampleResponse", restAction, responseFormat);
        }

        string IAPIGuideHost.ReadAPIGuideResponseText(IContext context, IService service, ResponseFormat responseFormat)
        {
            string responseText = string.Empty;

            string example = context.Get<IConnectionHost>().RequestQueryString(QUERY_STRING_EXAMPLE);

            if (string.IsNullOrWhiteSpace(example) == false)
            {
                example = example.ToLower().Trim();
            }

            switch (example)
            {
                case EXAMPLE_REQUEST:
                    responseText = ((IAPIGuide)service).ExampleRequest(context, responseFormat);
                    break;

                case EXAMPLE_RESPONSE:
                    responseText = ((IAPIGuide)service).ExampleResponse(context, responseFormat);
                    break;

                case DOCKS:
                    responseText = ((IAPIGuide)service).DocumentationHtml(context, responseFormat);
                    break;
            }

            return responseText;
        }
    }
}