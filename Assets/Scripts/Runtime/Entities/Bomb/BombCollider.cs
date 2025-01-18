using KBCore.Refs;
using UnityEngine;

namespace Game.Runtime.Entities.Bomb
{
    public class BombCollider : MonoBehaviour
    {
        [SerializeField, Parent] Rigidbody rb;
        [SerializeField, Self] Collider col;

        public Transform PlayerPosition { get; private set; }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                PlayerPosition = other.transform;
                rb.useGravity = false;
                col.isTrigger = true;
            }
        }
    }
}