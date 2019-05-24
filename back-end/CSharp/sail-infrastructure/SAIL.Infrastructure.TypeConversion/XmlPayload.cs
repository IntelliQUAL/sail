using System;
using System.Xml;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using SAIL.Framework.Host;

namespace SAIL.Infrastructure.TypeConversion
{
    public class XmlPayload : IPayload
    {
        private XmlNode _node = null;

        public XmlPayload(string xml)
        {
            LoadXml(xml);
        }

        public XmlPayload(XmlNode xmlNode)
        {
            _node = xmlNode;
        }

        public void LoadXml(string xmlPayload)
        {
            XmlDocument xmlDocument = new XmlDocument();

            xmlDocument.LoadXml(xmlPayload);

            _node = xmlDocument.DocumentElement;
        }

        void IPayload.LoadJson(string jsonPayload)
        {
            throw new NotImplementedException();
        }

        string IPayload.Xml
        {
            get { return _node.OuterXml; }
        }

        IPayload IPayload.this[int index]
        {
            get { throw new NotImplementedException(); }
        }

        IPayload IPayload.this[string name]
        {
            get { return ((IPayload)this).GetChildElement(name); }
        }

        string IPayload.Val
        {
            get
            {
                return _node.InnerText;
            }
            set
            {
                _node.InnerText = value;
            }
        }

        /*
        System.Xml.XmlNode IPayload.XmlNode
        {
            get
            {
                return _node;
            }
        }
        */


        public List<IPayload> ChildElements { get; private set; }

        IPayload IPayload.Where(string parentElementNameIs, string childElementNameIs, string innerTextIs)
        {
            IPayload result = null;

            if (_node != null)
            {
                foreach (XmlNode parentNode in _node.ChildNodes)
                {
                    if (parentNode.LocalName == parentElementNameIs)
                    {
                        foreach (XmlNode childNode in parentNode.ChildNodes)
                        {
                            if ((childNode.LocalName == childElementNameIs) &&
                                (childNode.InnerText == innerTextIs))
                            {
                                result = new XmlPayload(childNode.ParentNode);
                                break;
                            }
                        }
                    }

                    if (result != null)
                    {
                        break;
                    }
                }
            }

            return result;
        }

        void IPayload.LoadXml(string xmlPayload)
        {
            throw new NotImplementedException();
        }

        IPayload IPayload.AppendChildAttribute(string attributeName, string attributeValue)
        {
            XmlAttribute xmlAttribute = null;

            if (_node.Attributes[attributeName] == null)
            {
                xmlAttribute = _node.OwnerDocument.CreateAttribute(attributeName);
                xmlAttribute.Value = attributeValue;

                _node.Attributes.Append(xmlAttribute);
            }
            else
            {
                xmlAttribute = _node.Attributes[attributeName];
                _node.Attributes[attributeName].Value = attributeValue;
            }

            return new XmlPayload(xmlAttribute);
        }

        IPayload IPayload.AppendChildElement(string elementName)
        {
            IPayload result = null;

            try
            {
                elementName = elementName.Trim();

                if (string.IsNullOrEmpty(elementName) == false)
                {
                    XmlElement newElement = _node.OwnerDocument.CreateElement(elementName);

                    result = new XmlPayload(_node.AppendChild(newElement));
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        IPayload IPayload.AppendChildElement(string elementName, dynamic innerText)
        {
            IPayload newElement = null;

            if (innerText != null)
            {
                newElement = ((IPayload)this).AppendChildElement(elementName);

                newElement.Val = innerText.ToString();
            }

            return newElement;
        }

        IPayload IPayload.AppendChildElement(string elementName, dynamic innerText, string namespaceUri)
        {
            IPayload result = null;

            try
            {
                elementName = elementName.Trim();

                if (string.IsNullOrEmpty(elementName) == false)
                {
                    XmlElement newElement = _node.OwnerDocument.CreateElement(elementName, namespaceUri);

                    result = new XmlPayload(_node.AppendChild(newElement));

                    result.Val = innerText;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        private static XmlNode AppendXmlDocumentFragment(XmlNode appendTo,
                                                         string xml)
        {
            XmlNode newNode = null;

            System.Xml.XmlDocumentFragment xmlDocumentFragment = appendTo.OwnerDocument.CreateDocumentFragment();

            xml = RemoveDocType(xml);

            // Remove <?xml version="1.0" encoding="UTF-8" ?>  or related prefix.
            const string PREFIX_IDENTIFIER = "?>";

            int prefixLoc = xml.IndexOf(PREFIX_IDENTIFIER);

            if (prefixLoc > 0)
            {
                xml = xml.Substring(prefixLoc + PREFIX_IDENTIFIER.Length);
            }

            xmlDocumentFragment.InnerXml = xml;

            System.Xml.XmlNode xmlNode = (System.Xml.XmlNode)xmlDocumentFragment;

            newNode = appendTo.AppendChild(xmlNode);

            return newNode;
        }

        IPayload IPayload.AppendXmlFragment(string xml)
        {
            XmlNode xmlNode = AppendXmlDocumentFragment(_node, xml);

            return new XmlPayload(xmlNode);
        }

        private static string RemoveDocType(string xml)
        {
            int docTypeLoc = xml.IndexOf("<!DOCTYPE");

            if (docTypeLoc > 0)
            {
                // dtd value must be removed before appending xml fragment
                string leftOfDocType = xml.Substring(0, docTypeLoc);
                xml = xml.Substring(docTypeLoc);

                int endTagLoc = xml.IndexOf(">");
                xml = xml.Substring(endTagLoc + 1);

                StringBuilder newXml = new StringBuilder();

                newXml.Append(leftOfDocType);
                newXml.Append(xml);

                XmlDocument removeDocType = new XmlDocument();
                removeDocType.LoadXml(newXml.ToString());
                xml = removeDocType.DocumentElement.OuterXml;
            }

            return xml;
        }

        /*
        IPayload IPayload.AppendChildElement(string elementName, string innerText)
        {
            IPayload newElement = ((IPayload)this).AppendChildElement(elementName);

            newElement.Val = innerText;

            return newElement;
        }
         * */

        IPayload IPayload.GetChildElement(string elementName)
        {
            IPayload result = null;

            try
            {
                // IMPORTANT: WE USE LOCAL NAME TO GET ARROUND NAMESPACES
                if ((_node != null) &&
                    (_node.ChildNodes != null) &&
                    (_node.ChildNodes.Count > 0))
                {
                    foreach (XmlNode childNode in _node.ChildNodes)
                    {
                        if (childNode.LocalName.ToLower() == elementName.ToLower())
                        {
                            result = new XmlPayload(childNode);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public List<IPayload> GetChildElements(string elementName)
        {
            throw new NotImplementedException();
        }

        IPayload IPayload.ParentElement
        {
            get { return new XmlPayload(_node.ParentNode); }
        }

        #region IPayload Members


        IPayload IPayload.SelectChildElement(string xPath)
        {
            IPayload result = null;

            if (_node != null)
            {
                result = new XmlPayload(_node.SelectSingleNode(xPath));
            }
            else
            {
                result = new XmlPayload("<root />");
            }

            return result;
        }

        #endregion

        #region IPayload Members


        List<IPayload> IPayload.ChildElements
        {
            get
            {
                List<IPayload> list = new List<IPayload>();

                foreach (XmlNode childNode in _node.ChildNodes)
                {
                    IPayload childPayload = new XmlPayload(childNode);

                    list.Add(childPayload);
                }

                return list;
            }
        }

        List<IPayload> IPayload.GetChildElements(string elementName)
        {
            List<IPayload> list = new List<IPayload>();

            foreach (XmlNode childNode in _node.ChildNodes)
            {
                if (childNode.LocalName == elementName)
                {
                    list.Add(new XmlPayload(childNode));
                }
            }

            return list;
        }

        string IPayload.Json
        {
            get
            {
                string json = string.Empty;

                // convert from Xml to json                
                //json = JsonConvert.SerializeXmlNode(_node);

                /*
                if (json.Contains("\"root\":{"))
                {
                    json = json.Replace("\"root\":{", string.Empty);
                    json = json.Trim();
                    json = json.Substring(0, json.Length - 1);
                }
                */
                return json;
            }
        }

        void IPayload.LoadPayload(IPayload payload)
        {
            LoadXml(payload.Xml);
        }
        /*
        void IPayload.LoadXmlDocument(XmlDocument xmlDocument)
        {
            _node = xmlDocument.DocumentElement;
        }
        */

        string IPayload.LocalName
        {
            get { return _node.LocalName; }
        }

        string IPayload.JsonArray
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}
