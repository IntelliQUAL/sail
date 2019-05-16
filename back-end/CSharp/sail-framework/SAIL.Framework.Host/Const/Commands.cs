using System;
using System.Collections.Generic;
using System.Text;

namespace SAIL.Framework.Host.Consts
{
    public class Commands
    {
        // Stream support
        public const string RESPONSE_SENT_TO_STREAM = "ResponseSentToOutputStream";

        // CORS request via Query String
        public const string QUERY_STRING_CORS_METHOD = "cors-method";   // Cross-Origin Resource Sharing (CORS) "cors-method" query string parameter.

        public const string HTTP_PADDING_PREFIX = "p";          // jsonp support
        public const string HTTP_PADDING_PREFIX_2 = "callback"; // jsonp support
    }
}
