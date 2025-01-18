using UnityEngine;

namespace Game.Runtime.Systems.Spawn
{
    public interface IEntityFactory<T> where T : Entity 
    {
        T Create(Transform spawnPoint);
    }
}