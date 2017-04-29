using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Affector
{
    public static class Affector 
    {
        private static readonly LambdaExpressionBuilder Builder;
        private static readonly Dictionary<Type, AffectableEntityInfoContainer> Cache;

        static Affector()
        {
            Builder = new LambdaExpressionBuilder();
            Cache = new Dictionary<Type, AffectableEntityInfoContainer>();
        }

        private static AffectableEntityInfoContainer GetContainer(Type type)
        {
            AffectableEntityInfoContainer container;
            if(!Cache.TryGetValue(type, out container))
            {
                var cache = new AffectableEntityInfoCache(type);
                container = new AffectableEntityInfoContainer(cache);
                Cache[type] = container;
            }
            return container;
        }

        public static Func<TEntitySet, TEntitySet> Generate<TEntitySet>(string script)
            where TEntitySet : class
        {
            if(string.IsNullOrWhiteSpace(script))
            {
                return _ => _;
            }

            Type type = typeof(TEntitySet);
            var container = GetContainer(type);
            var expr = AffectableScriptParser.ParseExpression(script, container);
            var lambda = Builder.Build<TEntitySet>(expr, container.EntityInfoCache);
            return lambda.Compile();

        }
    }
}
