using System.Collections.Generic;
using System.Linq;

namespace Game.Runtime.Utilities.Patterns.StateMachines
{
    public class Or : IPredicate
    {
        public List<IPredicate> rules = new();

        public bool Evaluate() => rules.Any(r => r.Evaluate());
    }
}