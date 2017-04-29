using Sprache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Affector
{
    internal class KeyParser
    {
        public static Parser<Expression> ParseExpression(KeyPropertyContainer container)
        {
            return ConditionExpr(container).Parser;
        }

        private static readonly Parser<ExpressionType> And = ParserUtility.Operator("&&", ExpressionType.AndAlso);
        private static readonly Parser<ExpressionType> Or = ParserUtility.Operator("||", ExpressionType.OrElse);

        private static readonly Parser<ExpressionType> GreaterThan = ParserUtility.Operator(">", ExpressionType.GreaterThan);
        private static readonly Parser<ExpressionType> GreaterThanOrEqual = ParserUtility.Operator(">=", ExpressionType.GreaterThanOrEqual);
        private static readonly Parser<ExpressionType> LessThan = ParserUtility.Operator("<", ExpressionType.LessThan);
        private static readonly Parser<ExpressionType> LessThanOrEqual = ParserUtility.Operator("<=", ExpressionType.LessThanOrEqual);
        private static readonly Parser<ExpressionType> Equal = ParserUtility.Operator("=", ExpressionType.Equal);
        private static readonly Parser<ExpressionType> NotEqual = ParserUtility.Operator("!=", ExpressionType.NotEqual);

        private static BinaryExpression CallCondition(ExpressionType operand, Tuple<Type, string> value, KeyPropertyContainer container)
        {
            var expr = container.Property.GenerateGetProperty(container.Parameter);
            var tmpValue = AffectorUtility.CreateValueExpression(expr.Type, value);
            return Expression.MakeBinary(operand, expr, tmpValue);
        }

        private static ParserWithContainer<Expression, KeyPropertyContainer> ConditionFactor(KeyPropertyContainer container)
        {
            return new ParserWithContainer<Expression, KeyPropertyContainer>(
                (from lparen in Parse.Char('(').Token()
                 from expr in Parse.Ref(() => ConditionExpr(container).Parser)
                 from rparen in Parse.Char(')').Token()
                 select expr).Named("expression"), container);
        }

        private static ParserWithContainer<Expression, KeyPropertyContainer> ConditionTerm(KeyPropertyContainer container)
        {
            return new ParserWithContainer<Expression, KeyPropertyContainer>(
                (from operand in GreaterThanOrEqual.Or(LessThanOrEqual).Or(LessThan).Or(GreaterThan).Or(Equal).Or(NotEqual)
                 from value in ParserUtility.ConstantNumber.Or(ParserUtility.ConstantString)
                 select CallCondition(operand, value, container)).Named("conditionTerm")
                 .XOr(ConditionFactor(container).Parser), container);
        }

        private static ParserWithContainer<Expression, KeyPropertyContainer> AndConditionExpr(KeyPropertyContainer container)
        {
            return new ParserWithContainer<Expression, KeyPropertyContainer>(
                Parse.ChainOperator(And, ConditionTerm(container).Parser, Expression.MakeBinary),
                container);
        }

        private static ParserWithContainer<Expression, KeyPropertyContainer> ConditionExpr(KeyPropertyContainer container)
        {
            return new ParserWithContainer<Expression, KeyPropertyContainer>(
                Parse.ChainOperator(Or, AndConditionExpr(container).Parser, Expression.MakeBinary),
                container);
        }


    }
}
