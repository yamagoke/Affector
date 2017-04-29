using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Affector
{
    internal class GetSetProperty
    {
        public int? KeyOrder { get; private set; }
        public IEnumerable<string> TargetValueNames { get; private set; }
        public PropertyInfo PropertyInfo { get; private set; }
        
        public GetSetProperty(PropertyInfo pi, string parentPropertyName)
        {
            KeyOrder = GenerateKeyOrder(pi);
            TargetValueNames = GenerateTargetValueNames(pi, parentPropertyName);
            PropertyInfo = pi;
        }

        private int? GenerateKeyOrder(PropertyInfo pi)
        {
            var key = pi.GetCustomAttribute(typeof(KeyAttribute));
            return key == null ? (int?)null : ((KeyAttribute)key).Index;
        }

        private IEnumerable<string> GenerateTargetValueNames(PropertyInfo pi, string parentPropertyName)
        {
            var value = pi.GetCustomAttribute(typeof(ValueAttribute));
            if (value==null)
            {
                return Enumerable.Empty<string>();
            }
            var attr = (ValueAttribute)value;
            return attr.Names.Any()
                ? attr.Names.Select(_ => string.IsNullOrEmpty(_) ? parentPropertyName : parentPropertyName + "." + _).ToArray()
                : new[] { parentPropertyName };
        }

        public MemberExpression GenerateGetProperty(Expression entity)
        {
            return Expression.PropertyOrField(entity, PropertyInfo.Name);
        }

        public Tuple<BinaryExpression,ParameterExpression> GenerateSetProperty(ExpressionType expressionType, Expression entity)
        {
            var left = Expression.PropertyOrField(entity, PropertyInfo.Name);
            var setValue = Expression.Parameter(left.Type, "setValue");
            return Tuple.Create(Expression.MakeBinary(expressionType, left, setValue), setValue);
        }
    }
}
