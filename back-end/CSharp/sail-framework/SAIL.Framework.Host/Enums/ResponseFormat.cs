using System;
using System.Collections.Generic;
using System.Text;

namespace SAIL.Framework.Host.Enums
{
    public enum ResponseFormat
    {
        Unknown = 0,
        XML = 1,
        JSON = 2,
        Binary = 3,         //  base64 string
        Boolean = 4,        //  true.ToString(), false.ToString()
        Jpeg = 5,           //  Jpeg Image
        HTML = 6,
        Text = 7,           // text/plain
        CSV = 8,            // text/csv
        Zip = 9,            // application/zip
        Png = 10,           // image/png
        CSS = 11,           // text/css
        Gif = 12,           // image/gif
        Pdf = 13,           // application/pdf
        JavaScript = 14,    // application/javascript
        Soap = 15,          // text/xml
        QueryString = 16    // We use this to translate an object to a querystring or vice versa
    }
}
