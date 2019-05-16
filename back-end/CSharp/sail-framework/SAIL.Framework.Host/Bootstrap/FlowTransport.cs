using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace SAIL.Framework.Host.Bootstrap
{
    public class FlowTransport<I> : IContext
    {
        private I _payload = default(I);                // RAW payload (input)
        private object _syncRoot = new object();        // thread lock

        public object SyncRoot
        {
            get { return _syncRoot; }
            set { _syncRoot = value; }
        }

        private ConcurrentDictionary<string, object> _dictionary = new ConcurrentDictionary<string, object>();   // working data

        public FlowTransport(I payload)
        {
            // Assumption the request is being modified, so the response is null.
            _payload = payload;
        }

        public FlowTransport(I payload, IContext serviceContext)
        {
            // Assumption the request is being modified, so the response is null.
            _payload = payload;

            foreach (string key in serviceContext.Collection.Keys)
            {
                this[key] = serviceContext.GetByName(key);
            }
        }

        public I Payload
        {
            get
            {
                return _payload;
            }
        }

        // to ensure thready safety
        public object this[string index]
        {
            get
            {
                object item = null;

                if (_dictionary.ContainsKey(index))
                {
                    item = _dictionary[index];
                }

                return item;
            }

            set
            {
                _dictionary[index] = value;
            }
        }

        public C Get<C>()
        {
            // find the correct item
            C result = default(C);

            // Attempt to find the result by its' type
            result = FindObjectOfTypeC<C>(result);

            if ((result == null) &&
                (_payload != null))
            {
                // If we didn't find it by type, then check to see if the payload is the type we are looking for.
                if (typeof(C).GetTypeInfo().IsAssignableFrom(_payload.GetType().GetTypeInfo()))
                {
                    result = (C)(object)_payload;
                }
            }

            if (result == null)
            {
                // If we didn't find it by type or within the payload,
                //  then, if we have a ServiceLocator present, attempt to load it by the service locator.
                IServiceLocator serviceLocator = FindObjectOfTypeC<IServiceLocator>(default(IServiceLocator));

                if (serviceLocator != null)
                {
                    IContext context = this;

                    // Attempt to locate by convention.
                    string implementationName = serviceLocator.BuildImplementationFullName<C>(context);

                    result = (C)context.Get<IBinding>().LoadViaFullName(context, implementationName);
                }
            }

            return result;
        }

        private C FindObjectOfTypeC<C>(C result)
        {
            foreach (string index in this.IndexList)
            {
                if (_dictionary[index] != null)
                {
                    if (typeof(C).GetTypeInfo().IsAssignableFrom(_dictionary[index].GetType().GetTypeInfo()))
                    {
                        result = (C)_dictionary[index];
                        break;
                    }
                }
            }
            return result;
        }

        public C Get<C>(string index)
        {
            return (C)this[index];
        }

        private List<string> IndexList
        {
            get
            {
                List<string> indexList = new List<string>();

                foreach (object index in _dictionary.Keys)
                {
                    indexList.Add(index.ToString());
                }

                return indexList;
            }
        }

        //R IContext.Payload<R>()
        //{
        //    object result = this.Payload;

        //    return (R)result;
        //}

        void IContext.Set(object any)
        {
            if (any != null)
            {
                this[any.GetType().FullName] = any;
            }
        }

        C IContext.Get<C>()
        {
            return (C)this.Get<C>();
        }

        object IContext.GetByName(string name)
        {
            return this[name];
        }


        void IContext.SetByName(string name, object any)
        {
            this[name] = any;
        }

        T IContext.GetByNameAs<T>(string name)
        {
            T result = default(T);

            object value = ((IContext)this).GetByName(name);

            if (value != null)
            {
                result = (T)Convert.ChangeType(value, typeof(T));
            }

            return result;
        }

        ConcurrentDictionary<string, object> IContext.Collection
        {
            get
            {
                return _dictionary;
            }
        }
    }
}
