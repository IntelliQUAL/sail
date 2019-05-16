using System;
using System.Text;
using System.Collections.Generic;

using SAIL.Framework.Host.Enums;

namespace SAIL.Framework.Host.Bootstrap
{
    public static class TypeConversion
    {
        public static string ReadMediaType(ResponseFormat responseFormat)
        {
            string mediaType = string.Empty;

            try
            {
                switch (responseFormat)
                {
                    case ResponseFormat.Binary:
                        break;

                    case ResponseFormat.Boolean:
                        break;

                    case ResponseFormat.CSS:
                        mediaType = "text/css";
                        break;

                    case ResponseFormat.CSV:
                        mediaType = "text/csv";
                        break;

                    case ResponseFormat.Gif:
                        mediaType = "image/gif";
                        break;

                    case ResponseFormat.HTML:
                        mediaType = "text/html";
                        break;

                    case ResponseFormat.JavaScript:
                        mediaType = "application/javascript";
                        break;

                    case ResponseFormat.Jpeg:
                        mediaType = "image/jpeg";
                        break;

                    case ResponseFormat.JSON:
                        mediaType = "application/json";
                        break;

                    case ResponseFormat.Pdf:
                        mediaType = "application/pdf";
                        break;

                    case ResponseFormat.Png:
                        mediaType = "image/png";
                        break;

                    case ResponseFormat.Soap:
                        mediaType = "text/xml";
                        break;

                    case ResponseFormat.XML:
                        mediaType = "text/xml";
                        break;

                    case ResponseFormat.Zip:
                        mediaType = "application/zip";
                        break;

                    case ResponseFormat.Text:
                    case ResponseFormat.Unknown:
                    default:
                        mediaType = "text/plain";
                        break;
                }
            }
            catch
            {
                mediaType = "text/plain";
            }

            return mediaType;
        }

        public static int? ToNullableInt(string theString)
        {
            int i;
            if (Int32.TryParse(theString, out i)) return i;
            return null;
        }

        public static DateTime? ToNullableDateTime(string theString)
        {
            DateTime d;
            if (DateTime.TryParse(theString, out d)) return d;
            return null;
        }

        public static bool? ToNullableBool(string theString)
        {
            bool b;
            if (Boolean.TryParse(theString, out b)) return b;
            return null;
        }

        public static bool IsNumeric(object value)
        {
            bool isNumeric = false;

            try
            {
                if (value == null)
                {
                    isNumeric = false;
                }
                else
                {
                    double result;

                    if (double.TryParse(value.ToString(), out result))
                    {
                        isNumeric = true;
                    }
                }
            }
            catch { }

            return isNumeric;
        }

        /// <summary>
        /// Created:        05/04/2017
        /// Author:         David J. McKee
        /// Purpose:        Attempts to local the request object within the QueryString
        /// </summary>        
        internal static bool IsObjectOnlyQueryString<I>(IContext context, out I request)
        {
            bool objectOnQueryString = false;
            request = default(I);

            try
            {
                IConnectionHost connectionHost = context.Get<IConnectionHost>();
                IRequestHelper requestHelper = context.Get<IRequestHelper>();

                string queryString = connectionHost.QueryStringOnly;

                if (string.IsNullOrWhiteSpace(queryString) == false)
                {
                    request = requestHelper.StringToObject<I>(queryString, Enums.ResponseFormat.QueryString);

                    if (request != null)
                    {
                        objectOnQueryString = true;
                    }
                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }

            return objectOnQueryString;
        }
    }
}
