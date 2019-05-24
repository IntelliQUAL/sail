using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace SAIL.Framework.Host
{
    public interface IConnectionHost
    {
        void ResponseWrite(string text);
        string RequestQueryString(string name);
        string RequestForm(string name);
        string RequestAny(string name);
        string RequestUrlAuthority { get; }
        string PostBackUrl { get; }
        string HostUrl { get; }                                             // host only without page or QueryString
        string WebServerRootUrl { get; }                                    // Root path to web server.
        string PageUrl { get; }                                             // host and page only without QueryStrng
        void AddHeader(string name, string value);
        bool IsClientConnected { get; }
        string TransmitFile(string filename);                               // Important: Response must be returned to caller.
        void WriteSecureSessionCookie(string name, string value);
        string ReadSecureSessionCookie(string name, string defaultValue);
        Dictionary<string, string> Cookies { get; }
        List<IFile> RequestFiles { get; }
        string HttpMethod { get; }                                          // GET or POST
        Dictionary<string, string> Headers { get; }                         // All headers
        Dictionary<string, string> Form { get; }                            // All form values
        Dictionary<string, string> QueryString { get; }                     // All Query String Values
        string ContentType { get; }
        Stream RequestInputStream { get; }
        Stream ResponseOutputStream { get; }
        bool ResponseSentToOutputStream { get; set; }                       // default = false. If true then no data is returned in the response. The assumtion is that it has already been returned via the ResponseOutputStream
        string QueryStringOnly { get; }                                     // QueryStrng only without host or page
        string ApplicationPath { get; }
        string PhysicalApplicationPath { get; }
        string RequestUserIpAddress { get; }                                // Returns the IP address of the requesting user.
        //IPrincipalSecurityContext CurrentPrincipal { get; set; }            // Returns or sets the current Principal (End-User account)
        //IPrincipalSecurityContext ExecutingPrincipal { get; }               // Returns the principal the process is running under or running as (System account)

        void ResponseClear();
        void BinaryWrite(byte[] data);
    }
}
