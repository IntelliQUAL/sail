using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using SAIL.Framework.Host.Enums;
using SAIL.Framework.Host.Consts;
using SAIL.Framework.Host.Bootstrap;

namespace SAIL.Framework.Host.BaseClasses
{
    public abstract class RESTBase<CreateI, CreateO, ReadO, UpdateI, UpdateO, DeleteO, SearchI, SearchO> : IService, IConnectionContext
    {
        protected Enums.ResponseFormat _responseFormat = Enums.ResponseFormat.JSON;
        protected IConnectionHost _connectionHost = null;

        private const string EXAMPLE_REQUEST = "ExampleRequest";

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

        // Returns an empty entity
        public abstract CreateI ProcessNewRequest(IContext context);
        // Creates a new entity
        public abstract CreateO ProcessCreateRequest(IContext context, CreateI request);
        // Reads an existing entity
        public abstract ReadO ProcessReadRequest(IContext context, string id);
        // Updates and existing entity
        public abstract UpdateO ProcessUpdateRequest(IContext context, UpdateI request);
        // Deletes and existing entity
        public abstract DeleteO ProcessDeleteRequest(IContext context, string id);
        // returns all existing entities (usually with paging)
        public abstract SearchO ProcessSearchAllRequest(IContext context);
        // searches for an existing entity with search criteria
        public abstract SearchO ProcessSearchRequest(IContext context, SearchI request);

        // Implement all REST rules
        string IService.Execute(IContext context, string dataPayload, ResponseFormat responseFormat)
        {
            _responseFormat = responseFormat;

            string responseString = string.Empty;
            object responseObject = null;

            context.Set(responseFormat);

            if (string.IsNullOrWhiteSpace(dataPayload) == false)
            {
                try
                {
                    // Used for caching
                    //_requestHash = CrossCuttingConcerns.Md5.ComputeHash(dataPayload);

                    object request = null;

                    // Critical: The http method can be overridden by the "Access-Control-Request-Method" or by the
                    //  Cross-Origin Resource Sharing (CORS) "cors-method" query string parameter.                    

                    switch (_connectionHost.HttpMethod)
                    {
                        case "POST":    // Create
                            responseObject = default(CreateO);
                            IRequestHelper requestHelper = context.Get<IRequestHelper>();
                            request = requestHelper.StringToObject<CreateI>(dataPayload, responseFormat);
                            responseObject = ProcessCreateRequest(context, (CreateI)request);
                            break;

                        case "GET":     // Read                                                        
                            FlowTransport<string> readContext = new FlowTransport<string>(dataPayload, context);

                            if (dataPayload == "new")
                            {
                                request = dataPayload;
                                responseObject = ProcessNewRequest(context);
                            }
                            else if (IsSearch(dataPayload))
                            {
                                request = context.Get<IRequestHelper>().StringToObject<SearchI>(dataPayload, responseFormat);
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
                            request = context.Get<IRequestHelper>().StringToObject<UpdateI>(dataPayload, responseFormat);
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
                        responseString = context.Get<IResponseHelper>().ObjectToString(responseObject, responseFormat);
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
                                responseString = context.Get<IResponseHelper>().ObjectToString(responseObject, responseFormat);
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
        /// Created:        06/08/2017
        /// Author:         David J. McKee
        /// Purpose:        Returns an invalid request respose
        /// TODO:           Should be response format specific.
        /// </summary>
        /// <param name="responseFormat"></param>
        /// <param name="responseString"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
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

        private string BuildExample(Enums.ResponseFormat responseFormat, string responseString, string example)
        {
            //const string EXAMPLE_RESPONSE = "ExampleResponse";
            //const string WSDL = "WSDL";
            //const string WSDL_TYPED = "WsdlWithTypes";
            //const string XSD = "XSD";

            switch (_connectionHost.HttpMethod)
            {
                case "GET":     // Search All
                    //switch (example)
                    //{
                        //case EXAMPLE_REQUEST:
                        //    responseString = CrossCuttingConcerns.ResponseHelper.ObjectToString(this.ExampleSearchRequest, responseFormat);
                        //    break;

                        //case EXAMPLE_RESPONSE:
                        //    responseString = CrossCuttingConcerns.ResponseHelper.ObjectToString(this.ExampleSearchAllResponse, responseFormat);
                        //    break;

                        //case WSDL:
                        //    responseString = CrossCuttingConcerns.ResponseHelper.GenerateWsdl(this, this, this.ExampleSearchRequest, this.ExampleSearchAllResponse);
                        //    break;

                        //case WSDL_TYPED:
                        //    responseString = CrossCuttingConcerns.ResponseHelper.GenerateWsdlTypes(this, this, this.ExampleSearchRequest, this.ExampleSearchAllResponse);
                        //    break;

                        //case XSD:
                        //    responseString = CrossCuttingConcerns.ResponseHelper.GenerateXsd(this, this, this.ExampleSearchRequest, this.ExampleSearchAllResponse);
                        //    break;
                    //}
                    break;

                case "POST":    // Create or Search
                    //switch (example)
                    //{
                        //case EXAMPLE_REQUEST:
                        //    responseString = CrossCuttingConcerns.ResponseHelper.ObjectToString(this.ExampleCreateRequest, responseFormat);
                        //    break;

                        //case EXAMPLE_RESPONSE:
                        //    responseString = CrossCuttingConcerns.ResponseHelper.ObjectToString(this.ExampleCreateResponse, responseFormat);
                        //    break;

                        //case WSDL:
                        //    responseString = CrossCuttingConcerns.ResponseHelper.GenerateWsdl(this, this, this.ExampleCreateRequest, this.ExampleCreateResponse);
                        //    break;

                        //case WSDL_TYPED:
                        //    responseString = CrossCuttingConcerns.ResponseHelper.GenerateWsdlTypes(this, this, this.ExampleCreateRequest, this.ExampleCreateResponse);
                        //    break;

                        //case XSD:
                        //    responseString = CrossCuttingConcerns.ResponseHelper.GenerateXsd(this, this, this.ExampleCreateRequest, this.ExampleCreateResponse);
                        //    break;
                    //}
                    break;

                case "PUT":     // Update
                    //switch (example)
                    //{
                        //case EXAMPLE_REQUEST:
                        //    responseString = CrossCuttingConcerns.ResponseHelper.ObjectToString(this.ExampleUpdateRequest, responseFormat);
                        //    break;

                        //case EXAMPLE_RESPONSE:
                        //    responseString = CrossCuttingConcerns.ResponseHelper.ObjectToString(this.ExampleUpdateResponse, responseFormat);
                        //    break;

                        //case WSDL:
                        //    responseString = CrossCuttingConcerns.ResponseHelper.GenerateWsdl(this, this, this.ExampleUpdateRequest, this.ExampleUpdateResponse);
                        //    break;

                        //case WSDL_TYPED:
                        //    responseString = CrossCuttingConcerns.ResponseHelper.GenerateWsdlTypes(this, this, this.ExampleUpdateRequest, this.ExampleUpdateResponse);
                        //    break;

                        //case XSD:
                        //    responseString = CrossCuttingConcerns.ResponseHelper.GenerateXsd(this, this, this.ExampleUpdateRequest, this.ExampleUpdateResponse);
                        //    break;
                    //}
                    break;

                case "DELETE": // Delete
                    //switch (example)
                    //{
                        //case EXAMPLE_REQUEST:
                        //    responseString = CrossCuttingConcerns.ResponseHelper.ObjectToString("2334", responseFormat);
                        //    break;

                        //case EXAMPLE_RESPONSE:
                        //    responseString = CrossCuttingConcerns.ResponseHelper.ObjectToString(this.ExampleDeleteResponse, responseFormat);
                        //    break;

                        //case WSDL:
                        //    responseString = CrossCuttingConcerns.ResponseHelper.GenerateWsdl(this, this, "2334", this.ExampleDeleteResponse);
                        //    break;

                        //case WSDL_TYPED:
                        //    responseString = CrossCuttingConcerns.ResponseHelper.GenerateWsdlTypes(this, this, "2334", this.ExampleDeleteResponse);
                        //    break;

                        //case XSD:
                        //    responseString = CrossCuttingConcerns.ResponseHelper.GenerateXsd(this, this, "2334", this.ExampleDeleteResponse);
                        //    break;
                    //}
                    break;
            }

            return responseString;
        }

        private string RemoveDataContractNamespace(string responseString)
        {
            return responseString.Replace(" xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\">", ">");
        }

        /// <summary>
        /// Created:        06/07/2017
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
    }
}
