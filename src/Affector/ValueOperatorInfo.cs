using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Affector
{
    internal class ValueOperatorInfo
    {
        public string PropertyName { get; set; }
        public ExpressionType ExpressionType { get; set; }
        public Expression Value { get; set; }
    }
}
