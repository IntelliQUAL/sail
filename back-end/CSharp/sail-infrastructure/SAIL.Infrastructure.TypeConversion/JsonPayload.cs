using System;
using System.Xml;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.Serialization;

using SAIL.Framework.Host;

namespace SAIL.Infrastructure.TypeConversion
{
    class JsonPayload : IPayload
    { 
        //JObject _jsonObject = null;
        //JToken _jToken = null;
        //bool _isRoot = false;
        //JArray _jArray = null;

        public JsonPayload(string json)
        {
            ((IPayload)this).LoadJson(json);
        }

        /*
        public JsonPayload(JToken json)
        {
            _jToken = json;
            _isRoot = false;
        }

        public JsonPayload(JObject json, bool isRoot)
        {
            _jsonObject = json;
            _isRoot = isRoot;
        }
        */

        IPayload IPayload.this[string name]
        {
            get
            {
                IPayload result = null;

                /*
                if (_jsonObject != null)
                {
                    JToken child = _jsonObject[name];

                    result = new JsonPayload(child);
                }
                else if (_jToken != null)
                {
                    if (_jToken.Type == JTokenType.Property)
                    {
                        foreach (JObject child in _jToken.Children<JObject>())
                        {
                            foreach (JToken jToken in child.Children<JToken>())
                            {
                                IPayload childPayload = new JsonPayload(jToken);

                                if (string.Equals(childPayload.LocalName, name, StringComparison.CurrentCultureIgnoreCase))
                                {
                                    result = childPayload;
                                    break;
                                }
                            }

                            if (result != null)
                            {
                                break;
                            }
                        }
                    }
                    else if (_jToken.Type == JTokenType.Object)
                    {                        
                        foreach (JToken jToken in _jToken.Children<JToken>())
                        {
                            IPayload childPayload = new JsonPayload(jToken);

                            if (string.Equals(childPayload.LocalName, name, StringComparison.CurrentCultureIgnoreCase))
                            {
                                result = childPayload;
                                break;
                            }
                        }                    
                    }
                }
                */

                return result;
            }
        }

        IPayload IPayload.this[int index]
        {
            get
            {
                return ((IPayload)this).ChildElements[index];
            }
        }

        List<IPayload> IPayload.ChildElements
        {
            get
            {
                List<IPayload> result = new List<IPayload>();

                /*
                if (_jToken != null)
                {
                    if (_jToken.Type == JTokenType.Object)
                    {
                        foreach (object child in _jToken.Children())
                        {
                            if (child is JToken)
                            {
                                result.Add(new JsonPayload((JToken)child));
                            }
                            else
                            {

                            }
                        }
                    }
                    else if (_jToken.Type == JTokenType.Array)
                    {
                        foreach (JToken child in _jToken.Children())
                        {
                            result.Add(new JsonPayload(child));
                        }
                    }
                    else if (_jToken.Type == JTokenType.Property)
                    {
                        JProperty jProperty = (JProperty)_jToken;

                        if (jProperty.Value.Type == JTokenType.Array)
                        {
                            foreach (JToken child in jProperty.Value.Children())
                            {
                                result.Add(new JsonPayload(child));
                            }
                        }
                        else
                        {
                            // List of Property Objects
                            foreach (JObject child in _jToken.Children<JObject>())
                            {
                                foreach (JToken jToken in child.Children<JToken>())
                                {
                                    result.Add(new JsonPayload(jToken));
                                }
                            }
                        }
                    }
                }
                else if (_jsonObject != null)
                {
                    foreach (JToken child in _jsonObject.Children<JToken>())
                    {
                        result.Add(new JsonPayload(child));
                    }
                }
                */

                return result;
            }
        }

        string IPayload.Json
        {
            get
            {
                string json = string.Empty;

                /*
                if (_jsonObject != null)
                {
                    json = _jsonObject.ToString();
                }
                else if (_jToken != null)
                {
                    json = _jToken.ToString();
                }
                */

                return json;
            }
        }

        string IPayload.JsonArray
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        string IPayload.LocalName
        {
            get
            {
                string localName = string.Empty;

                /*
                if (_jToken != null)
                {
                    if (_jToken.Type == JTokenType.Property)
                    {
                        JProperty jproperty = (JProperty)_jToken;
                        localName = jproperty.Name;
                    }
                }
                else if (_jsonObject != null)
                {
                    if (_isRoot)
                    {

                    }
                    else
                    {
                        JToken jToken = _jsonObject.First;

                        if (jToken.Type == JTokenType.Property)
                        {
                            JProperty jproperty = (JProperty)jToken;
                            localName = jproperty.Name;
                        }
                    }
                }
                */

                return localName;
            }
        }

        IPayload IPayload.ParentElement
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        string IPayload.Val
        {
            get
            {
                string result = null;

                /*
                if (_jToken != null)
                {
                    if (_jToken.Type == JTokenType.Property)
                    {
                        JProperty jproperty = (JProperty)_jToken;

                        JToken valueToken = jproperty.Value;

                        if (valueToken.Type == JTokenType.Array)
                        {
                            List<string> textArray = new List<string>();

                            JArray jArray = (JArray)valueToken;

                            foreach (JToken childValueToken in jArray.Children<JToken>())
                            {
                                JValue childValueProperty = (JValue)childValueToken;

                                textArray.Add(childValueProperty.Value.ToString());
                            }

                            result = string.Join(",", textArray);
                        }
                        else
                        {
                            result = jproperty.Value.ToString();
                        }
                    }
                    else
                    {
                        result = _jToken.Value<string>();
                    }
                }
                else if (_jsonObject != null)
                {
                    if (_isRoot)
                    {

                    }
                    else
                    {
                        JToken jToken = _jsonObject.First;

                        if (jToken.Type == JTokenType.Property)
                        {
                            JProperty jproperty = (JProperty)jToken;
                            result = jproperty.Value.ToString();
                        }
                    }
                }
                */
              
                return result;
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        string IPayload.Xml
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /*
        XmlNode IPayload.XmlNode
        {
            get
            {
                throw new NotImplementedException();
            }
        }
*/

        IPayload IPayload.AppendChildAttribute(string attributeName, string attributeValue)
        {
            throw new NotImplementedException();
        }

        IPayload IPayload.AppendChildElement(string elementName)
        {
            /*
            JProperty newProperty = new JProperty(elementName);

            if (_jsonObject == null)
            {
                _jsonObject = (JObject)_jToken;
            }
            
            _jsonObject.Add(newProperty);

            JObject child = new JObject(newProperty);
            */

            //return new JsonPayload(child);
            return null;
        }

        IPayload IPayload.AppendChildElement(string elementName, dynamic innerText)
        {
            /*
            JProperty newProperty = new JProperty(elementName, innerText);

            if (_jsonObject == null)
            {
                _jsonObject = (JObject)_jToken;
            }

            _jsonObject.Add(newProperty);

            JObject child = new JObject(newProperty);
            */

            //return new JsonPayload(child);
            return null;
        }

        IPayload IPayload.AppendChildElement(string elementName, dynamic innerText, string namespaceUri)
        {
            throw new NotImplementedException();
        }

        IPayload IPayload.AppendXmlFragment(string xmlFragment)
        {
            throw new NotImplementedException();
        }

        IPayload IPayload.GetChildElement(string elementName)
        {
            throw new NotImplementedException();
        }

        List<IPayload> IPayload.GetChildElements(string elementName)
        {
            throw new NotImplementedException();
        }

        void IPayload.LoadJson(string jsonPayload)
        {
            if (jsonPayload.StartsWith("["))
            {
                //_jArray = JArray.Parse(jsonPayload);
            }
            else
            {
                //_jsonObject = JObject.Parse(jsonPayload);
            }

            //_isRoot = true;
        }

        void IPayload.LoadPayload(IPayload payload)
        {
            throw new NotImplementedException();
        }

        void IPayload.LoadXml(string xmlPayload)
        {
            throw new NotImplementedException();
        }



        IPayload IPayload.SelectChildElement(string xPath)
        {
            throw new NotImplementedException();
        }

        IPayload IPayload.Where(string parentElementNameIs, string childElementNameIs, string innerTextIs)
        {
            throw new NotImplementedException();
        }
    }
}
