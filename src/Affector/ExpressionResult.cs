using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Affector
{
    internal class ExpressionResult
    {
        public string FunctionName { get; set; }
        public PropertyInfo TargetPropertyInfo { get; set; }
        public Expression Condition { get; set; }
        public Expression Value { get; set; }   
    }
}
