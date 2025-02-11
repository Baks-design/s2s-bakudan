using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Game.Runtime.Utilities.Patterns.Flyweight
{
    public class FlyweightFactory : MonoBehaviour, IFlyweightService
    {
        [SerializeField] bool collectionCheck = true;
        [SerializeField] int defaultCapacity = 10;
        [SerializeField] int maxPoolSize = 100;
        readonly Dictionary<FlyweightType, IObjectPool<Flyweight>> pools = new();
        readonly HashSet<Flyweight> activeFlyweights = new();

        public Flyweight Spawn(FlyweightSettings settings)
        {
            var flyweight = GetPoolFor(settings)?.Get();
            if (flyweight != null)
                activeFlyweights.Add(flyweight);
            return flyweight;
        }

        public int SpaceLeft() => defaultCapacity - activeFlyweights.Count;

        public void ReturnToPool(Flyweight f)
        {
            if (activeFlyweights.Contains(f))
                activeFlyweights.Remove(f);
            GetPoolFor(f.settings)?.Release(f);
        }

        IObjectPool<Flyweight> GetPoolFor(FlyweightSettings settings)
        {
            if (pools.TryGetValue(settings.type, out var pool)) return pool;

            pool = new ObjectPool<Flyweight>(
                settings.Create,
                settings.OnGet,
                settings.OnRelease,
                settings.OnDestroyPoolObject,
                collectionCheck,
                defaultCapacity,
                maxPoolSize
            );
            pools.Add(settings.type, pool);
            return pool;
        }
    }
}