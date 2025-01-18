using UnityEngine;

namespace Game.Runtime.Systems.Spawn
{
    public class EntityFactory<T> : IEntityFactory<T> where T : Entity 
    {
        readonly EntityData[] data;

        public EntityFactory(EntityData[] data) => this.data = data;

        public T Create(Transform spawnPoint) 
        {
            var entityData = data[Random.Range(0, data.Length)];
            var instance = GameObject.Instantiate(entityData.prefab, spawnPoint.position, spawnPoint.rotation);
            return instance.GetComponent<T>();
        }
    }
}