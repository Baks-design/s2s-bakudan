using UnityEngine;

namespace Game.Runtime.Systems.Spawn
{
    public class LinearSpawnPointStragegy : ISpawnPointStrategy 
    {
        int index = 0;
        readonly Transform[] spawnPoints;

        public LinearSpawnPointStragegy(Transform[] spawnPoints) => this.spawnPoints = spawnPoints;

        public Transform NextSpawnPoint() 
        {
            var result = spawnPoints[index];
            index = (index + 1) % spawnPoints.Length;
            return result;
        }
    }
}