using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Affector
{
    internal class LambdaExpressionBuilder
    {
        public Expression<Func<T, T>> Build<T>(ExpressionResultSet result, AffectableEntityInfoCache cache)
        {
            var parameter = Expression.Parameter(cache.Type, "dataSet");
            var newParameter = Expression.Parameter(cache.Type, "newDataSet");
            var assign = Expression.Assign(newParameter, Expression.New(parameter.Type));
            var label = Expression.Label(newParameter.Type, "returnEntity");

            var nonAffectedProperties = cache.NonAffectablePropertyInfos.Concat(
                cache.AffectablePropertyInfos.Select(
                    _ => new KeyValuePair<string, PropertyInfo>(_.Key, _.Value.PropertyInfo)))
                    .ToDictionary(_ => _.Key, _ => _.Value);

            var expr = new List<Expression> { assign };

            foreach(var group in result.ExpressionResults.GroupBy(_=>_.TargetPropertyInfo.Name))
            {
                var info = cache.AffectableEntityInfo(group.Key);
                expr.Add(BuildMain(group.Select(_ => _).ToArray(), result.Parameters, info, parameter, newParameter));
                nonAffectedProperties.Remove(info.PropertyName);
            }

            expr.AddRange(AssignOnlyExpression(parameter, newParameter, nonAffectedProperties.Values));
            expr.Add(Expression.Return(label, newParameter, newParameter.Type));
            expr.Add(Expression.Label(label, Expression.Constant(null, newParameter.Type)));
            var block = Expression.Block(newParameter.Type, new[] { newParameter }, expr);
            var lambda = Expression.Lambda<Func<T, T>>(block, new[] { parameter });
            return lambda;
        }

        private Expression BuildMain(ExpressionResult[] expressionResults, ParameterCache paramCache, AffectableEntityInfo info,
            ParameterExpression parameter, ParameterExpression newParameter)
        {
            var newAffectableEntity = Expression.Property(newParameter, info.PropertyInfo);
            if(expressionResults.Length==0)
            {
                return newParameter;
            }

            var affectableEntity = Expression.Property(parameter, info.PropertyInfo);
            var selectParam = paramCache.Get(expressionResults[0].FunctionName);
            var lambda = Expression.Lambda(BuildIfThenElseSection(expressionResults, selectParam, info.ReturnLabel),
                selectParam.Item1);
            var selectExpr = SelectExpression(selectParam.Item1.Type, affectableEntity, lambda);

            return Expression.Assign(newAffectableEntity, selectExpr);
        }

        private Expression BuildIfThenElseSection(ExpressionResult[] expressionResults,
            Tuple<ParameterExpression,ParameterExpression> parameters, LabelTarget labelTarget)
        {
            if(expressionResults.Length==0)
            {
                throw new ArgumentException();
            }
            var label = Expression.Label(labelTarget, Expression.Constant(null, parameters.Item2.Type));
            if (expressionResults.Length ==1)
            {
                if(expressionResults[0].Condition.NodeType==ExpressionType.Default)
                {
                    return Expression.Block(new[] { parameters.Item2 },
                        expressionResults[0].Value,
                        Expression.Return(labelTarget, parameters.Item2, parameters.Item2.Type), label);
                }
            }
            var assign = Expression.Assign(parameters.Item2, Expression.Constant(null, parameters.Item2.Type));
            var returnExpr = Expression.Return(labelTarget,
                Expression.Coalesce(parameters.Item2, parameters.Item1), parameters.Item1.Type);
            var exprs = new Expression[]
                {assign}.Concat(expressionResults.Select(_ => Expression.IfThen(_.Condition, _.Value)))
                .Concat(new Expression[] { returnExpr, label });
            return Expression.Block(new[] { parameters.Item2 }, exprs);
        }

        private IEnumerable<Expression> AssignOnlyExpression(ParameterExpression parameter, ParameterExpression newParameter,
            IEnumerable<PropertyInfo> propertyInfos)
        {
            return propertyInfos.Select(pi =>
            {
                var right = Expression.Property(parameter, pi);
                var left = Expression.Property(newParameter, pi);
                return Expression.Assign(left, right);
            });
        }

        private static MethodCallExpression SelectExpression(Type type, Expression data, Expression map)
        {
            var selectExpr = Expression.Call(
                typeof(Enumerable), "Select", new[] { type, type }, data, map);
            return Expression.Call(typeof(Enumerable), "ToArray", new[] { type }, selectExpr);
        }
    }
}
