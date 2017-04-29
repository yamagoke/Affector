using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Affector
{
    internal class AffectableEntityInfoCache
    {
        private readonly Dictionary<string, AffectableEntityInfo> _affectableCache;
        private readonly Dictionary<string, PropertyInfo> _nonAffectableCache;
        public Type Type { get; private set; }

        public AffectableEntityInfoCache(Type type)
        {
            Type = type;
            _affectableCache = AffectableProperties(type).Select(_ => new AffectableEntityInfo(_))
                .ToDictionary(_ => _.PropertyName, _ => _);
            _nonAffectableCache = NonAffectableProperties(type)
                .ToDictionary(_ => _.Item2, _ => _.Item1);
        }

        private IEnumerable<PropertyInfo> AffectableProperties(Type type)
        {
            return type.GetProperties().Where(pi =>
                pi.PropertyType.IsArray &&
                pi.PropertyType.GetElementType().GetCustomAttribute(typeof(AffectableAttribute)) != null);
        }

        private IEnumerable<Tuple<PropertyInfo, string>> NonAffectableProperties(Type type)
        {
            foreach(var prop in type.GetProperties())
            {
                if (!_affectableCache.ContainsKey(prop.Name))
                {
                    yield return Tuple.Create(prop, prop.Name);
                }
            }
        }

        public IEnumerable<KeyValuePair<string,PropertyInfo>> NonAffectablePropertyInfos
        {
            get { return _nonAffectableCache.Select(_ => _); }
        }

        public IEnumerable<KeyValuePair<string, AffectableEntityInfo>> AffectablePropertyInfos
        {
            get { return _affectableCache.Select(_ => _); }
        }

        public AffectableEntityInfo AffectableEntityInfo(string propertyName)
        {
            return _affectableCache[propertyName];
        }

        public bool TryGetAffectableEntityInfo(string targetValueName, out AffectableEntityInfo info)
        {
            var rtn = new List<AffectableEntityInfo>();
            foreach (var entityInfo in _affectableCache.Values)
            {
                var result = entityInfo.PropertyExpressions.Values.FirstOrDefault(
                    _ => _.TargetValueNames.Any(v => v == targetValueName));
                if (result != null)
                {
                    rtn.Add(entityInfo);
                }
            }
            if(rtn.Count==1)
            {
                info = rtn[0];
                return true;
            }
            if(rtn.Count==0)
            {
                info = null;
                return false;
            }
            throw new ArgumentException("duplicate targetValueName.");
        }
    }
}
