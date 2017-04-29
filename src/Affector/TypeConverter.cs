using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Affector
{
    internal class TypeConverter
    {
        public static Expression Convert(Type type, string value)
        {
            if(type==typeof(double))
            {
                return Expression.Constant(Double.Parse(value), type);
            }
            if (type == typeof(Int32))
            {
                return Expression.Constant(Int32.Parse(value), type);
            }
            if (type == typeof(string))
            {
                return Expression.Constant(value, type);
            }
            if (type == typeof(DateTime))
            {
                return Expression.Constant(DateTime.Parse(value), type);
            }
            if (type == typeof(Int16))
            {
                return Expression.Constant(Int16.Parse(value), type);
            }
            if (type == typeof(Int64))
            {
                return Expression.Constant(Int64.Parse(value), type);
            }
            if (type == typeof(UInt16))
            {
                return Expression.Constant(UInt16.Parse(value), type);
            }
            if (type == typeof(UInt32))
            {
                return Expression.Constant(UInt32.Parse(value), type);
            }
            if (type == typeof(UInt64))
            {
                return Expression.Constant(UInt64.Parse(value), type);
            }
            if (type == typeof(Single))
            {
                return Expression.Constant(Single.Parse(value), type);
            }
            if (type == typeof(bool))
            {
                return Expression.Constant(bool.Parse(value), type);
            }
            if (type == typeof(byte))
            {
                return Expression.Constant(byte.Parse(value), type);
            }
            if (type == typeof(sbyte))
            {
                return Expression.Constant(sbyte.Parse(value), type);
            }
            if (type == typeof(Char))
            {
                return Expression.Constant(Char.Parse(value), type);
            }
            if (type == typeof(TimeSpan))
            {
                return Expression.Constant(TimeSpan.Parse(value), type);
            }
            if (type == typeof(DateTimeOffset))
            {
                return Expression.Constant(DateTimeOffset.Parse(value), type);
            }
            if (type == typeof(decimal))
            {
                return Expression.Constant(decimal.Parse(value), type);
            }
            if (type == typeof(Guid))
            {
                return Expression.Constant(Guid.Parse(value), type);
            }
            if (type == typeof(double?))
            {
                return value.ToLower()!="null"
                    ? Expression.Constant(Double.Parse(value), type)
                    : Expression.Constant(null, type);
            }
            if (type == typeof(Int32?))
            {
                return value.ToLower() != "null"
                    ? Expression.Constant(Int32.Parse(value), type)
                    : Expression.Constant(null, type);
            }
            if (type == typeof(DateTime?))
            {
                return value.ToLower() != "null"
                    ? Expression.Constant(DateTime.Parse(value), type)
                    : Expression.Constant(null, type);
            }
            if (type == typeof(Int16?))
            {
                return value.ToLower() != "null"
                    ? Expression.Constant(Int16.Parse(value), type)
                    : Expression.Constant(null, type);
            }
            if (type == typeof(Int64?))
            {
                return value.ToLower() != "null"
                    ? Expression.Constant(Int64.Parse(value), type)
                    : Expression.Constant(null, type);
            }
            if (type == typeof(UInt16?))
            {
                return value.ToLower() != "null"
                    ? Expression.Constant(UInt16.Parse(value), type)
                    : Expression.Constant(null, type);
            }
            if (type == typeof(UInt32?))
            {
                return value.ToLower() != "null"
                    ? Expression.Constant(UInt32.Parse(value), type)
                    : Expression.Constant(null, type);
            }
            if (type == typeof(UInt64?))
            {
                return value.ToLower() != "null"
                    ? Expression.Constant(UInt64.Parse(value), type)
                    : Expression.Constant(null, type);
            }
            if (type == typeof(Single?))
            {
                return value.ToLower() != "null"
                    ? Expression.Constant(Single.Parse(value), type)
                    : Expression.Constant(null, type);
            }
            if (type == typeof(bool?))
            {
                return value.ToLower() != "null"
                    ? Expression.Constant(bool.Parse(value), type)
                    : Expression.Constant(null, type);
            }
            if (type == typeof(byte?))
            {
                return value.ToLower() != "null"
                    ? Expression.Constant(byte.Parse(value), type)
                    : Expression.Constant(null, type);
            }
            if (type == typeof(sbyte?))
            {
                return value.ToLower() != "null"
                    ? Expression.Constant(sbyte.Parse(value), type)
                    : Expression.Constant(null, type);
            }
            if (type == typeof(Char?))
            {
                return value.ToLower() != "null"
                    ? Expression.Constant(Char.Parse(value), type)
                    : Expression.Constant(null, type);
            }
            if (type == typeof(TimeSpan?))
            {
                return value.ToLower() != "null"
                    ? Expression.Constant(TimeSpan.Parse(value), type)
                    : Expression.Constant(null, type);
            }
            if (type == typeof(DateTimeOffset?))
            {
                return value.ToLower() != "null"
                    ? Expression.Constant(DateTimeOffset.Parse(value), type)
                    : Expression.Constant(null, type);
            }
            if (type == typeof(decimal?))
            {
                return value.ToLower() != "null"
                    ? Expression.Constant(decimal.Parse(value), type)
                    : Expression.Constant(null, type);
            }
            if (type == typeof(Guid?))
            {
                return value.ToLower() != "null"
                    ? Expression.Constant(Guid.Parse(value), type)
                    : Expression.Constant(null, type);
            }
            if (type.IsEnum)
            {
                return Expression.Constant(Enum.Parse(type, value), type);
            }

            var constructor = type.GetConstructor(new[] { typeof(string) });
            if (constructor == null)
            {
                throw new ArgumentException("constructor error.");
            }
            return Expression.New(constructor, Expression.Constant(value, typeof(string)));
        }
    }
}
