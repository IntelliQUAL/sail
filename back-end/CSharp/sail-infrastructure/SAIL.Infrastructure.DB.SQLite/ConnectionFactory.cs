using System;

namespace SAIL.Infrastructure.DB.SQLite
{
    public class ConnectionFactory : IConnectionFactory
    {

        T IConnectionFactory.GetConnection<T>(Framework.AssemblyLine.IContext context, string connectionName, string technologyCode)
        {
            dynamic connection = null;

            string connectionString = string.Empty;

            try
            {
                try
                {
                    //If the context is in a transaction, this will return a connection.
                    connection = (SQLiteConnection)context.GetByName(connectionName + "_" + technologyCode);
                }
                catch (System.Exception ex2)
                {
                    context.Get<IExceptionHandler>().HandleException(ex2);
                }

                if (connection == null)
                {
                    IServiceLocator serviceLocator = context.Get<IServiceLocator>();

                    string connectionStringProvider = this.GetType().FullName.Replace("ConnectionFactory", "ConnectionStringProvider");

                    //A transaction has not been started, so create a new connection.
                    connectionString = serviceLocator.LocateByName<IConnectionStringProvider>(connectionStringProvider).GetConnectionString(context, connectionName, technologyCode);

                    connection = new SQLiteConnection(connectionString);

                    connection.Open();
                }
            }
            catch (System.Exception ex)
            {
                StringBuilder stringBuilder = new StringBuilder();

                System.Exception innerException = ex.InnerException;

                while (innerException != null)
                {
                    stringBuilder.AppendLine(innerException.StackTrace);
                    stringBuilder.AppendLine(innerException.Message);

                    innerException = innerException.InnerException;
                }

                context.Get<IExceptionHandler>().HandleExceptionWithDetail(ex, "ConnectionString: " + connectionString + " " + stringBuilder.ToString());
            }

            return connection;
        }
    }
}
}
