using System.Collections.Generic;
using System.Linq;

namespace Game.Runtime.Utilities.Patterns.StateMachines
{
    public class And : IPredicate
    {
        public List<IPredicate> rules = new();

        public bool Evaluate() => rules.All(r => r.Evaluate());
    }
}