namespace Game.Runtime.Utilities.Patterns.Flyweight
{
    public interface IFlyweightService
    {
        public int SpaceLeft();
        public Flyweight Spawn(FlyweightSettings settings);
        public void ReturnToPool(Flyweight f);
    }
}