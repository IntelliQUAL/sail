using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using SAIL.Framework.Host.Enums;

namespace SAIL.Framework.Host.BaseClasses
{
    public abstract class SecureRESTBase<CreateI, CreateO, ReadO, UpdateI, UpdateO, DeleteO, SearchI, SearchO> : RESTBase<CreateI, CreateO, ReadO, UpdateI, UpdateO, DeleteO, SearchI, SearchO>
        where CreateI : IServiceResponse
        where CreateO : IServiceResponse
        where ReadO : IServiceResponse
        where UpdateO : IServiceResponse
        where DeleteO : IServiceResponse
        where SearchO : IServiceResponse
    {
        public abstract CreateI ProcessNew(IContext context);
        public abstract CreateO ProcessCreate(IContext context, CreateI request);
        public abstract ReadO ProcessRead(IContext context, string id);
        public abstract UpdateO ProcessUpdate(IContext context, UpdateI request);
        public abstract DeleteO ProcessDelete(IContext context, string id);
        public abstract SearchO ProcessSearchAll(IContext context);
        public abstract SearchO ProcessSearch(IContext context, SearchI searchCriteria);
        public abstract bool IsAuthorized(IContext context, RESTAction action, object request);

        private void SetNotAuthorized(IContext context, IServiceResponse serviceResponse)
        {
            try
            {
                serviceResponse.CorrelationId = context.GetByName(SAIL.Framework.Host.Consts.Context.CORRELATION_ID).ToString();
            }
            catch { }

            serviceResponse.ErrorCode = "401";
            serviceResponse.ErrorText = "401 Unauthorized";
            serviceResponse.Success = false;
            serviceResponse.UserMessage = "Unauthorized";

            // Set the Http response.
            context.SetByName("HttpStatusCode", "Unauthorized");
        }

        public override CreateI ProcessNewRequest(IContext context)
        {
            CreateI response = default(CreateI);

            if (IsAuthorized(context, RESTAction.New, null))
            {
                response = ProcessNew(context);
            }
            else
            {
                response = (CreateI)Activator.CreateInstance(typeof(CreateI));
                SetNotAuthorized(context, response);
            }

            return response;
        }

        public override CreateO ProcessCreateRequest(IContext context, CreateI request)
        {
            CreateO response = default(CreateO);

            if (IsAuthorized(context, RESTAction.Create, request))
            {
                response = ProcessCreate(context, request);
            }
            else
            {
                response = (CreateO)Activator.CreateInstance(typeof(CreateO));
                SetNotAuthorized(context, response);
            }

            return response;
        }

        public override ReadO ProcessReadRequest(IContext context, string id)
        {
            ReadO response = default(ReadO);

            if (IsAuthorized(context, RESTAction.Read, id))
            {
                response = ProcessRead(context, id);
            }
            else
            {
                response = (ReadO)Activator.CreateInstance(typeof(ReadO));
                SetNotAuthorized(context, response);
            }

            return response;
        }

        public override UpdateO ProcessUpdateRequest(IContext context, UpdateI request)
        {
            UpdateO response = default(UpdateO);

            if (IsAuthorized(context, RESTAction.Update, request))
            {
                response = ProcessUpdate(context, request);
            }
            else
            {
                response = (UpdateO)Activator.CreateInstance(typeof(UpdateO));
                SetNotAuthorized(context, response);
            }

            return response;
        }

        public override DeleteO ProcessDeleteRequest(IContext context, string id)
        {
            DeleteO response = default(DeleteO);

            if (IsAuthorized(context, RESTAction.Delete, id))
            {
                response = ProcessDelete(context, id);
            }
            else
            {
                response = (DeleteO)Activator.CreateInstance(typeof(DeleteO));
                SetNotAuthorized(context, response);
            }

            return response;
        }

        public override SearchO ProcessSearchAllRequest(IContext context)
        {
            SearchO response = default(SearchO);

            if (IsAuthorized(context, RESTAction.Search, null))
            {
                response = ProcessSearchAll(context);
            }
            else
            {
                response = (SearchO)Activator.CreateInstance(typeof(SearchO));
                SetNotAuthorized(context, response);
            }

            return response;
        }

        public override SearchO ProcessSearchRequest(IContext context, SearchI request)
        {
            SearchO response = default(SearchO);

            if (IsAuthorized(context, RESTAction.Search, request))
            {
                response = ProcessSearch(context, request);
            }
            else
            {
                response = (SearchO)Activator.CreateInstance(typeof(SearchO));
                SetNotAuthorized(context, response);
            }

            return response;
        }
    }
}
