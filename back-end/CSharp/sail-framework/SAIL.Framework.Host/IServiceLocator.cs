using System;
using System.Collections.Generic;
using System.Text;

namespace SAIL.Framework.Host
{
    public interface IServiceLocator
    {
        // Important: While not required, T is usually an interface.        
        T Locate<T>(IContext context, params object[] args);
        T Locate<T>(IContext context, string moduleName, string serviceName, params object[] args);
        // Only fullname is considered when loading type.
        T LocateByName<T>(IContext context, string fullName, params object[] args);

        // Assuming T is an interface, builds a implementation name based on convention or configuration.
        string BuildImplementationFullName<T>(IContext context);
        // Dependency Injection: 
        //  T should be an interface. 
        //  The context is first searched for T and returned if found.
        //  If T is not found, then a new instance of T is created from classFullName
        //  When used for mocking, simply set the mock type on the context within your unit test.
        T Inject<T>(IContext context, string classFullName, params object[] args);

        // Builds a full class name from a module name and service name.
        // usually this is implemented as {company}.{module}.{class} or
        // {company}.{module.subsystem}.{class}
        // Company here is often a standard prefix e.g. WidgetCorp.OrderManagement.Orders
        string BuildServiceFullName<T>(IContext context, string moduleName, string serviceName);
    }
}
