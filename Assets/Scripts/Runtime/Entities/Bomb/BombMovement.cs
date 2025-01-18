using KBCore.Refs;
using UnityEngine;

namespace Game.Runtime.Entities.Bomb
{
    public class BombMovement : MonoBehaviour
    {
        [SerializeField, Self] Transform tr;
        [SerializeField, Child] BombCollider col;
        [SerializeField, Self] Rigidbody rb;
        [SerializeField] float followSpeed = 5f;
        [SerializeField] float stopDistance = 1f;

        public void FollowEntities()
        {
            if (!col.PlayerPosition) return;

            var distance = Vector3.Distance(tr.position, col.PlayerPosition.position);
            if (distance > stopDistance)
                rb.linearVelocity = Vector3.Lerp(tr.position, col.PlayerPosition.position, followSpeed * Time.deltaTime);
        }
    }
}