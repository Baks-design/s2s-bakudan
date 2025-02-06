using Game.Runtime.Components.Damage;
using Game.Runtime.Utilities.Helpers;
using UnityEngine;

namespace Game.Runtime.Entities.Packs
{
    public class HealthPack : MonoBehaviour
    {
        [SerializeField] int restoreAmount = 25;

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(TagsIDs.PlayerTag) && other.TryGetComponent(out Damageable damageable))
            {
                if (damageable.IsAbleToCure)
                {
                    damageable.Heal(restoreAmount);
                    Destroy(gameObject);
                }
            }
        }
    }
}