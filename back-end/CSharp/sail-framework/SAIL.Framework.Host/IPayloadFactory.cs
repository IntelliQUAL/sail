using System;
using System.Xml;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using SAIL.Framework.Host.Enums;

namespace SAIL.Framework.Host
{
    public interface IPayloadFactory
    {
        IPayload FromXml(string xml);
        //IPayload FromXmlDocument(XmlDocument xmlDocument);
        //IPayload FromXmlNode(XmlDocument xmlDocument, XmlNode xmlNode);
        IPayload FromXmlFile(string xmlFilePathName);
        IPayload FromJson(string json);
        IPayload FromAny(string text, ResponseFormat responseFormat);       // Loads from any ResponseFormat (XPOLastMile.Framework.HostInterfaces.Enums.ResponseFormat)
        IPayload Default(ResponseFormat responseFormat);                    // Initializes a default payload    
        string ToAny(IPayload payload, ResponseFormat responseFormat);
        string ToAny(List<IPayload> payload, ResponseFormat responseFormat);
    }
}
