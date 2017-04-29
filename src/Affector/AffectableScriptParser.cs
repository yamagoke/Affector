using Sprache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Affector
{
    internal class AffectableScriptParser
    {
        public static ExpressionResultSet ParseExpression(string script, AffectableEntityInfoContainer container)
        {
            return Lambda(container).Parser.Parse(script);
        }

        private static readonly Parser<char> Comma = Parse.Char(',').Token();

        private static ParserWithContainer<Expression, KeyPropertyContainer> KeyTerm(KeyPropertyContainer container)
        {
            return new ParserWithContainer<Expression, KeyPropertyContainer>(
                KeyParser.ParseExpression(container), container);
        }

        private static ParserWithContainer<Expression, ValuePropertyContainer> ValueTerm(ValuePropertyContainer container)
        {
            return new ParserWithContainer<Expression, ValuePropertyContainer>(
                ValueParser.ParseExpression(container), container);
        }

        private static readonly Parser<string> Period = Parse.Char('.').Return(".");

        private static readonly Parser<string> Identifier =
            (from head in Parse.Char('_').Or(Parse.Letter).AtLeastOnce().Text()
             from remain in (Parse.Char('_').Or(Parse.LetterOrDigit)).Many().Text()
             select head + remain).Named("Identifier");

        private static readonly Parser<string> FunctionName = Parse.ChainOperator(Period, Identifier, (t, l, r) => l + t + r);

        private static Parser<Tuple<PropertyInfo, Expression, Expression>> ValueOnlyArgumentTerm(ValuePropertyContainer container)
        {
            return from v in ValueTerm(container).Parser.Once()
                   select Tuple.Create<PropertyInfo, Expression, Expression>(container.EntityInfo.PropertyInfo, Expression.Empty(), v.First());
        }

        private static Parser<Expression> AlwaysFailure()
        {
            return i => Result.Failure<Expression>(i, string.Empty, new string[] { });
        }

        private static ParserWithContainer<Expression, KeyPropertyContainer> CallKeyTerm(AffectableEntityInfo info, 
            ParameterExpression parameter, int index)
        {
            GetSetProperty prop;
            if(!info.TryGetKeyProperty(index,out prop))
            {
                return new ParserWithContainer<Expression, KeyPropertyContainer>(
                    AlwaysFailure(), null);
            }
            var container = new KeyPropertyContainer(prop, parameter);
            return new ParserWithContainer<Expression, KeyPropertyContainer>(
                KeyTerm(container).Parser, container);
        }

        private static Parser<Tuple<PropertyInfo, Expression, Expression>> ArgumentsTerm(string functionName,
            AffectableEntityInfoContainer container)
        {
            AffectableEntityInfo info;
            if(!container.EntityInfoCache.TryGetAffectableEntityInfo(functionName,out info))
            {
                throw new ArgumentException(string.Format("Function name {0} is not found in entityInfoCache.", functionName));
            }

            var parameter = container.ParameterCache.Get(functionName);
            var prop = new ValuePropertyContainer(info, functionName, parameter);
            Func<int, Parser<Expression>> argsExpr = i => (from separator in Comma
                                                           from item in CallKeyTerm(info, parameter.Item1, i).Parser
                                                           select item);
            return (from keys in
                        (from firstKey in CallKeyTerm(info, parameter.Item1, 0).Parser.Once()
                         from tailKey in KeyTermMany(argsExpr, 1)
                         select firstKey.Concat(tailKey))
                    from comma in Comma
                    from v in ValueTerm(prop).Parser.Once()
                    select Tuple.Create(info.PropertyInfo, Join(keys.ToArray()), v.First())).Or(ValueOnlyArgumentTerm(prop));
        }

        private static Parser<IEnumerable<Expression>> KeyTermMany(Func<int, Parser<Expression>> parserFunc, int initial)
        {
            return i =>
            {
                var remainder = i;
                var result = new List<Expression>();
                var j = initial;
                var r = parserFunc(j)(i);

                while (r.WasSuccessful)
                {
                    if (remainder.Equals(r.Remainder))
                    {
                        break;
                    }
                    j++;
                    result.Add(r.Value);
                    remainder = r.Remainder;
                    r = parserFunc(j)(remainder);
                }
                return Result.Success<IEnumerable<Expression>>(result, remainder);
            };
        }

        private static Expression Join(Expression[] keys)
        {
            if (keys.Length==1)
            {
                return keys[0];
            }
            var bExpr = keys[0];
            for(int i=1;i<keys.Length;i++)
            {
                bExpr = Expression.MakeBinary(ExpressionType.AndAlso, bExpr, keys[i]);
            }
            return bExpr;
        }

        private static Parser<ExpressionResult> FunctionTerm(AffectableEntityInfoContainer container)
        {
            return from name in FunctionName
                   from lparen in Parse.Char('(')
                   from args in ArgumentsTerm(name, container)
                   from rparen in Parse.Char(')')
                   select new ExpressionResult
                   {
                       FunctionName = name,
                       TargetPropertyInfo = args.Item1,
                       Condition = args.Item2,
                       Value = args.Item3
                   };
        }

        private static ExpressionResultSet ToResult(IEnumerable<ExpressionResult> results, ParameterCache parameters)
        {
            return new ExpressionResultSet
            {
                ExpressionResults = results.ToArray(),
                Parameters = parameters
            };
        }

        private static Parser<ExpressionResultSet> Expr(AffectableEntityInfoContainer container)
        {
            return from f in FunctionTerm(container).DelimitedBy(Comma)
                   select ToResult(f, container.ParameterCache);
        }

        private static ParserWithContainer<ExpressionResultSet, AffectableEntityInfoContainer> Lambda(AffectableEntityInfoContainer container)
        {
            return new ParserWithContainer<ExpressionResultSet, AffectableEntityInfoContainer>(Expr(container).End(), container);
        }
    }
}
