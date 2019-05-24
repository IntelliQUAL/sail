using System;
using System.Web;
using System.Linq;
using System.Collections.Generic;

using Microsoft.AspNetCore.Http;

using SAIL.Framework.Host;
using SAIL.Framework.Host.Consts;


namespace SAIL.Host.API.Helpers
{
    public class ConnectionHost : IConnectionHost
    {
        HttpContext _httpContext = null;

        public ConnectionHost(HttpContext context)
        {
            _httpContext = context;
        }

        void IConnectionHost.ResponseWrite(string text)
        {
            _httpContext.Response.WriteAsync(text);
            //_httpContext.Response.Flush();
        }

        string IConnectionHost.RequestQueryString(string name)
        {
            string value = string.Empty;

            if (_httpContext != null)
            {
                if (_httpContext.Request != null)
                {
                    if (string.IsNullOrWhiteSpace(name))
                    {
                        // This code works but also sucks. There must be a better way to pull the null queryString
                        foreach (string key in _httpContext.Request.Query.Keys)
                        {
                            var queryValue = _httpContext.Request.Query[key];

                            if (queryValue.Count == 1)
                            {
                                var zeroElement = queryValue[0];

                                if (string.IsNullOrWhiteSpace(zeroElement))
                                {
                                    const int ARBITRARILY_LONG_NAME_LEN = 50;

                                    if (key.Length > ARBITRARILY_LONG_NAME_LEN)
                                    {
                                        value = HttpUtility.UrlDecode(key);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else
                    { 
                        if (string.IsNullOrWhiteSpace(_httpContext.Request.Query[name]) == false)
                        {
                            value = _httpContext.Request.Query[name].ToString();
                        }
                    }
                }
            }

            return value;
        }

        string IConnectionHost.RequestUrlAuthority
        {
            get
            {
                //return _httpContext.Request.Url.Authority;
                return string.Empty;
            }
        }

        string IConnectionHost.PostBackUrl
        {
            get 
            { 
                // return _httpContext.Request.Url.ToString();
                return string.Empty;
            }
        }

        /// <summary>
        /// Created:        4/14/2013
        /// Author:         David J. McKee
        /// Purpose:        Reads any Form or QueryString value.
        ///                 Note: QueryString values take precedence over form values.
        /// </summary>        
        /// <returns></returns>
        string IConnectionHost.RequestAny(string name)
        {
            string value = string.Empty;

            value = ((IConnectionHost)this).RequestQueryString(name);

            if (string.IsNullOrEmpty(value))
            {
                value = ((IConnectionHost)this).RequestForm(name);
            }

            if (string.IsNullOrEmpty(value))
            {
                Dictionary<string, string> headers = ((IConnectionHost)this).Headers;

                if (headers != null)
                {
                    if (headers.ContainsKey(name))
                    {
                        value = headers[name];
                    }
                }
            }

            return value;
        }

        string IConnectionHost.RequestForm(string name)
        {
            string formValue = null;

            /*
            if (_httpContext != null)
            {
                if (_httpContext.Request.Form != null)
                {
                    if (_httpContext.Request.Form.ContainsKey(name))
                    {
                        formValue = _httpContext.Request.Form[name];
                    }
                }
            }
            */

            return formValue;
        }

        void IConnectionHost.AddHeader(string name, string value)
        { 
            _httpContext.Response.Headers.Add(new KeyValuePair<string, Microsoft.Extensions.Primitives.StringValues>(name, value));
        }

        bool IConnectionHost.IsClientConnected
        {
            //get { return _httpContext.Response.IsClientConnected; }
            get
            {
                return true;
            }
        }

        string IConnectionHost.TransmitFile(string filename)
        {
            if (System.IO.File.Exists(filename))
            {
                Microsoft.Extensions.FileProviders.PhysicalFileProvider file = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(filename);
                _httpContext.Response.SendFileAsync(file.GetFileInfo(filename));
            }

            return Commands.RESPONSE_SENT_TO_STREAM;
        }

        string IConnectionHost.ReadSecureSessionCookie(string name, string defaultValue)
        {
            string value = defaultValue;

            try
            {
                name = name.Replace("=", string.Empty);

                if (_httpContext.Request.Cookies.Keys.Contains<string>(name))
                {
                    //string encryptedValue = _httpContext.Request.Cookies[name].Value;
                    string encryptedValue = string.Empty;

                    if (string.IsNullOrWhiteSpace(encryptedValue) == false)
                    {
                        //value = CryptoHelper.DecryptString(encryptedValue);
                    }
                }
            }
            catch { }

            return value;
        }

        void IConnectionHost.WriteSecureSessionCookie(string name, string value)
        {
            name = name.Replace("=", string.Empty);

            //string encryptedValue = CryptoHelper.EncryptString(value);

            //_httpContext.Response.Cookies[name].Value = encryptedValue;
            //_httpContext.Response.Cookies[name].HttpOnly = true;
        }

        string IConnectionHost.HostUrl
        {
            get
            {
                // Assumption: this will never end in a slash
                /*
                string hostUrl = _httpContext.Request.Url.Scheme + Uri.SchemeDelimiter + ((IConnectionHost)this).RequestUrlAuthority +
                            _httpContext.Request.ApplicationPath;

                if (hostUrl.EndsWith("/"))
                {
                    hostUrl = hostUrl.Substring(0, hostUrl.Length - 1);
                }

                return hostUrl;
                */
                return string.Empty;               
            }
        }

        string IConnectionHost.PageUrl
        {
            get
            {
                //return _httpContext.Request.Url.Scheme + Uri.SchemeDelimiter + ((IConnectionHost)this).RequestUrlAuthority + _httpContext.Request.Url.AbsolutePath;
                return string.Empty;
            }
        }

        /*
        List<SAIL.Framework.Host.IFile> IConnectionHost.RequestFiles
        {
            get
            {
                List<IFile> requestFiles = new List<IFile>();

                if ((_httpContext.Request.Files != null) &&
                    (_httpContext.Request.Files.Count > 0))
                {
                    foreach (string formFieldName in _httpContext.Request.Files.AllKeys)
                    {
                        HttpPostedFileBase file = _httpContext.Request.Files[formFieldName];

                        FileUploadInfo fileToUpload = new FileUploadInfo(file, formFieldName);

                        requestFiles.Add(fileToUpload);
                    }
                }

                return requestFiles;
            }
        }
        */

        // Critical: The http method can be overridden by the "Access-Control-Request-Method" or by the
        //  Cross-Origin Resource Sharing (CORS) "cors-method" query string parameter.
        string IConnectionHost.HttpMethod
        {
            get
            {
                string httpMethod = "GET";

                if (_httpContext != null)
                {
                    //httpMethod = _httpContext.Request.HttpMethod;

                    // This is to allow compatability with jsonp wihtout using 'Access-Control-Request-Method'.
                    string corsMethod = ((IConnectionHost)this).RequestQueryString(Commands.QUERY_STRING_CORS_METHOD);

                    if (string.IsNullOrWhiteSpace(corsMethod) == false)
                    {
                        corsMethod = corsMethod.Replace("/", string.Empty);
                        httpMethod = corsMethod.Trim().ToUpper();
                    }
                }

                //if (_owinContext != null)
                //{
                //    httpMethod = _owinContext.Request.Method;
                //}

                return httpMethod;
            }
        }

        string IConnectionHost.ContentType
        {
            get { return _httpContext.Request.ContentType; }
        }

        Dictionary<string, string> IConnectionHost.Headers
        {
            get
            {
                Dictionary<string, string> headers = new Dictionary<string, string>();

                if (_httpContext != null)
                {
                    //headers = new NameValueCollectionToDictionary(_httpContext.Request.Headers).Dictionary;
                }

                //if (_owinContext != null)
                //{
                //    foreach (string key in _owinContext.Request.Headers.Keys)
                //    {
                //        headers[key] = _owinContext.Request.Headers[key];
                //    }
                //}

                return headers;
            }
        }

        Dictionary<string, string> IConnectionHost.Form
        {
            get
            {
                //return new NameValueCollectionToDictionary(_httpContext.Request.Form).Dictionary;
                return new Dictionary<string, string>();
            }
        }

        System.IO.Stream IConnectionHost.ResponseOutputStream
        {
            get { return _httpContext.Response.Body; }
        }

        string IConnectionHost.WebServerRootUrl
        {
            get
            {
                // Assumption: this will never end in a slash
                //string hostUrl = _httpContext.Request.Url.Scheme + Uri.SchemeDelimiter + ((IConnectionHost)this).RequestUrlAuthority;
                string hostUrl = string.Empty;

                if (hostUrl.EndsWith("/"))
                {
                    hostUrl = hostUrl.Substring(0, hostUrl.Length - 1);
                }

                return hostUrl;
            }
        }


        string IConnectionHost.QueryStringOnly
        {
            get
            {
                //string queryString = _httpContext.Request.Url.ToString();
                string queryString = string.Empty;

                int questionLoc = queryString.IndexOf("?");

                if (questionLoc > 0)
                {
                    queryString = queryString.Substring(questionLoc + 1);
                }
                else
                {
                    queryString = string.Empty;
                }

                return queryString;
            }
        }


        string IConnectionHost.ApplicationPath
        {
            get 
            {
                //return _httpContext.Request.ApplicationPath; 
                return string.Empty;
            }
        }

        string IConnectionHost.PhysicalApplicationPath
        {
            get 
            { 
                //return _httpContext.Request.PhysicalApplicationPath;
                return string.Empty;
            }
        }

        // This should always return ipv4
        string IConnectionHost.RequestUserIpAddress
        {
            get
            {
                string ipAddress = string.Empty;

                try
                {
                    //ipAddress = _httpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                    if (ipAddress == null)
                    {
                   //     ipAddress = _httpContext.Request.ServerVariables["REMOTE_ADDR"];
                    }
                }
                catch { }

                if (string.IsNullOrWhiteSpace(ipAddress))
                {
                   // ipAddress = _httpContext.Request.UserHostAddress;
                }

                if (ipAddress == "::1")
                {
                    ipAddress = "127.0.0.1";
                }

                return ipAddress;
            }
        }


        System.IO.Stream IConnectionHost.RequestInputStream
        {
            get
            {
                //return _httpContext.Request.InputStream;
                return null;
            }
        }

        /*
        IPrincipalSecurityContext IConnectionHost.CurrentPrincipal
        {
            get
            {
                if (_principalSecurityContext == null)
                {
                    _principalSecurityContext = new PrincipalToPrincipalSecurityContext(_httpContext.User).PrincipalSecurityContext;
                }

                return _principalSecurityContext;
            }
            set
            {
                _principalSecurityContext = value;
            }
        }
        */

        void IConnectionHost.BinaryWrite(byte[] data)
        {
           // _httpContext.Response.BinaryWrite(data);
        }

        void IConnectionHost.ResponseClear()
        {
            _httpContext.Response.Clear();
        }


        Dictionary<string, string> IConnectionHost.QueryString
        {
            get
            {
                Dictionary<string, string> result = new Dictionary<string, string>();

                if ((_httpContext.Request != null) &&
                    (_httpContext.Request.QueryString != null))
                {
                    /*
                    foreach (string key in _httpContext.Request.QueryString.AllKeys)
                    {
                        if (string.IsNullOrWhiteSpace(key) == false)
                        {
                            result.Add(key, _httpContext.Request.QueryString[key]);
                        }
                    }
                    */
                }

                return result;
            }
        }

        private bool _responseSentToOutputStream = false;


        bool IConnectionHost.ResponseSentToOutputStream
        {
            get
            {
                return _responseSentToOutputStream;
            }
            set
            {
                _responseSentToOutputStream = value;
            }
        }

        /*
        IPrincipalSecurityContext IConnectionHost.ExecutingPrincipal
        {
            get
            {
                System.Security.Principal.IIdentity processIdentity = System.Security.Principal.WindowsIdentity.GetCurrent(false);

                System.Security.Principal.GenericPrincipal executingPrincipal =
                    new System.Security.Principal.GenericPrincipal(processIdentity, null);

                return new PrincipalToPrincipalSecurityContext(executingPrincipal).PrincipalSecurityContext;
            }
        }
        */

        Dictionary<string, string> IConnectionHost.Cookies
        {
            get
            {
                return new Dictionary<string, string>();
            }
        }

        List<IFile> IConnectionHost.RequestFiles => throw new NotImplementedException();
    }
}