using System;
using System.Net;
using System.Web;
using System.Text;
using System.Net.Http;
using System.Collections.Generic;

using SAIL.Framework.Host;
using SAIL.Framework.Host.Enums;
using SAIL.Framework.Host.Consts;
using SAIL.Framework.Host.Bootstrap;

namespace SAIL.Host.API.Helpers
{
    internal static class ControllerCommon
    {
        /// <summary>
        /// Created:        06/18/2017
        /// Author:         David J. McKee
        /// Purpose:        Loads and executes a "real" service by its' full name or alias
        /// Important:      This controller also includes support for jsonp, via ajax and jQuery
        ///                 Critical 'httpContext' will be technology specific and is resolved to IConnectionHost via the ConnectionHost class
        /// </summary>        
        public static string ExecuteService(IContext context, string responseFormatAsText, string serviceName, string dataPayload, IConnectionHost connectionHost)
        {
            string response = string.Empty;

            // Contains a string of text returned to the caller in the event of an InternalServerError
            StringBuilder issueList = new StringBuilder();

            try
            {
                // For performance tracking only.
                DateTime startTime = DateTime.Now;

                context.Set(new FileIO());
                context.Set(new PathIO());

                IAPIGuideHost aPIGuideHost = new APIGuideHost();

                context.Set(aPIGuideHost);

                // Load the service to execute
                // We do it this way rather than reusing an instance of binder to ensure the binder is wired up.                
                object serviceInstance = context.Get<IBinding>().LoadViaFullName(context, serviceName, issueList);

                // ajax and JQUERY jsonp support
                #region jsonp

                string p = connectionHost.RequestAny(Commands.HTTP_PADDING_PREFIX);     // JQUERY support

                if (string.IsNullOrWhiteSpace(p))
                {
                    p = connectionHost.RequestAny(Commands.HTTP_PADDING_PREFIX_2);      // ajax support
                }

                #endregion

                if (string.IsNullOrWhiteSpace(dataPayload))
                {
                    // used for $.ajax, JQUERY and GET Search Criteria
                    dataPayload = connectionHost.RequestQueryString(null);
                }

                // Resolve the desired response format.
                ResponseFormat responseFormat = (ResponseFormat)Enum.Parse(typeof(ResponseFormat), responseFormatAsText, true);

                // Assign the HttpContext if needed.
                if (typeof(IConnectionContext).IsAssignableFrom(serviceInstance.GetType()))
                {
                    IConnectionContext cc = (IConnectionContext)serviceInstance;
                    cc.ConnectionHost = connectionHost;
                }

                // Execute the business process
                //if (typeof(IService).IsAssignableFrom(serviceInstance.GetType()))
                //{
                    IService bp = (IService)serviceInstance;

                    string responseText = string.Empty;

                    if (aPIGuideHost.IsAPIGuideRequest(context))
                    {
                        response = aPIGuideHost.ReadAPIGuideResponseText(context, bp, responseFormat);
                    }
                    else
                    {
                        response = bp.Execute(context, dataPayload, responseFormat);
                    }

                    if ((connectionHost != null) && (connectionHost.ResponseSentToOutputStream))
                    {
                        // Tell the caller not to return any response.
                        response = string.Empty; // new HttpResponseMessage(HttpStatusCode.OK);
                    }
                    else if (responseText == Commands.RESPONSE_SENT_TO_STREAM)
                    {
                        response = string.Empty; // new HttpResponseMessage(HttpStatusCode.OK);
                    }
                    else if (string.IsNullOrWhiteSpace(responseText) == false)
                    {
                        StringBuilder responseBuilder = new StringBuilder();

                        // WRITE CALLBACK IF NEEDED
                        #region jsonp
                        //if (string.IsNullOrEmpty(p) == false)
                        //{
                        //    responseBuilder.Append(p + "(");
                        //}
                        #endregion

                        responseBuilder.Append(responseText);

                        //response = CreateHttpResponseType(context);
                        response = responseBuilder.ToString();

                        if (string.IsNullOrEmpty(p))
                        {
                            response = responseBuilder.ToString();
                        }
                        else
                        {
                            // jsonp
                            responseBuilder.Append(");");
                            response = responseBuilder.ToString();
                        }
                    }
                //}
                //else if (typeof(ITest).IsAssignableFrom(serviceInstance.GetType()))
                //{
                 //   ITest bp = (ITest)serviceInstance;
                  //  TestResult testResult = bp.ExecuteTest(context, responseFormat);
                   // string responseText = testResult.ToString();
                    //response = string.Empty; // new HttpResponseMessage(HttpStatusCode.OK);
                   // response = responseText;
                //}
            }
            catch (System.Exception ex)
            {
                // Critial: Each service implementing IBusinessProcess should handle their own interal exception and should NOT throw them up the stack.
                //
                // IF YOU GET HERE
                // 1. Check hostconfig.xml for the appropriate paths with the key BusinessProcessPathList
                //      the assembly you are trying to load must exists within the <Key>BusinessProcessPathList</Key> path list specified
                // 2. SAIL.Framework.Host.csproj                
                // 3. Debug from 'IBinder.LoadViaFullName()'
                // 4. If all else fails try an iisreset

                //response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                response = ex.ToString() + System.Environment.NewLine + issueList.ToString();
            }

            return response;
        }

        private static HttpResponseMessage CreateHttpResponseType(IContext context)
        {
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);

            try
            {
                object httpResponse = context.GetByName(Context.HTTP_STATUS_CODE);

                if (httpResponse != null)
                {
                    HttpStatusCode newStatus;
                    if (Enum.TryParse<HttpStatusCode>(httpResponse.ToString(), out newStatus))
                    {
                        response = new HttpResponseMessage(newStatus);
                    }
                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }

            return response;
        }
    }
}