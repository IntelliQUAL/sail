using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAIL.ViewModel.SqlEsque
{
    public class OR : predicate
    {
        public OR()
        {

        }

        public OR(predicate predicateInfo)
        {
            this.@operator = predicateInfo.@operator;
            this.and = predicateInfo.and;
            this.field = predicateInfo.field;
            this.leftParen = predicateInfo.leftParen;
            this.or = predicateInfo.or;
            this.rightParen = predicateInfo.rightParen;
            this.value = predicateInfo.value;
        }

        public OR(string @operator,
                    AND and,
                    string field,
                    bool leftParen,
                    OR or,
                    bool rightParen,
                    string value)
        {
            this.@operator = @operator;
            this.and = and;
            this.field = field;
            this.leftParen = leftParen;
            this.or = or;
            this.rightParen = rightParen;
            this.value = value;
        }
    }
}
