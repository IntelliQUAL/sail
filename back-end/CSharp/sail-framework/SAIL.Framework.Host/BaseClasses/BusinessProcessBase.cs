using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using SAIL.Framework.Host.Enums;
using SAIL.Framework.Host.Bootstrap;

namespace SAIL.Framework.Host.BaseClasses
{
    /// <summary>
    /// Created:        09/17/2014
    /// Author:         David J. McKee
    /// Purpose:        1. Performs the simple task of turning the XML or JSON request into the appropriate .NET object.
    ///                 2. Performs the simple task of turning the .NET object into the appropriate XML or JSON response. 
    /// Important:      A blank request will return an example request in the correct format.
    /// Awesome:
    ///                 Adding "&e=Request" to the QueryString will return an example request.
    ///                 Adding "&e=Response" to the QueryString will return an example response.
    /// </summary>
    /// <typeparam name="I">Input data type</typeparam>
    /// <typeparam name="O">Output data type</typeparam>
    /// MarshalByRefObject Required to load in another appDomain 
    public abstract class BusinessProcessBase<I, O> : MarshalByRefObject, IBusinessProcess, IConnectionContext, IProtectedMemory, IService
    {
        private const int APP_DOMAIN_REFRESH_MINUTES = 10;

        // Seperate memory space that is refreshed periodically
        private static object _appDomainSyncRoot = new object();
        private static DateTime _lastAppDomainRefreash = DateTime.Now;
        private delegate void refreshAppDomain();

        protected IConnectionHost _connectionHost = null;
        protected string _requestHash = string.Empty;
        protected ResponseFormat _responseFormat = Enums.ResponseFormat.XML;

        public abstract O ProcessRequest(I request);
        public abstract I ExampleRequest { get; }
        public abstract O ExampleResponse { get; }

        protected virtual bool RunInProtectedMemory
        {
            get
            {
                return false;
            }
        }

        protected virtual int ListenerCount
        {
            get
            {
                return 10;
            }
        }

        #region IBusinessProcess Members

        string IBusinessProcess.Execute(string dataPayload, ResponseFormat responseFormat)
        {
            IContext context = CrossCuttingConcerns.BuildDefaultServiceContext(_connectionHost, this);

            return ((IService)this).Execute(context, dataPayload, responseFormat);
        }

        private string PromptInvalidRequest(ResponseFormat responseFormat, string responseString, System.Exception ex)
        {
            // Return an example request for a blank request.
            IContext context = CrossCuttingConcerns.BuildDefaultServiceContext(_connectionHost, this);

            StringBuilder blankRequest = new StringBuilder();
            blankRequest.AppendLine("Invalid Request!");

            blankRequest.AppendLine("Expected Format:");
            blankRequest.AppendLine();
            blankRequest.AppendLine(CrossCuttingConcerns.ResponseHelper(context).ObjectToString(this.ExampleRequest, responseFormat));

            if (ex != null)
            {
                blankRequest.AppendLine();
                blankRequest.AppendLine(ex.ToString());
            }

            responseString = blankRequest.ToString();
            return responseString;
        }

        private string RemoveDataContractNamespace(string responseString)
        {
            return responseString.Replace(" xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\">", ">");
        }

        #endregion

        #region IConnectionContext Members

        IConnectionHost IConnectionContext.ConnectionHost
        {
            get
            {
                return _connectionHost;
            }
            set
            {
                _connectionHost = value;
            }
        }

        bool IConnectionContext.IsAsync
        {
            get { return false; }
        }

        #endregion

        bool IProtectedMemory.RunInProtectedMemory
        {
            get { return this.RunInProtectedMemory; }
        }

        int IProtectedMemory.TimeoutSeconds
        {
            get
            {
                return 90;  // 90 seconds
            }
        }

        int IProtectedMemory.PoleIntervalMilliseconds
        {
            get
            {
                return 100;
            }
        }

        int IProtectedMemory.ListenerCount
        {
            get
            {
                return this.ListenerCount;
            }
        }

        string IProtectedMemory.ProcessArguments(IBusinessProcess businessProcess,
                                                    IConnectionHost connectionHost,
                                                    string listenerGuid)
        {
            string processArguments = string.Empty;

            IContext context = CrossCuttingConcerns.BuildDefaultServiceContext(connectionHost, businessProcess);

            try
            {
                // Return the process arguments
                processArguments = "XPOLastMile.Infrastructure.ESB.Producer \"" + connectionHost.HostUrl + "/xml.ashx?c=XPOLastMile.Framework.HostInterfaces.EsbConfig&bpName=" + businessProcess.GetType() + "&listenerGuid=" + listenerGuid + "\"";
            }
            catch (System.Exception ex)
            {
                CrossCuttingConcerns.ExceptionHandler(context).HandleException(context, ex);
            }

            return processArguments;
        }

        string IService.Execute(IContext context, string dataPayload, ResponseFormat responseFormat)
        {
            _responseFormat = responseFormat;

            string responseString = string.Empty;

            if (string.IsNullOrWhiteSpace(dataPayload) == false)
            {
                try
                {
                    //IMD5 md5 = CrossCuttingConcerns.Md5;

                    //_requestHash = md5.ComputeHash(dataPayload);

                    O responseObject = default(O);

                    I request = (I)CrossCuttingConcerns.RequestHelper(context).StringToObject<I>(dataPayload, responseFormat);

                    if (request != null)
                    {
                        responseObject = ProcessRequest(request);
                        responseString = CrossCuttingConcerns.ResponseHelper(context).ObjectToString(responseObject, responseFormat);
                    }
                    else
                    {
                        responseString = PromptInvalidRequest(responseFormat, responseString, null);
                    }
                }
                catch (System.Exception ex)
                {
                    responseString = PromptInvalidRequest(responseFormat, responseString, ex);
                }
            }
            else
            {
                const string QUERY_STRING_EXAMPLE = "e";
                const string EXAMPLE_REQUEST = "ExampleRequest";
                const string EXAMPLE_RESPONSE = "ExampleResponse";
                const string WSDL = "WSDL";
                const string WSDL_TYPED = "WsdlWithTypes";
                const string XSD = "XSD";

                if (_connectionHost != null)
                {
                    string example = _connectionHost.RequestQueryString(QUERY_STRING_EXAMPLE);

                    if (string.IsNullOrEmpty(example))
                    {
                        I request;

                        //AK 11/28/2014 - Support for passing in no querystring value (for request objects that have no inputs).
                        O responseObject = default(O);

                        if (TypeConversion.IsObjectOnlyQueryString<I>(context, out request))
                        {
                            responseObject = ProcessRequest(request);
                            responseString = CrossCuttingConcerns.ResponseHelper(context).ObjectToString(responseObject, responseFormat);
                        }
                        else
                        {
                            request = (I)Activator.CreateInstance(typeof(I));

                            responseObject = ProcessRequest(request);
                            responseString = CrossCuttingConcerns.ResponseHelper(context).ObjectToString(responseObject, responseFormat);
                            //responseString = PromptInvalidRequest(responseFormat, responseString, null);
                        }
                    }
                    else
                    {
                        switch (example)
                        {
                            case EXAMPLE_REQUEST:
                                responseString = CrossCuttingConcerns.ResponseHelper(context).ObjectToString(this.ExampleRequest, responseFormat);
                                break;

                            case EXAMPLE_RESPONSE:
                                responseString = CrossCuttingConcerns.ResponseHelper(context).ObjectToString(this.ExampleResponse, responseFormat);
                                break;

                            case WSDL:
                                //responseString = CrossCuttingConcerns.ResponseHelper(context).GenerateWsdl(this, this, this.ExampleRequest, this.ExampleResponse);
                                break;

                            case WSDL_TYPED:
                                //responseString = CrossCuttingConcerns.ResponseHelper(context).GenerateWsdlTypes(this, this, this.ExampleRequest, this.ExampleResponse);
                                break;

                            case XSD:
                                //responseString = CrossCuttingConcerns.ResponseHelper(context).GenerateXsd(this, this, this.ExampleRequest, this.ExampleResponse);
                                break;
                        }
                    }
                }
            }

            return RemoveDataContractNamespace(responseString);
        }

        string IProtectedMemory.ProcessFileName
        {
            get
            {
                string processPath = string.Empty;

                foreach (string path in ConfigHelper.ReadBusinessProcessPathList())
                {
                    processPath = path + "\\XPOLastMile.Agent.exe";

                    if (System.IO.File.Exists(processPath))
                    {
                        break;
                    }
                }

                return processPath;
            }
        }

        ITrace IProtectedMemory.TraceEmit
        {
            get
            {
                IContext context = CrossCuttingConcerns.BuildDefaultServiceContext(_connectionHost, this);
                return CrossCuttingConcerns.Trace(context);
            }
        }

        protected FlowTransport<object> DefaultServiceContext
        {
            get
            {
                return CrossCuttingConcerns.BuildDefaultServiceContext(_connectionHost, this);
            }
        }
    }
}
