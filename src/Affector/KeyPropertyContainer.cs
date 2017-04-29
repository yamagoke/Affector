using System.Linq.Expressions;

namespace Affector
{
    internal class KeyPropertyContainer
    {
        public GetSetProperty Property { get; private set; }
        public ParameterExpression Parameter { get; private set; }

        public KeyPropertyContainer(GetSetProperty property,ParameterExpression parameter)
        {
            Property = property;
            Parameter = parameter;
        }
    }
}
