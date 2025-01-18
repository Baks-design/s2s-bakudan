namespace Game.Runtime.Utilities.Patterns.StateMachines
{
    public class Not : IPredicate
    {
        public IPredicate rule;

        public bool Evaluate() => !rule.Evaluate();
    }
}