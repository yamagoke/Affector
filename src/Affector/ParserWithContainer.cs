using Sprache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Affector
{
    internal class ParserWithContainer<TParser, TContainer>
    {
        public Parser<TParser> Parser { get; private set; }
        public TContainer Container { get; private set; }
        public ParserWithContainer(Parser<TParser> parser, TContainer container)
        {
            Parser = parser;
            Container = container;
        }
    }
}
