using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Affector
{
    internal class ExpressionResultSet
    {
        public ParameterCache Parameters { get; set; }
        public ExpressionResult[] ExpressionResults { get; set; }
    }
}
