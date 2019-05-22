using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using SAIL.Framework.Host.Consts;
using SAIL.Framework.Host.Bootstrap;

namespace SAIL.Framework.Host.BaseClasses
{
    /// <summary>
    /// Created:        09/21/2013
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
    public abstract class RepositoryBusinessProcessBase<CreateI, CreateO, ReadO, UpdateI, UpdateO, DeleteO, SearchI, SearchO> : MarshalByRefObject, IBusinessProcess, IConnectionContext, IProtectedMemory
    {
        private const string EXAMPLE_REQUEST = "ExampleRequest";
        private const int APP_DOMAIN_REFRESH_MINUTES = 10;

        // Seperate memory space that is refreshed periodically
        private static object _appDomainSyncRoot = new object();
        private static DateTime _lastAppDomainRefreash = DateTime.Now;
        private delegate void refreshAppDomain();

        protected IConnectionHost _connectionHost = null;
        protected string _requestHash = string.Empty;
        protected Enums.ResponseFormat _responseFormat = Enums.ResponseFormat.XML;

        public abstract CreateI ProcessNewRequest(IContext context);

        public abstract CreateO ProcessCreateRequest(IContext context, CreateI request);
        public abstract ReadO ProcessReadRequest(IContext context, string id);
        public abstract UpdateO ProcessUpdateRequest(IContext context, UpdateI request);
        public abstract DeleteO ProcessDeleteRequest(IContext context, string id);
        public abstract SearchO ProcessSearchAllRequest(IContext context);

        public abstract SearchO ProcessSearchRequest(IContext context, SearchI request);

        public abstract CreateI ExampleCreateRequest { get; }
        public abstract UpdateI ExampleUpdateRequest { get; }

        public abstract CreateO ExampleCreateResponse { get; }
        public abstract ReadO ExampleReadResponse { get; }
        public abstract UpdateO ExampleUpdateResponse { get; }
        public abstract DeleteO ExampleDeleteResponse { get; }

        public abstract SearchI ExampleSearchRequest { get; }

        public abstract SearchO ExampleSearchAllResponse { get; }

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

        protected FlowTransport<object> DefaultServiceContext
        {
            get
            {
                return CrossCuttingConcerns.BuildDefaultServiceContext(_connectionHost, this);
            }
        }

        string IBusinessProcess.Execute(string dataPayload, Enums.ResponseFormat responseFormat)
        {
            _responseFormat = responseFormat;

            string responseString = string.Empty;
            object responseObject = null;

            IContext context = CrossCuttingConcerns.BuildDefaultServiceContext(_connectionHost, this);

            context.Set(responseFormat);

            if (string.IsNullOrWhiteSpace(dataPayload) == false)
            {
                try
                {
                    _requestHash = CrossCuttingConcerns.Md5.ComputeHash(dataPayload);

                    object request = null;

                    // Critical: The http method can be overridden by the "Access-Control-Request-Method" or by the
                    //  Cross-Origin Resource Sharing (CORS) "cors-method" query string parameter.                    

                    switch (_connectionHost.HttpMethod)
                    {
                        case "POST":    // Create
                            responseObject = default(CreateO);
                            request = (CreateI)CrossCuttingConcerns.RequestHelper(context).StringToObject<CreateI>(dataPayload, responseFormat);
                            responseObject = ProcessCreateRequest(context, (CreateI)request);
                            break;

                        case "GET":     // Read                                                        
                            FlowTransport<string> readContext = new FlowTransport<string>(dataPayload, CrossCuttingConcerns.BuildDefaultServiceContext(_connectionHost, this));

                            if (dataPayload == "new")
                            {
                                request = dataPayload;
                                responseObject = ProcessNewRequest(context);
                            }
                            else if (IsSearch(dataPayload))
                            {
                                request = (SearchI)CrossCuttingConcerns.RequestHelper(context).StringToObject<SearchI>(dataPayload, responseFormat);
                                responseObject = default(SearchO);
                                responseObject = ProcessSearchRequest(context, (SearchI)request);
                            }
                            else
                            {
                                request = dataPayload;
                                responseObject = default(ReadO);
                                responseObject = ProcessReadRequest(context, dataPayload);
                            }
                            break;

                        case "PUT":     // Update
                            responseObject = default(UpdateO);
                            request = (UpdateI)CrossCuttingConcerns.RequestHelper(context).StringToObject<UpdateI>(dataPayload, responseFormat);
                            responseObject = ProcessUpdateRequest(context, (UpdateI)request);
                            break;

                        case "DELETE":  // Delete
                            responseObject = default(DeleteO);
                            request = dataPayload;
                            responseObject = ProcessDeleteRequest(context, dataPayload);
                            break;
                    }

                    if (request == null)
                    {
                        responseString = PromptInvalidRequest(responseFormat, responseString, null);
                    }
                    else
                    {
                        responseString = CrossCuttingConcerns.ResponseHelper(context).ObjectToString(responseObject, responseFormat);
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

                if (_connectionHost != null)
                {
                    string example = _connectionHost.RequestQueryString(QUERY_STRING_EXAMPLE);

                    if (string.IsNullOrEmpty(example))
                    {
                        if (_connectionHost.HttpMethod == "GET")
                        {
                            // Search All                            
                            responseObject = ProcessSearchAllRequest(context);

                            if (_connectionHost.ResponseSentToOutputStream)
                            {
                                // Tell the caller not to return any response.
                                responseString = Commands.RESPONSE_SENT_TO_STREAM;
                            }
                            else
                            {
                                responseString = CrossCuttingConcerns.ResponseHelper(context).ObjectToString(responseObject, responseFormat);
                            }
                        }
                        else
                        {
                            responseString = PromptInvalidRequest(responseFormat, responseString, null);
                        }
                    }
                    else
                    {
                        responseString = BuildExample(responseFormat, responseString, example);
                    }
                }
            }

            return RemoveDataContractNamespace(responseString);
        }

        /// <summary>
        /// Created:        04/19/2015
        /// Author          David J. McKee
        /// Purpose:        Checks to see if we are dealing with a Search request that contains search criterial.
        /// Important:      In order to determine if this is a search request we only need to determine if it's not a read.
        /// </summary>        
        private bool IsSearch(string dataPayload)
        {
            bool isSearch = false;

            const int MAX_GUID_LENGTH = 100;

            if (dataPayload.Length > MAX_GUID_LENGTH)
            {
                isSearch = true;
            }
            else
            {
                // Test for json
                if (dataPayload.StartsWith("{") && dataPayload.EndsWith("}") && dataPayload.Contains("\"") && dataPayload.Contains(":"))
                {
                    isSearch = true;
                }
                else if (dataPayload.StartsWith("<") && dataPayload.EndsWith(">") && dataPayload.Contains("/>"))
                {
                    // test for XML
                    isSearch = true;
                }
            }

            return isSearch;
        }

        private string BuildExample(Enums.ResponseFormat responseFormat, string responseString, string example)
        {
            const string EXAMPLE_RESPONSE = "ExampleResponse";
            const string WSDL = "WSDL";
            const string WSDL_TYPED = "WsdlWithTypes";
            const string XSD = "XSD";

            IContext context = CrossCuttingConcerns.BuildDefaultServiceContext(_connectionHost, this);

            switch (_connectionHost.HttpMethod)
            {
                case "GET":     // Search All
                    switch (example)
                    {
                        case EXAMPLE_REQUEST:
                            responseString = CrossCuttingConcerns.ResponseHelper(context).ObjectToString(this.ExampleSearchRequest, responseFormat);
                            break;

                        case EXAMPLE_RESPONSE:
                            responseString = CrossCuttingConcerns.ResponseHelper(context).ObjectToString(this.ExampleSearchAllResponse, responseFormat);
                            break;

                        /*
                        case WSDL:
                            responseString = CrossCuttingConcerns.ResponseHelper(context).GenerateWsdl(this, this, this.ExampleSearchRequest, this.ExampleSearchAllResponse);
                            break;

                        case WSDL_TYPED:
                            responseString = CrossCuttingConcerns.ResponseHelper(context).GenerateWsdlTypes(this, this, this.ExampleSearchRequest, this.ExampleSearchAllResponse);
                            break;

                        case XSD:
                            responseString = CrossCuttingConcerns.ResponseHelper(context).GenerateXsd(this, this, this.ExampleSearchRequest, this.ExampleSearchAllResponse);
                            break;
                        */                           
                    }
                    break;

                case "POST":    // Create or Search
                    switch (example)
                    {
                        case EXAMPLE_REQUEST:
                            responseString = CrossCuttingConcerns.ResponseHelper(context).ObjectToString(this.ExampleCreateRequest, responseFormat);
                            break;

                        case EXAMPLE_RESPONSE:
                            responseString = CrossCuttingConcerns.ResponseHelper(context).ObjectToString(this.ExampleCreateResponse, responseFormat);
                            break;

                        /*
                        case WSDL:
                            responseString = CrossCuttingConcerns.ResponseHelper(context).GenerateWsdl(this, this, this.ExampleCreateRequest, this.ExampleCreateResponse);
                            break;

                        case WSDL_TYPED:
                            responseString = CrossCuttingConcerns.ResponseHelper(context).GenerateWsdlTypes(this, this, this.ExampleCreateRequest, this.ExampleCreateResponse);
                            break;

                        case XSD:
                            responseString = CrossCuttingConcerns.ResponseHelper(context).GenerateXsd(this, this, this.ExampleCreateRequest, this.ExampleCreateResponse);
                            break;
                        */                           
                    }
                    break;

                case "PUT":     // Update
                    switch (example)
                    {
                        case EXAMPLE_REQUEST:
                            responseString = CrossCuttingConcerns.ResponseHelper(context).ObjectToString(this.ExampleUpdateRequest, responseFormat);
                            break;

                        case EXAMPLE_RESPONSE:
                            responseString = CrossCuttingConcerns.ResponseHelper(context).ObjectToString(this.ExampleUpdateResponse, responseFormat);
                            break;

                        /*
                        case WSDL:
                            responseString = CrossCuttingConcerns.ResponseHelper(context).GenerateWsdl(this, this, this.ExampleUpdateRequest, this.ExampleUpdateResponse);
                            break;

                        case WSDL_TYPED:
                            responseString = CrossCuttingConcerns.ResponseHelper(context).GenerateWsdlTypes(this, this, this.ExampleUpdateRequest, this.ExampleUpdateResponse);
                            break;

                        case XSD:
                            responseString = CrossCuttingConcerns.ResponseHelper(context).GenerateXsd(this, this, this.ExampleUpdateRequest, this.ExampleUpdateResponse);
                            break;
                        */                           
                    }
                    break;

                case "DELETE": // Delete
                    switch (example)
                    {
                        case EXAMPLE_REQUEST:
                            responseString = CrossCuttingConcerns.ResponseHelper(context).ObjectToString("2334", responseFormat);
                            break;

                        case EXAMPLE_RESPONSE:
                            responseString = CrossCuttingConcerns.ResponseHelper(context).ObjectToString(this.ExampleDeleteResponse, responseFormat);
                            break;

                        /*
                        case WSDL:
                            responseString = CrossCuttingConcerns.ResponseHelper(context).GenerateWsdl(this, this, "2334", this.ExampleDeleteResponse);
                            break;

                        case WSDL_TYPED:
                            responseString = CrossCuttingConcerns.ResponseHelper(context).GenerateWsdlTypes(this, this, "2334", this.ExampleDeleteResponse);
                            break;

                        case XSD:
                            responseString = CrossCuttingConcerns.ResponseHelper(context).GenerateXsd(this, this, "2334", this.ExampleDeleteResponse);
                            break;
                        */                           
                    }
                    break;
            }

            return responseString;
        }

        private string PromptInvalidRequest(Enums.ResponseFormat responseFormat, string responseString, System.Exception ex)
        {
            // Return an example request for a blank request.

            StringBuilder blankRequest = new StringBuilder();
            blankRequest.AppendLine("Invalid Request!");

            blankRequest.AppendLine("Expected Format:");
            blankRequest.AppendLine();
            blankRequest.AppendLine(BuildExample(responseFormat, responseString, EXAMPLE_REQUEST));

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

            try
            {
                // Return the process arguments
                processArguments = "XPOLastMile.Infrastructure.ESB.Producer \"" + connectionHost.HostUrl + "/xml.ashx?c=XPOLastMile.Framework.HostInterfaces.EsbConfig&bpName=" + businessProcess.GetType() + "&listenerGuid=" + listenerGuid + "\"";
            }
            catch (System.Exception ex)
            {
                IContext context = CrossCuttingConcerns.BuildDefaultServiceContext(_connectionHost, this);

                CrossCuttingConcerns.ExceptionHandler(context).HandleException(context, ex);
            }

            return processArguments;
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

        public ITrace TraceEmit
        {
            get
            {
                IContext context = CrossCuttingConcerns.BuildDefaultServiceContext(_connectionHost, this);
                return CrossCuttingConcerns.Trace(context);
            }
        }
    }
}
