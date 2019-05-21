using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using SAIL.Framework.Host;
using SAIL.Framework.Host.Bootstrap;

using IQ.Entities.VastDB;

namespace IQ.BUS.Vast.Common
{
    public class AppendRequestTableSchema : IOperation<IQ.Entities.VastDB.Entity, IQ.Entities.VastDB.Entity>
    {
        string _configSourceId = string.Empty;

        public AppendRequestTableSchema()
        {

        }

        public AppendRequestTableSchema(string configSourceId)
        {
            _configSourceId = configSourceId;
        }

        /// <summary>
        /// Created:        07/16/2015
        /// Author:         David J. McKee
        /// Purpose:        Reads full meta data on a given table.
        /// </summary>        
        public static Entities.VastMetaDB.Table GetTableMetaData(IContext context, string databaseName, string tableID)
        {
            Entities.VastDB.Entity newEntity = new Entities.VastDB.Entity();

            // Backup the current database in context
            string originalContextDatabaseName = context.GetByName(IQ.Entities.VastDB.Const.Context.DATABASE_ID).ToString();

            try
            {
                // Set the passed-in database as the current context database
                context.SetByName(IQ.Entities.VastDB.Const.Context.DATABASE_ID, databaseName);

                FlowTransport<Entities.VastDB.Entity> newEntityContext;
                List<Operation<IQ.Entities.VastDB.Entity, IQ.Entities.VastDB.Entity>> operationList;
                InitTableForOperationList(context, databaseName, tableID, newEntity, out newEntityContext, out operationList);

                IQ.BUS.Vast.Common.DefaultOerationLists.AppendNewOperationList(context, operationList);

                // Execute each operation
                foreach (Operation<IQ.Entities.VastDB.Entity, IQ.Entities.VastDB.Entity> operation in operationList)
                {
                    IOperation<IQ.Entities.VastDB.Entity, IQ.Entities.VastDB.Entity> operationToExec = operation.OperationInstance;

                    IQ.Entities.VastDB.Entity input = newEntityContext.Payload;

                    operationToExec.Execute(context, input, ref newEntity);
                }
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }
            finally
            {
                // Restore the database
                context.SetByName(IQ.Entities.VastDB.Const.Context.DATABASE_ID, originalContextDatabaseName);
            }

            return newEntity.Table;
        }

        public static void InitTableForOperationList(IContext context,
                                                        string databaseName,
                                                        string tableID,
                                                        Entities.VastDB.Entity newEntity,
                                                        out FlowTransport<Entities.VastDB.Entity> newEntityContext,
                                                        out List<Operation<IQ.Entities.VastDB.Entity,
                                                        IQ.Entities.VastDB.Entity>> operationList)
        {
            newEntity.TableID = tableID;
            newEntity.Table = new IQ.Entities.VastMetaDB.Table();
            newEntity.Table.ID = newEntity.TableID;
            newEntity.ActionResponseType = IQ.Entities.VastDB.Const.Actions.ACTION_NEW;

            // Read sysTableSchmea                
            MetaDataHelper.AppendTableMetaData(context, databaseName, newEntity.Table, IQ.Entities.VastDB.Const.Actions.ACTION_NEW);

            newEntityContext = new FlowTransport<Entities.VastDB.Entity>(newEntity, context);

            operationList = new List<Operation<IQ.Entities.VastDB.Entity, IQ.Entities.VastDB.Entity>>();
        }

        void IOperation<Entity, Entity>.Execute(IContext context, Entity input, ref Entity output)
        {
            try
            {
                if (context.Payload<IQ.Entities.VastDB.Entity>().Table == null)
                {
                    context.Payload<IQ.Entities.VastDB.Entity>().Table = new Entities.VastMetaDB.Table();
                    context.Payload<IQ.Entities.VastDB.Entity>().Table.ID = context.Payload<IQ.Entities.VastDB.Entity>().TableID;
                }

                // Append Column List
                context.Payload<IQ.Entities.VastDB.Entity>().Table.ColumnList =
                    IQ.BUS.Vast.Common.MetaDataHelper.SearchColumnList(context,
                                                                        context.Payload<IQ.Entities.VastDB.Entity>().Table.ColumnVisibilityInstanceGUID,
                                                                        context.Payload<IQ.Entities.VastDB.Entity>().Table.ColumnVisibilityDatabaseName,
                                                                        context.Payload<IQ.Entities.VastDB.Entity>().Table.ColumnVisibilityTableName,
                                                                        context.Payload<IQ.Entities.VastDB.Entity>().Table.ID,
                                                                        context.Payload<IQ.Entities.VastDB.Entity>().Table.PrimaryKeyColumnID,
                                                                        IQ.Entities.VastDB.Const.Actions.ACTION_CREATE);
            }
            catch (System.Exception ex)
            {
                context.Get<IExceptionHandler>().HandleException(context, ex);
            }
        }
    }
}
