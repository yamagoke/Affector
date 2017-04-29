using Sprache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Affector
{
    internal class ParserUtility
    {
        public static Parser<ExpressionType> Operator(string op, ExpressionType opType)
        {
            return Parse.String(op).Token().Return(opType);
        }

        public static readonly Parser<Tuple<Type, string>> ConstantDecimal =
            Parse.Decimal
                .Select(x => Tuple.Create(typeof(double), x))
                .Named("number");

        public static readonly Parser<Tuple<Type, string>> ExponentialNotation =
            (from first in Parse.Decimal
             from e in Parse.IgnoreCase('e')
             from sign in Parse.Char('-').XOr(Parse.Char('+')).XOr(Parse.Return('+'))
             from last in Parse.Number
             select first + e + sign + last)
            .Select(x => Tuple.Create(typeof(double), x)).Named("exponentialNumber");

        public static readonly Parser<Tuple<Type, string>> ConstantNumber =
            ((from sign in Parse.Char('-')
              from factor in ExponentialNotation.Or(ConstantDecimal)
              select Tuple.Create(factor.Item1, "-" + factor.Item2))
            .XOr(ExponentialNotation).Or(ConstantDecimal)).Token();

        public static readonly Parser<Tuple<Type, string>> ConstantString =
            (from lparen in Parse.Char('"')
             from expr in
                 (from s in Parse.CharExcept('"').AtLeastOnce().Text() select Tuple.Create(s.GetType(), s))
             from rparen in Parse.Char('"')
             select expr).Or(
                from s in Parse.CharExcept(new[] { ',', '(', ')' }).AtLeastOnce().Text() select Tuple.Create(s.GetType(), s))
            .Named("string");

        //public static readonly Parser<ExpressionType> AddAssign = Operator("+", ExpressionType.AddAssignChecked);
        //public static readonly Parser<ExpressionType> SubtractAssign = Operator("-", ExpressionType.SubtractAssignChecked);
        //public static readonly Parser<ExpressionType> MultiplyAssign = Operator("*", ExpressionType.MultiplyAssignChecked);
        //public static readonly Parser<ExpressionType> DivideAssign = Operator("/", ExpressionType.DivideAssign);
        //public static readonly Parser<ExpressionType> ModuloAssign = Operator("%", ExpressionType.ModuloAssign);
        //public static readonly Parser<ExpressionType> PowerAssign = Operator("^", ExpressionType.PowerAssign);
        //public static readonly Parser<ExpressionType> Assign = Operator("=", ExpressionType.Assign);
    }
}
