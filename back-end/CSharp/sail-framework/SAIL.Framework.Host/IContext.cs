using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.Concurrent;

namespace SAIL.Framework.Host
{
    /// <summary>
    /// Created:        05/16/2019
    /// Author:         David J. McKee
    /// Purpose:        Provides a generic context that can be used everywhere.
    /// </summary>
    public interface IContext
    {
        // Reads the first instance of a given type.
        C Get<C>();

        // Includes any variable on the context 
        void Set(object any);

        // Reads a given variable from the context using a specific name and type.
        T GetByNameAs<T>(string name);

        // Reads a given variable by name only.
        object GetByName(string name);

        // Includes a variable on the context in a specific named slot.
        void SetByName(string name, object any);

        // Reads the entire context as a dictionary.
        ConcurrentDictionary<string, object> Collection { get; }

        R Payload<R>();
    }
}
