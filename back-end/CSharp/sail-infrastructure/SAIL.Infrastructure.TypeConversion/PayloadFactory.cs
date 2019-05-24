using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using SAIL.Framework.Host;
using SAIL.Framework.Host.Enums;
using SAIL.Framework.Host.Bootstrap;
using System.Xml;

namespace SAIL.Infrastructure.TypeConversion
{
    public class PayloadFactory : IPayloadFactory
    {
        //XmlPayload _xmlHelper = null;
        JsonPayload _jsonHelper = null;

        #region IPayloadFactory Members

        /*
        IPayload IPayloadFactory.FromXml(string xml)
        {
            _xmlHelper = new XmlPayload(xml);

            return _xmlHelper;
        }
        */

        /*
        IPayload IPayloadFactory.FromXmlDocument(System.Xml.XmlDocument xmlDocument)
        {
            _xmlHelper = new XmlPayload(xmlDocument.DocumentElement);

            return _xmlHelper;
        }

        IPayload IPayloadFactory.FromXmlNode(System.Xml.XmlDocument xmlDocument, System.Xml.XmlNode xmlNode)
        {
            _xmlHelper = new XmlPayload(xmlNode);

            return _xmlHelper;
        }
        */

        #endregion

        /*

    IPayload IPayloadFactory.FromXmlFile(string xmlFilePathName)
    {
        try
        {
            System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
            xmlDocument.Load(xmlFilePathName);

            _xmlHelper = new XmlPayload(xmlDocument.DocumentElement);
        }
        catch (System.Exception ex)
        {
            IContext context = CrossCuttingConcerns.BuildDefaultServiceContext(null, null, null);

            context.Get<IExceptionHandler>().HandleException(context, ex, "xmlFilePathName: " + xmlFilePathName);
        }

        return _xmlHelper;
    }
    */

        IPayload IPayloadFactory.FromJson(string json)
        {
            _jsonHelper = new JsonPayload(json);

            return _jsonHelper;
        }

        // Important: responseFormat should be XPOLastMile.Framework.HostInterfaces.Enums.ResponseFormat
        IPayload IPayloadFactory.FromAny(string text, ResponseFormat responseFormat)
        {
            IPayload payload = null;
                        
            switch (responseFormat)
            {
                case ResponseFormat.XML:
                    if (string.IsNullOrWhiteSpace(text))
                    {
                        text = "</root>";
                    }

                    payload = ((IPayloadFactory)this).FromXml(text);
                    break;

                case ResponseFormat.JSON:
                    if (string.IsNullOrWhiteSpace(text))
                    {
                        text = "{}";
                    }

                    payload = ((IPayloadFactory)this).FromJson(text);
                    break;
            }

            return payload;
        }
                                                
        string IPayloadFactory.ToAny(IPayload payload, ResponseFormat responseFormat)
        {
            string response = string.Empty;

            switch (responseFormat)
            {
                case ResponseFormat.XML:
                    response = payload.Xml;
                    break;

                case ResponseFormat.JSON:
                    response = payload.Json;
                    break;
            }

            return response;
        }

        string IPayloadFactory.ToAny(List<IPayload> payloadList, ResponseFormat responseFormat)
        {
            string response = string.Empty;

            switch (responseFormat)
            {
                case ResponseFormat.JSON:
                    StringBuilder workingJsonString = new StringBuilder();

                    workingJsonString.Append("[");

                    for (int payloadIndex = 0; payloadIndex < payloadList.Count; payloadIndex++)
                    {
                        string json = payloadList[payloadIndex].Json;

                        workingJsonString.Append(json);

                        if (!(payloadIndex == payloadList.Count - 1))
                        {
                            workingJsonString.AppendLine(",");
                        }
                    }

                    workingJsonString.AppendLine("]");

                    response = workingJsonString.ToString();
                    break;

                case ResponseFormat.XML:
                    IPayload working = ((IPayloadFactory)this).Default(responseFormat);

                    foreach (IPayload payload in payloadList)
                    {
                        working.AppendXmlFragment(payload.Xml);
                    }

                    response = working.Xml;
                    break;

            }

            return response;
        }

        IPayload IPayloadFactory.Default(ResponseFormat responseFormat)
        {
            IPayload payload = null; 

            switch (responseFormat)
            {
                case ResponseFormat.XML:                    
                    payload = ((IPayloadFactory)this).FromXml("</root>");
                    break;

                case ResponseFormat.JSON:                    
                    payload = ((IPayloadFactory)this).FromJson("{}");
                    break;
            }

            return payload;
        }

        IPayload IPayloadFactory.FromXml(string xml)
        {
            throw new NotImplementedException();
        }

        IPayload IPayloadFactory.FromXmlFile(string xmlFilePathName)
        {
            throw new NotImplementedException();
        }
    }
}
