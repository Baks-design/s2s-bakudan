namespace Game.Runtime.Utilities.Patterns.StateMachines
{
    public interface IState
    {
        void OnEnter() { }
        void FixedUpdate() { }
        void Update() { }
        void OnExit() { }
    }
}