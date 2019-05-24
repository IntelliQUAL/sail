using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using Newtonsoft.Json;

using SAIL.Framework.Host;
using SAIL.Framework.Host.Enums;

namespace SAIL.Infrastructure.TypeConversion
{
    public class ResponseHelper : IResponseHelper
    {
        string IResponseHelper.ObjectToString(object any, ResponseFormat responseFormat)
        {
            string response = string.Empty;
            
            switch (responseFormat)
            {
                case ResponseFormat.JSON:
                    if (any is string)
                    {
                        response = any.ToString();
                    }
                    else
                    {
                        response = JsonConvert.SerializeObject(any);
                    }
                    break;

                case ResponseFormat.HTML:
                    response = any.ToString();
                    break;

            }

            return response;
        }
    }
}
