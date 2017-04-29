using Sprache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Affector
{
    internal class ValueParser
    {
        public static Parser<Expression> ParseExpression(ValuePropertyContainer container)
        {
            return Expr(container).Parser;
        }

        private static readonly Parser<ExpressionType> AddAssign = ParserUtility.Operator("+", ExpressionType.AddAssignChecked);
        private static readonly Parser<ExpressionType> SubtractAssign = ParserUtility.Operator("-", ExpressionType.SubtractAssignChecked);
        private static readonly Parser<ExpressionType> MultiplyAssign = ParserUtility.Operator("*", ExpressionType.MultiplyAssignChecked);
        private static readonly Parser<ExpressionType> DivideAssign = ParserUtility.Operator("/", ExpressionType.DivideAssign);
        private static readonly Parser<ExpressionType> ModuloAssign = ParserUtility.Operator("%", ExpressionType.ModuloAssign);
        private static readonly Parser<ExpressionType> PowerAssign = ParserUtility.Operator("^", ExpressionType.PowerAssign);
        private static readonly Parser<ExpressionType> Assign = ParserUtility.Operator("=", ExpressionType.Assign);

        private static Expression CallSetValue(ExpressionType operand, Tuple<Type, string> value, ValuePropertyContainer container)
        {
            var infos = container.Properties.Select(prop => new ValueOperatorInfo
            {
                ExpressionType = operand,
                PropertyName = prop.PropertyInfo.Name,
                Value = AffectorUtility.CreateValueExpression(prop.PropertyInfo.PropertyType, value),
            }).ToArray();
            return container.EntityInfo.IfThenSection(container.Parameter, container.NewParameter, infos);
        }

        private static readonly Parser<ExpressionType> NumericOperatorTerm =
            AddAssign.Or(SubtractAssign).Or(MultiplyAssign).Or(DivideAssign).Or(ModuloAssign)
            .Or(PowerAssign).Or(Assign);

        private static readonly Parser<ExpressionType> StringOperatorTerm = AddAssign.Or(Assign);

        private static Parser<Expression> NumericTerm(ValuePropertyContainer container)
        {
            return (from op in NumericOperatorTerm
                    from value in ParserUtility.ConstantNumber
                    select CallSetValue(op, value, container))
                    .Named("NumericValueExpression");
        }

        private static Parser<Expression> StringTerm(ValuePropertyContainer container)
        {
            return (from op in StringOperatorTerm
                    from value in ParserUtility.ConstantString
                    select CallSetValue(op, value, container))
                    .Named("StringValueExpression");
        }

        private static ParserWithContainer<Expression, ValuePropertyContainer> Expr(ValuePropertyContainer container)
        {
            return new ParserWithContainer<Expression, ValuePropertyContainer>(
                NumericTerm(container).Or(StringTerm(container)), container);
        }
    }
}
