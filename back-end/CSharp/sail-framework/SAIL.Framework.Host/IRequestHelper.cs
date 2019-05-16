using System;
using System.Text;
using System.Collections.Generic;

using SAIL.Framework.Host.Enums;

namespace SAIL.Framework.Host
{
    public interface IRequestHelper
    {
        T StringToObject<T>(string dataPayload, ResponseFormat responseFormat);
    }
}
