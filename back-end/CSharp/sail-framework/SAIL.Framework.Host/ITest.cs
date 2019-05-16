using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using SAIL.Framework.Host.Enums;

namespace SAIL.Framework.Host
{
    public interface ITest
    {
        TestResult ExecuteTest(IContext context, ResponseFormat responseFormat);
    }
}
