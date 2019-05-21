using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using SAIL.Framework.Host;

using IQ.Entities.VastDB;

namespace IQ.BUS.Vast.Common
{
    public static class DefaultOerationLists
    {
        public static void AppendNewOperationList(IContext context,
                                                    List<Operation<IQ.Entities.VastDB.Entity, IQ.Entities.VastDB.Entity>> operationList)
        {
            // Add Standard Table Schema
            AppendEntityOperationInstance(context, "IQ.BUS.Vast.Common.AppendTableSchema", operationList);

            // Add Standard Column Schemas                
            AppendEntityOperationInstance(context, "IQ.OPS.New.ValidateColumnSchemas", operationList);

            // Exclude all columns marked as internal from the response
            AppendEntityOperationInstance(context, "IQ.OPS.New.RemoveInternalColumns", operationList);
        }

        public static void AppendEntityOperationInstance(IContext context,
                                                            string operationFullName,
                                                            List<Operation<IQ.Entities.VastDB.Entity, IQ.Entities.VastDB.Entity>> operationList)
        {
            Operation<IQ.Entities.VastDB.Entity, IQ.Entities.VastDB.Entity> operation = new Operation<Entity, Entity>();
            operation.OperationInstance = context.Get<IServiceLocator>().LocateByName<IOperation<IQ.Entities.VastDB.Entity, IQ.Entities.VastDB.Entity>>(context, operationFullName);
            operationList.Add(operation);
        }

        public static void AppendEntityOperationInstance(IContext context,
                                                            IOperation<IQ.Entities.VastDB.Entity, IQ.Entities.VastDB.Entity> operationInstance,
                                                            List<Operation<IQ.Entities.VastDB.Entity, IQ.Entities.VastDB.Entity>> operationList)
        {
            Operation<IQ.Entities.VastDB.Entity, IQ.Entities.VastDB.Entity> operation = new Operation<Entity, Entity>();
            operation.OperationInstance = operationInstance;
            operationList.Add(operation);
        }

        public static void AppendSearchOperationInstance(IContext context,
                                                            string operationFullName,
                                                            List<Operation<IQ.Entities.VastDB.EntitySearch, IQ.Entities.VastDB.SearchResponse>> operationList)
        {
            try
            {
                Operation<IQ.Entities.VastDB.EntitySearch, IQ.Entities.VastDB.SearchResponse> operation = new Operation<IQ.Entities.VastDB.EntitySearch, IQ.Entities.VastDB.SearchResponse>();

                operation.OperationInstance = context.Get<IServiceLocator>().LocateByName<IOperation<IQ.Entities.VastDB.EntitySearch, IQ.Entities.VastDB.SearchResponse>>(context, operationFullName);

                if (operation.OperationInstance != null)
                {
                    operationList.Add(operation);
                }
                else
                {
                    context.Get<ITrace>().Emit(System.Diagnostics.TraceLevel.Error, "Unable to load: " + operationFullName);
                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }
        }
    }
}
