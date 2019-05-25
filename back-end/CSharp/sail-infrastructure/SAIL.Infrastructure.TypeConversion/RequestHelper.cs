using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using SAIL.Framework.Host;
using SAIL.Framework.Host.Enums;

using Newtonsoft.Json;

namespace SAIL.Infrastructure.TypeConversion
{
    class RequestHelper : IRequestHelper
    {
        T IRequestHelper.StringToObject<T>(string dataPayload, ResponseFormat responseFormat)
        {            
            dynamic response = string.Empty;

            switch (responseFormat)
            {
                case ResponseFormat.JSON:
                    if (typeof(T) == typeof(string))
                    {
                        response = dataPayload;
                    }
                    else
                    { 
                        response = JsonConvert.DeserializeObject<T>(dataPayload);
                    }
                    break;

            }

            return response;
        }
    }
}
