using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using System.Runtime.Serialization;

namespace IQ.Entities.VastMetaDB.SqlEsque
{
    [Serializable]
    [DataContract(Namespace = "")]
    public class Where : PredicateParent
    {
        public Where(Predicate predicate) : base(predicate)
        {

        }

        public Where(Predicate predicate, AND and) : base(predicate, and)
        {

        }

        public Where(Predicate predicate, OR or) : base(predicate, or)
        {

        }
    }
}
