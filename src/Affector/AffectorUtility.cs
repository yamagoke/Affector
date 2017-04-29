using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Affector
{
    internal class AffectorUtility
    {
        public static bool IsNullable(Type type)
        {
            return Nullable.GetUnderlyingType(type) != null;
        }

        // Todo
        public static Expression CreateValueExpression(Type type, Tuple<Type,string> value)
        {
            return TypeConverter.Convert(type, value.Item2);
        }
    }
}
