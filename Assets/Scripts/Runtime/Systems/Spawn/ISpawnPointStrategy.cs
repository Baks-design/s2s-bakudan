using UnityEngine;

namespace Game.Runtime.Systems.Spawn
{
    public interface ISpawnPointStrategy 
    {
        Transform NextSpawnPoint();
    }
}