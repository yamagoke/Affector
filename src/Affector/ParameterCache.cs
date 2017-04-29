using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Affector
{
    internal class ParameterCache
    {
        private readonly AffectableEntityInfoCache _cache;
        private readonly Dictionary<string, Tuple<ParameterExpression, ParameterExpression>> _selectItemParameterCache
            = new Dictionary<string, Tuple<ParameterExpression, ParameterExpression>>();

        public ParameterCache(AffectableEntityInfoCache cache)
        {
            _cache = cache;
        }

        public Tuple<ParameterExpression, ParameterExpression> Get(string functionName)
        {
            AffectableEntityInfo entityInfo;
            if(!_cache.TryGetAffectableEntityInfo(functionName, out entityInfo))
            {
                throw new ArgumentException(string.Format("function name ({0}) is not found.", functionName));
            }

            Tuple<ParameterExpression, ParameterExpression> rtn;
            if(!_selectItemParameterCache.TryGetValue(entityInfo.PropertyName, out rtn))
            {
                rtn = Tuple.Create(Expression.Parameter(entityInfo.Type, "entityParameter"),
                    Expression.Variable(entityInfo.Type, "newEntityParameter"));
                _selectItemParameterCache[entityInfo.PropertyName] = rtn;
            }
            return rtn;
        }
    }
}
