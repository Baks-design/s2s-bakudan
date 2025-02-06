using UnityEngine;
using Game.Runtime.Entities.Bomb;
using Game.Runtime.Components.UI;

namespace Game.Runtime.Systems.Management.Setup
{
    public class BombsSetup : MonoBehaviour
    {
        BombController[] bombs;
        MiniMapView minimap;

        void Start()
        {
            FindEntities();
            SetupMinimap();
        }

        void FindEntities()
        {
            bombs = FindObjectsByType<BombController>(FindObjectsSortMode.None);
            minimap = FindFirstObjectByType<MiniMapView>();
        }

        void SetupMinimap()
        {
            for (var i = 0; i < bombs.Length; i++)
            {
                var img2 = minimap.Follow(bombs[i].transform);
                img2.color = new Color(1, 1, 1);
            }
        }
    }
}