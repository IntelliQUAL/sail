using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SAIL.ViewModel.Generic
{
    public class predicate
    {
        public string field = string.Empty;
        public string @operator = string.Empty;
        public string value = string.Empty;
        public bool leftParen = false;
        public bool rightParen = false;

        public AND and = null;
        public OR or = null;
    }
}
