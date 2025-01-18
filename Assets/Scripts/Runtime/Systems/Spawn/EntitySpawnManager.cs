using UnityEngine;

namespace Game.Runtime.Systems.Spawn
{
    public abstract class EntitySpawnManager : MonoBehaviour
    {
        protected enum SpawnPointStrategyType
        {
            Linear,
            Random
        }
        
        [SerializeField] protected SpawnPointStrategyType spawnPointStrategyType = SpawnPointStrategyType.Linear;
        [SerializeField] protected Transform[] spawnPoints;
        protected ISpawnPointStrategy spawnPointStrategy;

        protected virtual void Awake() => spawnPointStrategy = spawnPointStrategyType switch
        {
            SpawnPointStrategyType.Linear => new LinearSpawnPointStragegy(spawnPoints),
            SpawnPointStrategyType.Random => new RandomSpawnPointStrategy(spawnPoints),
            _ => spawnPointStrategy
        };

        public abstract void Spawn();
    }
}