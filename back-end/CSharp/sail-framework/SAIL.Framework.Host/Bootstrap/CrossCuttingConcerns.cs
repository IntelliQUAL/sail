using System;
using System.Text;
using System.Collections.Generic;

namespace SAIL.Framework.Host.Bootstrap
{
    /// <summary>
    /// Created:        04/27/2017
    /// Author:         David J. McKee
    /// Purpose:        
    /// </summary>
    public static class CrossCuttingConcerns
    {

        private static IBinding _binder = null;
        private static IConfiguration _hostConfiguration = null;
        private static IExceptionHandler _exceptionHandler = null;
        private static IRequestHelper _requestHelper = null;
        private static IResponseHelper _responseHelper = null;
        //private static IFileIO _fileIo = null;
        private static IMD5 _md5 = null;
        private static ITrace _trace = null; 

        [ThreadStatic]
        private static IContext _threadContext = null;

        /// <summary>
        /// Created:        06/15/2017
        /// Author:         David J. McKee
        /// Purpose:        Loads all standard cross cutting concerns into a common context.
        /// </summary>
        public static IContext BuildDefaultServiceContext(IConnectionHost connectionHost, IBinding binding, IConfiguration hostConfiguration)
        {
            IContext context = new FlowTransport<object>(null);

            try
            {
                _binder = binding;
                _hostConfiguration = hostConfiguration;

                context.SetByName("IConnectionHost", connectionHost);
                context.Set(binding);
                context.Set(hostConfiguration);
                context.Set(LoadCrossCuttingConcern(context, "ServiceLocator", "SAIL.Infrastructure.Services.ServiceLocator"));
                context.Set(LoadCrossCuttingConcern(context, "RequestHelper", "SAIL.Infrastructure.TypeConversion.RequestHelper"));
                context.Set(LoadCrossCuttingConcern(context, "ResponseHelper", "SAIL.Infrastructure.TypeConversion.ResponseHelper"));
                context.Set(LoadCrossCuttingConcern(context, "ExceptionHandler", typeof(DefaultExceptionHandler).FullName));
                context.Set(LoadCrossCuttingConcern(context, "Trace", typeof(DefaultTracer).FullName));
                context.Set(LoadCrossCuttingConcern(context, "CsvIO", "SAIL.Infrastructure.FileSystem.CsvIo"));
                context.Set(LoadCrossCuttingConcern(context, "QueueFactory", "SAIL.Infrastructure.Queue.RabbitMQ.QueueFactory"));
                context.Set(LoadCrossCuttingConcern(context, "PayloadFactory", "SAIL.Infrastructure.TypeConversion.PayloadFactory"));
                context.Set(LoadCrossCuttingConcern(context, "AppConfigFactor", "SAIL.Infrastructure.Services.AppConfigFactory"));

                _threadContext = context;
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }

            return context;
        }

        internal static FlowTransport<object> BuildDefaultServiceContext(IConnectionHost connectionHost, IBusinessProcess businessProcess)
        {
            FlowTransport<object> flowTransport = new FlowTransport<object>(null);

            IContext context = flowTransport;

            context.Set(connectionHost);
            context.Set(businessProcess);

            return flowTransport;
        }

        internal static IMD5 Md5
        {
            get
            {
                if (_md5 == null)
                {
                    IContext context = new FlowTransport<object>(null);

                    _md5 = (IMD5)LoadCrossCuttingConcern(context, "MD5", typeof(DefaultMD5Handler).FullName);
                }

                return _md5;
            }

        }

        /// <summary>
        /// Created:        06/15/2017
        /// Author:         David J. McKee
        /// Purpose:        Returns an implementation of the IRequestHelper
        /// </summary>
        internal static IRequestHelper RequestHelper(IContext context)
        {
            if (_requestHelper == null)
            {
                _requestHelper = (IRequestHelper)LoadCrossCuttingConcern(context, "RequestHelper", "SAIL.Infrastructure.TypeConversion.RequestHelper");
            }

            return _requestHelper;
        }

        internal static IResponseHelper ResponseHelper(IContext context)
        {
            if (_responseHelper == null)
            {
                _responseHelper = (IResponseHelper)LoadCrossCuttingConcern(context, "ResponseHelper", "SAIL.Infrastructure.TypeConversion.ResponseHelper");
            }

            return _responseHelper;
        }

        internal static ITrace Trace(IContext context)
        {
            if (_trace == null)
            {
                _trace = (ITrace)LoadCrossCuttingConcern(context, "Trace", typeof(DefaultTracer).FullName);
            }

            return _trace;
        }

        /// <summary>
        /// Created:        06/15/2017
        /// Author:         David J. McKee
        /// Purpose:        Returns an implementation of the IExceptionHandler
        /// </summary>
        internal static IExceptionHandler ExceptionHandler(IContext context)
        {
            if (_exceptionHandler == null)
            {
                if (context == null)
                {
                    context = _threadContext;
                }

                _exceptionHandler = (IExceptionHandler)LoadCrossCuttingConcern(context, "ExceptionHandler", "PIPE.Infrastructure.ExceptionHandling.ExceptionHandler");
            }

            return _exceptionHandler;
        }

        /// <summary>
        /// Created:        06/15/2017
        /// Author:         David J. McKee
        /// Purpose:        Dynamically loads a Cross Cutting Concern based on a configuration key   
        /// IMPORTANT:      THIS MAY BE THE MOST IMPORTANT METHOD IN THE ENTIRE APPLICATION.
        /// </summary>        
        private static object LoadCrossCuttingConcern(IContext context, string configKey, string defaultImplementation)
        {
            object ccc = null;
            StringBuilder issueList = new StringBuilder();

            try
            {
                string componentFullName = defaultImplementation;

                try
                {
                    componentFullName = _hostConfiguration.ReadSetting(configKey, defaultImplementation);

                    if (string.IsNullOrWhiteSpace(componentFullName))
                    {
                        componentFullName = defaultImplementation;
                    }
                }
                catch (System.Exception ex2)
                {
                    if (_exceptionHandler != null)
                    {
                        _exceptionHandler.HandleException(context, ex2, issueList.ToString());
                    }
                }

                ccc = _binder.LoadViaFullName(context, componentFullName, issueList);
            }
            catch (System.Exception ex)
            {
                if (_exceptionHandler == null)
                {
                    // If we get here we have major problems
                    throw new System.Exception("Exception Handler Not Present", ex);
                }
                else
                {
                    _exceptionHandler.HandleException(context, ex, issueList.ToString());
                }
            }

            return ccc;
        }
    }
}
