using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using System.Runtime.Serialization;
using IQ.Entities.VastMetaDB.SqlEsque;

namespace IQ.Entities.VastMetaDB.SqlEsque
{
    [Serializable]
    [DataContract(Namespace = "")]
    public class PredicateParent
    {
        private Predicate _predicate = null;
        private AND _and = null;
        private OR _or = null;

        [DataMember(EmitDefaultValue = false)]
        public OR Or
        {
            get { return _or; }
            set { _or = value; }
        }

        [DataMember(EmitDefaultValue = false)]
        public AND And
        {
            get { return _and; }
            set { _and = value; }
        }

        [DataMember(EmitDefaultValue = false)]
        public Predicate Predicate
        {
            get { return _predicate; }
            set { _predicate = value; }
        }

        public PredicateParent(Predicate predicate)
        {
            _predicate = predicate;
        }

        public PredicateParent(Predicate predicate, AND and)
        {
            _predicate = predicate;
            _and = and;
        }

        public PredicateParent(Predicate predicate, OR or)
        {
            _predicate = predicate;
            _or = or;
        }
    }
}
