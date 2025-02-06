namespace Game.Runtime.Systems.Spawn
{
    public class EntitySpawner<T> where T : Entity
    {
        readonly IEntityFactory<T> entityFactory;
        readonly ISpawnPointStrategy spawnPointStrategy;

        public EntitySpawner(IEntityFactory<T> entityFactory, ISpawnPointStrategy spawnPointStrategy)
        {
            this.entityFactory = entityFactory;
            this.spawnPointStrategy = spawnPointStrategy;
        }

        public T Spawn() => entityFactory.Create(spawnPointStrategy.NextSpawnPoint());
    }
}