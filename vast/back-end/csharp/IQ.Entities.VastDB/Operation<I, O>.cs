using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using SAIL.Framework.Host;

namespace IQ.Entities.VastDB
{
    public class Operation<I, O>
    {
        public int OperationID;
        public string ConfigSourceKey;
        public string ConfigSourceID;
        public int Sequence;
        public bool Async;
        public IOperation<I, O> OperationInstance = null;
    }
}
