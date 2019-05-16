using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SAIL.Framework.Host
{
    /// <summary>
    /// Tip: Use DbgView to read these messages.
    /// </summary>
    class DefaultExceptionHandler : IExceptionHandler
    {
        //[DllImport("Kernel32.dll", EntryPoint = "OutputDebugString")]
        //internal static extern void OutputDebugString(String s);

        void IExceptionHandler.HandleException(IContext context, Exception ex)
        {
            //OutputDebugString(ex.ToString() + '\n');
            Console.WriteLine(ex.ToString());
        }

        void IExceptionHandler.HandleException(IContext context, Exception ex, string detail)
        {
            //OutputDebugString(ex.ToString() + "|" + detail + '\n');
            Console.WriteLine(ex.ToString() + "|" + detail);
        }

        void IExceptionHandler.HandleException(IContext context, Exception ex, IServiceResponse serviceResponse)
        {
            string message = ex.ToString();

            serviceResponse.UserMessage = "Unhandled Exception";
            serviceResponse.ErrorText = message;

            //OutputDebugString(message + '\n');
            Console.WriteLine(message);
        }
    }
}
