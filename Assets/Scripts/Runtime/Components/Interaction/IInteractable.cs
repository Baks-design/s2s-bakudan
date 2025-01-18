namespace Game.Runtime.Systems.Interaction
{
    public interface IInteractable
    {
        public bool HoldInteract { get; }
        public bool IsInteractable { get; }

        public void OnInteract();
    }
}