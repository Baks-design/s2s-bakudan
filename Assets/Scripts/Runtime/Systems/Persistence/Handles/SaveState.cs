using Game.Runtime.Utilities.Helpers;
using Game.Runtime.Utilities.Patterns.ServiceLocator;
using UnityEngine;

namespace Game.Runtime.Systems.Persistence
{
    public class SaveState : MonoBehaviour
    {
        ISerializeService serializeService;

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(GameTags.PlayerTag))
            {
                ServiceLocator.Global.Get(out serializeService);
                serializeService.SaveGame();
            }
        }
    }
}