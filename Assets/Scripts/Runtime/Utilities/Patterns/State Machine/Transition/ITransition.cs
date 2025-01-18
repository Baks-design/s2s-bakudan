namespace Game.Runtime.Utilities.Patterns.StateMachines
{
    public interface ITransition
    {
        IState To { get; }
        IPredicate Condition { get; }
    }
}