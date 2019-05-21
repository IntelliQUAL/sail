using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace IQ.Entities.VastMetaDB.SqlEsque
{
    [Serializable]
    public class AND : PredicateParent
    {
        public AND(Predicate predicate) : base(predicate)
        {

        }

        public AND(Predicate predicate, AND and) : base(predicate, and)
        {

        }

        public AND(Predicate predicate, OR or) : base(predicate, or)
        {

        }
    }
}
