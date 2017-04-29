using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Affector
{
    internal class AffectableEntityInfoContainer
    {
        public AffectableEntityInfoCache EntityInfoCache { get; private set; }
        public ParameterCache ParameterCache { get; private set; }

        public AffectableEntityInfoContainer(AffectableEntityInfoCache cache)
        {
            EntityInfoCache = cache;
            ParameterCache = new ParameterCache(cache);
        }
    }
}
