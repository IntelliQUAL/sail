using System;
using System.Text;
using System.Collections.Generic;

using SAIL.Framework.Host.Enums;

namespace SAIL.Framework.Host
{
    public interface IResponseHelper
    {
        string ObjectToString(object any, ResponseFormat responseFormat);
    }
}
