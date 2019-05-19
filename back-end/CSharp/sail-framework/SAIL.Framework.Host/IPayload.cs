using System;
using System.Xml;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SAIL.Framework.Host
{
    public interface IPayload
    {
        // Load
        void LoadXml(string xmlPayload);
        void LoadJson(string jsonPayload);
        void LoadPayload(IPayload payload);
        void LoadXmlDocument(XmlDocument xmlDocument);

        // Read full graph
        string Xml { get; }
        string Json { get; }
        string JsonArray { get; }   // Returns a json array from all childs objects without a root element

        // Properties
        IPayload this[int index] { get; }
        IPayload this[string name] { get; }
        string Val { get; set; }            // InnerText or ToString() of the core entity
        System.Xml.XmlNode XmlNode { get; }

        // Append
        IPayload AppendChildAttribute(string attributeName, string attributeValue);
        IPayload AppendChildElement(string elementName);
        IPayload AppendChildElement(string elementName, dynamic innerText);
        IPayload AppendChildElement(string elementName, dynamic innerText, string namespaceUri);
        IPayload AppendXmlFragment(string xmlFragment);

        // Get
        IPayload GetChildElement(string elementName);
        List<IPayload> GetChildElements(string elementName);
        IPayload ParentElement { get; }
        List<IPayload> ChildElements { get; }
        string LocalName { get; }

        // Where
        IPayload Where(string parentElementNameIs, string childElementNameIs, string innerTextIs);

        // Select
        IPayload SelectChildElement(string xPath);
    }
}
