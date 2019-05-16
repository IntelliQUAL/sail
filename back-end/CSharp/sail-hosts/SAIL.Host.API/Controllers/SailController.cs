using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

using SAIL.Framework.Host;
using SAIL.Host.API.Helpers;
using SAIL.Framework.Host.Bootstrap;

namespace SAIL.Host.API.Controllers
{
    [Route("api/[controller]")]
    public class SailController : Controller
    {
        private static string _servicePrefix = string.Empty;

        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        /// <summary>
        /// Created:        05/16/2019
        /// Author:         David J. McKee
        /// Purpose:        Responds to a get request with an Id
        /// Example:        GET api/sail/json/{module}/{service}/{entity}/{id}
        /// </summary>
        [HttpGet("{responseFormatText}/{module}/{service}/{entity}/{id}")]
        public string Get(string responseFormatText, string module, string service, string entity, string id)
        {
            return ExecuteBusinessProcess(responseFormatText, module, service, entity, id);
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        private string BuildServiceName(IContext context, string module, string service)
        {
            if (string.IsNullOrWhiteSpace(_servicePrefix))
            {
                const string KEY_SERVICE_PREFIX = "ServicePrefix";
                const string DEFAULT_SERVICE_PREFIX = "svc";

                _servicePrefix = context.Get<IConfiguration>().ReadSetting(KEY_SERVICE_PREFIX, DEFAULT_SERVICE_PREFIX);
            }

            const string MAP_RESOURCE_MODULE_ONLY = "MapResourceModuleOnly";

            string serviceName = context.Get<IHostConfig>().ReadSetting(MAP_RESOURCE_MODULE_ONLY + module, string.Empty);

            if (string.IsNullOrWhiteSpace(serviceName))
            {
                serviceName = _servicePrefix + "." + module + "." + service;
            }

            return serviceName;
        }

        /// <summary>
        /// Created:        05/16/2019
        /// Author:         David J. McKee
        /// Purpose:        Processes all request types.
        /// </summary>
        private string ExecuteBusinessProcess(string responseFormatAsText, string module, string service, string entity, string dataPayload)
        {
            IConnectionHost connectionHost = null;

            //const string MS_HTTP_CONTEXT = "MS_HttpContext";
            //const string OWIN_CONTEXT = "MS_OwinContext";

            /*
            if (Request.Properties.ContainsKey(MS_HTTP_CONTEXT))
            {
                connectionHost = new ConnectionHost((HttpContextWrapper)Request.Properties[MS_HTTP_CONTEXT]);
            }
            else if (Request.Properties.ContainsKey(OWIN_CONTEXT))
            {
                //connectionHost = new ConnectionHost((OwinContext)Request.Properties[OWIN_CONTEXT]);
            }
            */

            IHostConfig hostConfiguration = new HostConfiguration();
            IBinding binder = new Binder();
            IContext context = CrossCuttingConcerns.BuildDefaultServiceContext(connectionHost, binder, hostConfiguration);

            string serviceName = BuildServiceName(context, module, service);

            const string MAP_RESOURCE_TO_PREFIX = "MapResource";

            serviceName = context.Get<IHostConfig>().ReadSetting(MAP_RESOURCE_TO_PREFIX + serviceName, serviceName);

            if (string.IsNullOrWhiteSpace(entity))
            {
                const string USE_SERVICE_AS_ENTITY_NAME = "UseServiceAsEntityName";

                bool useServiceAsEntityName = Convert.ToBoolean(context.Get<IHostConfig>().ReadSetting(USE_SERVICE_AS_ENTITY_NAME + serviceName, false.ToString()));

                if (useServiceAsEntityName)
                {
                    entity = service;
                }
            }

            if (string.IsNullOrWhiteSpace(entity) == false)
            {
                context.SetByName(SAIL.Framework.Repository.Const.Context.TABLE_OR_COLLECTION_NAME, entity);
            }

            string httpResponseMessage = ControllerCommon.ExecuteService(context, responseFormatAsText, serviceName, dataPayload, connectionHost);

            return httpResponseMessage;
        }
    }
}
