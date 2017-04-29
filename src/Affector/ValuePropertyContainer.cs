using System;
using System.Linq.Expressions;

namespace Affector
{
    internal class ValuePropertyContainer
    {
        public AffectableEntityInfo EntityInfo { get; private set; }
        public GetSetProperty[] Properties { get; private set; }
        public ParameterExpression Parameter { get; private set; }
        public ParameterExpression NewParameter { get; private set; }

        public ValuePropertyContainer(AffectableEntityInfo entityInfo, string targetName,
            Tuple<ParameterExpression, ParameterExpression> parameters)
        {
            EntityInfo = entityInfo;
            Properties = entityInfo.GetValueProperty(targetName);
            Parameter = parameters.Item1;
            NewParameter = parameters.Item2;
        }
    }
}
