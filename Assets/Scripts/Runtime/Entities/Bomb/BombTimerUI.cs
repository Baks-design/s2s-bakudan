using Game.Runtime.Utilities.Helpers.Timers;
using KBCore.Refs;
using UnityEngine;

namespace Game.Runtime.Entities.Bomb
{
    public class BombUI : MonoBehaviour
    {
        [SerializeField, Self] TextMesh textMesh;

        public void UpdateTimer(CountdownTimer timer) => textMesh.text = timer.ToString();
    }
}