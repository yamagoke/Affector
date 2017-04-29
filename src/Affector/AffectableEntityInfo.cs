using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Affector
{
    internal class AffectableEntityInfo
    {
        public PropertyInfo PropertyInfo { get; private set; }
        public LabelTarget ReturnLabel { get; private set; }
        public Type Type { get; private set; }
        public string PropertyName { get; private set; }
        public Dictionary<string, GetSetProperty> PropertyExpressions { get; private set; }

        public AffectableEntityInfo(PropertyInfo pi)
        {
            PropertyInfo = pi;
            Type = pi.PropertyType.GetElementType();
            PropertyName = pi.Name;
            PropertyExpressions = CreatePropertyExpressions(Type, pi.Name);
            ReturnLabel = Expression.Label(Type, "returnValue");
        }

        public bool TryGetKeyProperty(int keyOrder, out GetSetProperty property)
        {
            property = PropertyExpressions.Values.FirstOrDefault(_ => _.KeyOrder == keyOrder);
            return property != null;
        }

        public GetSetProperty[] GetValueProperty(string targetValueName)
        {
            var rtn = PropertyExpressions.Values
                .Where(v => v.TargetValueNames.Any(_ => _ == targetValueName)).ToArray();
            if(rtn.Length==0)
            {
                throw new ArgumentException("targetValueName({0}) is not found.", targetValueName);
            }
            return rtn;
        }

        private Dictionary<string,GetSetProperty> CreatePropertyExpressions(Type type, string propertyName)
        {
            var rtn = new Dictionary<string, GetSetProperty>();
            foreach(var pi in type.GetProperties())
            {
                if(pi.CanRead && pi.GetMethod.IsPublic && pi.CanWrite && pi.SetMethod.IsPublic)
                {
                    rtn[pi.Name] = new GetSetProperty(pi, propertyName);
                }
            }
            return rtn;
        }

        private MemberInitExpression GenerateCopyExpression(Expression entity)
        {
            var ctor = Expression.New(Type);
            return Expression.MemberInit(ctor, CreateMemberAssignment(entity));
        }

        private IEnumerable<MemberAssignment> CreateMemberAssignment(Expression entity)
        {
            foreach(var pe in PropertyExpressions)
            {
                var value = pe.Value.GenerateGetProperty(entity);
                var prop = pe.Value.PropertyInfo;
                yield return Expression.Bind(prop, value);
            }
        }

        public Expression IfThenSection(ParameterExpression entity, ParameterExpression newEntity, ValueOperatorInfo[] valueOpInfos)
        {
            var newEntityExpr = Expression.Coalesce(newEntity, GenerateCopyExpression(entity));
            var assigned = Expression.Assign(newEntity, newEntityExpr);
            var exprs = new List<Expression> { assigned };
            exprs.AddRange(ValueOperationExpression(newEntity, valueOpInfos));
            return Expression.Block(exprs);
        }

        private IEnumerable<Expression> ValueOperationExpression(ParameterExpression newEntity, IEnumerable<ValueOperatorInfo> valueOpInfos)
        {
            foreach(var info in valueOpInfos)
            {
                var propExprPair = PropertyExpressions[info.PropertyName].GenerateSetProperty(info.ExpressionType, newEntity);
                var setLambda = Expression.Lambda(propExprPair.Item1, newEntity, propExprPair.Item2);
                yield return Expression.Invoke(setLambda, newEntity, info.Value);
            }
        }
    }
}
